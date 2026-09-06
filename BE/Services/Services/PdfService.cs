
using iTextSharp.text;
using iTextSharp.text.pdf;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.SDK.Error;
using Ordbox.Services.ARCA.Enum;
using Ordbox.Services.Common;
using Ordbox.Services.Models.Dtos.DtoRequest;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;
using Document = iTextSharp.text.Document;
using Font = iTextSharp.text.Font;
using Paragraph = iTextSharp.text.Paragraph;
using Path = System.IO.Path;

namespace Ordbox.Services.Services
{
    public class PdfService
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment _Env;
        private ErrorManager _logger;

        private bool mostrarIvaTotal;
        private bool mostrarIvaTotal2;
        private bool mostrarIvaA;
        private bool mostrarIvaB;
        private BaseColor boldColor = BaseColor.Black;
        private Font textFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
        public PdfService(ErrorManager logger, IConfiguration configuration, IWebHostEnvironment env)

        {
            _configuration = configuration;
            _Env = env;
            _logger = logger;
        }

        public Task<OperationResponse<byte[]>> PrintInvoiceARCA(DtoRequestInvoice invoice)
        {
            using (var stream = new MemoryStream())
            using (Document document = new Document(PageSize.A4, 5f, 5f, 15f, 80f))
            {
                try
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    writer.PageEvent = new FooterWithCAEEvent(invoice, _configuration.GetSection("Pdf:Cuit").Value.ToString());

                    document.Open();

                    document.Add(this.CabeceraArca(invoice));

                    int itemsPerPage = 25;
                    int itemCount = 0;

                    PdfPTable table = CrearTablaDetalle();

                    foreach (var item in invoice.InvoiceDetails)
                    {
                        AgregarFilaDetalle(table, item);

                        itemCount++;

                        if (itemCount == itemsPerPage)
                        {
                            document.Add(table);
                            document.NewPage();
                            table = CrearTablaDetalle();
                            itemCount = 0;
                        }
                    }

                    if (itemCount > 0)
                    {
                        document.Add(table);
                    }

                    PdfPTable totalIva = CrearTablaTotales(invoice);
                    document.Add(totalIva);

                    document.Close();
                    byte[] pdfBytes = stream.ToArray();
                    return Task.FromResult(new OperationResponse<byte[]>(pdfBytes));

                }
                catch (Exception ex)
                {
                    _logger.LogError(ErrorsCodes.C_010_ERROR_EXCEPTION, ex: ex);
                    throw;
                }
            }
        }

        public async Task<OperationResponse<byte[]>> Imprimir(Paragraph paragraph, DtoRequestInvoice invoice = null)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 5f, 5f, 15f, 80f);

                string filePath = Path.Combine(_Env.ContentRootPath, "PDF_Factura");
                string fileName = $"archivo_{DateTime.Now.ToString("yyyyMMdd")}.pdf";
                string fullPath = Path.Combine(filePath, fileName);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                try
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);

                    if (invoice != null && !string.IsNullOrEmpty(invoice.CAE))
                    {
                        writer.PageEvent = new FooterWithCAEEvent(invoice, _configuration.GetSection("Pdf:Cuit").Value.ToString());
                    }
                    document.Open();
                    document.Add(paragraph);
                    document.Close();

                    byte[] pdfBytes = stream.ToArray();

                    File.WriteAllBytes(fullPath, pdfBytes);

                    return new OperationResponse<byte[]>(pdfBytes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ErrorsCodes.C_010_ERROR_EXCEPTION, ex: ex);
                    throw;
                }
                finally
                {
                    document.Dispose();
                    stream.Dispose();
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
            }
        }

        public async Task<Paragraph> Encabezado(DtoRequestEncabezadoPDF model)
        {

            string titulo = _configuration.GetSection("Pdf:Name").Value;
            string dni = _configuration.GetSection("Pdf:Cuit").Value;
            string direccion = _configuration.GetSection("Pdf:Direccion").Value;
            string nombre_apellido = _configuration.GetSection("Pdf:Nombre").Value;
            string email = _configuration.GetSection("Pdf:Email").Value;

            string imagePath = Path.Combine(_Env.WebRootPath, "Assets", "dantesLogo1.png");
            // Crear el objeto de imagen
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imagePath);

            // Establecer el tamaño de la imagen (opcional)
            image.ScaleToFit(60f, 60f); // Ajusta la imagen al tamaño máximo de 50x50 puntos

            PdfPTable table = new PdfPTable(2);

            Phrase phrase = new Phrase();

            // Estilo para el título
            Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.Black);
            Chunk titleChunk = new Chunk(titulo, titleFont);

            Font titletextFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.Black);
            Chunk titleComprobante = new(model.TituloComprobante, titletextFont);
            Chunk numero = new(model.NumeroComprobante, titletextFont);

            phrase.Add(new Chunk(titleChunk));
            phrase.Add(Chunk.Newline);
            phrase.Add(Chunk.Newline);
            phrase.Add(new Chunk(""));
            phrase.Add(new Chunk(titleComprobante));

            phrase.Add(Chunk.Newline);
            phrase.Add(new Chunk("N°: ", titletextFont));
            phrase.Add(new Chunk(numero));
            phrase.Add(Chunk.Newline);
            phrase.Add(new Chunk(dni, titletextFont));
            phrase.Add(Chunk.Newline);
            phrase.Add(new Chunk(direccion, titletextFont));
            phrase.Add(Chunk.Newline);
            phrase.Add(new Chunk(nombre_apellido, titletextFont));
            phrase.Add(Chunk.Newline);
            phrase.Add(new Chunk(email, titletextFont));

            // Primera columna: texto
            PdfPCell textCell = new PdfPCell(phrase)
            {
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 10f,
                VerticalAlignment = Element.ALIGN_LEFT,
                PaddingBottom = -10f
            };
            // Establecer alineación y tamaño de fuente para el título
            textCell.HorizontalAlignment = Element.ALIGN_LEFT;
            textCell.Phrase.Font.Size = 16;

            // Agregar la celda a la tabla
            table.AddCell(textCell);

            // Segunda columna: imagen
            PdfPCell imageCell = new PdfPCell(image)
            {
                Border = PdfPCell.NO_BORDER,
                Padding = 0f,
                VerticalAlignment = Element.ALIGN_RIGHT,
                PaddingTop = 20f,// Mueve la imagen mas abajo
                PaddingBottom = -10f
            };
            table.AddCell(imageCell);

            float[] columnWidths = { 6f, 2f };
            table.SetWidths(columnWidths);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(table);

            return paragraph;
        }

        public async Task<Paragraph> Cabecera(DtoRequestCabeceraPDF model)
        {
            //Le agrego color a la letra de la tabla y tamaño
            BaseColor black = BaseColor.Black;
            Font fontTitle = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD, black);
            Font fontText = FontFactory.GetFont(FontFactory.HELVETICA, 8);

            var resumen = model;
            Paragraph parrafo = new();
            string CUIT = null;

            if (resumen.Cuit != null)
            {
                CUIT = resumen.Cuit.ToString();
            }

            string Cliente = resumen.Nombre;
            string Dirección = resumen.Direccion;
            DateTime fecha = resumen.Fecha;


            // Mover la declaración fuera del bucle
            Phrase textoIzquierda = new()
            {
                new Chunk("Cliente: " + Cliente, fontText),
                Chunk.Newline,
                (CUIT != null) ? new Chunk("CUIT: " + CUIT, fontText) : null,
                Chunk.Newline,
                new Chunk("Dirección: " + Dirección, fontText),
            };

            PdfPTable table = new PdfPTable(2);
            //table.SpacingAfter = -10f;

            // Primera columna: paragraph
            PdfPCell cell1 = new PdfPCell(textoIzquierda)
            {
                Border = PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER,
                PaddingTop = 5f,
                PaddingBottom = 5f,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            mostrarIvaTotal = string.Equals(resumen.Tipo, "B", StringComparison.OrdinalIgnoreCase);
            mostrarIvaTotal2 = string.Equals(resumen.Tipo, "A", StringComparison.OrdinalIgnoreCase);

            mostrarIvaA = string.Equals(resumen.Tipo, "1", StringComparison.OrdinalIgnoreCase);
            mostrarIvaB = string.Equals(resumen.Tipo, "2", StringComparison.OrdinalIgnoreCase);

            //Segunda Columna
            Phrase textoDerecha = new()
            {
               (resumen.Tipo != null) ? new Chunk("Tipo: " + resumen.Tipo, fontText) : null,
                Chunk.Newline,
                new Chunk("Fecha: " + fecha.ToString("yyyy-MM-dd"), fontText),
                Chunk.Newline
            };

            PdfPCell cell2 = new PdfPCell(textoDerecha)
            {
                Border = PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER,
                PaddingTop = 5f,
                PaddingBottom = 5f,
                HorizontalAlignment = Element.ALIGN_RIGHT
            };

            table.AddCell(cell1);
            table.AddCell(cell2);
            parrafo.Add(table);

            return parrafo;
        }


        public async Task<Paragraph> Detalle(DtoRequestDetallePDF model)
        {

            Paragraph paragraph = new Paragraph();
            PdfPTable table = new PdfPTable(5);

            // Establecer el ancho de las columnas
            float[] columnWidths = { 2f, 6f, 3f, 1f, 2f }; // Ancho entre columnas
            table.SetWidths(columnWidths);

            //Le agrego color a la letra de la tabla y tamaño
            BaseColor black = BaseColor.Black;
            Font font = FontFactory.GetFont(FontFactory.HELVETICA, 7, Font.BOLD, black);

            table.AddCell(new PdfPCell(new Phrase("Cantidad", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });

            PdfPCell productoCell = new PdfPCell(new Phrase("Producto", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            };

            table.AddCell(productoCell);
            table.AddCell(new PdfPCell(new Phrase("Precio Unitario", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });


            table.AddCell(new PdfPCell(new Phrase("Iva", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });

            table.AddCell(new PdfPCell(new Phrase("Importe", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });


            var resumen = model;

            //COmprobante Compra
            if (resumen.Detalle != null && resumen.Detalle.Any())
            {


                foreach (var item in resumen.Detalle)
                {

                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.ProductName, textFont))
                    {
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", item.Price), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase((item.Iva % 1 == 0) ? ((int)item.Iva).ToString() : item.Iva.ToString(CultureInfo.InvariantCulture), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", (item.Price * item.Quantity)), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });
                }

            }


            //Presupuesto
            if (resumen.BudgetDetails != null && resumen.BudgetDetails.Any())
            {
                foreach (var item in resumen.BudgetDetails)
                {

                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.ProductName, textFont))
                    {
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });


                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", item.Price), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(" ", textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", (item.Price * item.Quantity)), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                }
            }


            table.AddCell(paragraph);


            PdfPCell emptyCell = new PdfPCell()
            {
                Border = PdfPCell.NO_BORDER
            };

            Paragraph paragraphDetalle = new Paragraph();
            paragraphDetalle.Add(table);

            // Agregar espacio vertical entre las tablas
            Paragraph paragraphSaltoDeLinea = new Paragraph();
            paragraphSaltoDeLinea.Add(new Paragraph(" "));

            Paragraph paragraphTotal = new Paragraph();

            PdfPTable table2 = new PdfPTable(4);


            float[] columnWidths2 = { 1f, 1f, 1f, 1f }; // Ancho relativo de cada columna
            table2.SetWidths(columnWidths2);

            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);

            if (mostrarIvaTotal != true && mostrarIvaTotal2 != true && mostrarIvaB != true && mostrarIvaA != true)
            {
                if (resumen.Iva10 != 0)
                {

                    table2.AddCell(new PdfPCell(new Phrase("Iva 10: ", textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                    });

                    //table2.AddCell(emptyCell);
                    table2.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + resumen.Iva10), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                    });
                    table2.AddCell(emptyCell);
                }

                if (resumen.Iva21 != 0)
                {

                    table2.AddCell(emptyCell);
                    table2.AddCell(emptyCell);
                    table2.AddCell(emptyCell);
                    table2.AddCell(emptyCell);
                    table2.AddCell(new PdfPCell(new Phrase("Iva 21: ", textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                    });


                    table2.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + resumen.Iva21), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                    });

                    table2.AddCell(emptyCell);
                    table2.AddCell(emptyCell);
                }

                if (resumen.Iva27 != 0)
                {

                    table2.AddCell(emptyCell);
                    table2.AddCell(emptyCell);
                    table2.AddCell(emptyCell);

                    table2.AddCell(new PdfPCell(new Phrase("Iva 27: ", textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                    });

                    table2.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + resumen.Iva27), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                    });
                    table2.AddCell(emptyCell);
                    table2.AddCell(emptyCell);
                }

            }


            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);

            var subTotal = resumen.Total - resumen.IvaTotal;
            if (mostrarIvaTotal != true)
            {
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(new PdfPCell(new Phrase("IvaTotal: ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                table2.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + resumen.IvaTotal.ToString()), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,

                });

                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(new PdfPCell(new Phrase("SubTotal: ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase("$" + subTotal.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Total:", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + resumen.Total.ToString()), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
            }


            table2.AddCell(emptyCell);



            if (mostrarIvaTotal2 != true)
            {
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Total:", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + resumen.Total.ToString()), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
            }

            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);

            paragraphTotal.Add(table2);
            paragraph.Add(paragraphDetalle);
            paragraph.Add(paragraphTotal);

            return paragraph;

        }

        public async Task<Paragraph> DetalleRecibo(DtoRequestDetallePDF model)
        {
            PdfPCell emptyCell = new PdfPCell()
            {
                Border = PdfPCell.NO_BORDER
            };

            Paragraph paragraph = new Paragraph();
            PdfPTable table = new PdfPTable(5);

            //Le agrego color a la letra de la tabla y tamaño
            BaseColor black = BaseColor.Black;
            Font font = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD, black);

            var resumen = model;

            //Recibo
            if (resumen.QuittanceDetails != null && resumen.QuittanceDetails.Any())
            {
                // Establecer el ancho de las columnas
                float[] columnWidths = { 3f, 3f, 3f, 2f, 3f }; // Ancho entre columnas
                table.SetWidths(columnWidths);


                table.AddCell(new PdfPCell(new Phrase("Suma Recibida", font))
                {
                    Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });

                table.AddCell(new PdfPCell(new Phrase("Banco", font))
                {
                    Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });

                table.AddCell(new PdfPCell(new Phrase("En Concepto de ", font))
                {
                    Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });

                table.AddCell(new PdfPCell(new Phrase("Cheque", font))
                {
                    Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });

                table.AddCell(new PdfPCell(new Phrase("Total: ", font))
                {
                    Border = PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });

                foreach (var item in resumen.QuittanceDetails)
                {
                    Chunk cashChunk = new Chunk(model.Cash.ToString(), textFont);
                    table.AddCell(new PdfPCell(new Phrase("$" + cashChunk))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.RIGHT_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.Bank, textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.RIGHT_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(model.Concept, textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = PdfPCell.RIGHT_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase("N°" + item.CheckNumber, textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.RIGHT_BORDER,
                        PaddingTop = 10f
                    });


                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + model.Total2), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.LEFT_BORDER,
                        PaddingTop = 10f
                    });
                }
                paragraph.Add(table);
            }

            //------------------------------------------------------
            //------------------------------------------------------

            //Tabla de Productos Recibo
            if (resumen.QuittanceProductDetails != null && resumen.QuittanceProductDetails.Any())
            {
                // Establecer el ancho de las columnas
                float[] columnWidthsProduct = { 2f, 6f, 3f, 1f, 2f }; // Ancho entre columnas
                table.SetWidths(columnWidthsProduct);

                //Le agrego color a la letra de la tabla y tamaño           

                table.AddCell(new PdfPCell(new Phrase("Cantidad", font))
                {
                    Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });

                PdfPCell productoCell = new PdfPCell(new Phrase("Producto", font))
                {
                    Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                };

                table.AddCell(productoCell);
                table.AddCell(new PdfPCell(new Phrase("Precio Unitario", font))
                {
                    Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });


                table.AddCell(new PdfPCell(new Phrase("Iva", font))
                {
                    Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });

                table.AddCell(new PdfPCell(new Phrase("Importe", font))
                {
                    Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    PaddingBottom = 10f,
                    PaddingTop = 5f
                });

                foreach (var item in resumen.QuittanceProductDetails)
                {

                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.ProductName, textFont))
                    {
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", item.Price), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase((item.Iva % 1 == 0) ? ((int)item.Iva).ToString() : item.Iva.ToString(CultureInfo.InvariantCulture), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", (item.Price * item.Quantity)), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });
                }

                paragraph.Add(table);

                Paragraph paragraphTotal = new Paragraph();

                PdfPTable tableTotal = new PdfPTable(4);

                float[] columnWidths2 = { 1f, 2f, 1f, 1f }; // Ancho relativo de cada columna
                tableTotal.SetWidths(columnWidths2);
                tableTotal.SpacingBefore = 25f;
                tableTotal.SpacingAfter = 25f;


                tableTotal.AddCell(emptyCell);
                tableTotal.AddCell(emptyCell);

                tableTotal.AddCell(emptyCell);
                tableTotal.AddCell(emptyCell);

                tableTotal.AddCell(emptyCell);
                tableTotal.AddCell(emptyCell);
                tableTotal.AddCell(new PdfPCell(new Phrase("Total:"))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER
                });
                tableTotal.AddCell(new PdfPCell(new Phrase(string.Format(" ${0:0.00}", resumen.Total2)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                });

                paragraphTotal.Add(tableTotal);

                if (!string.IsNullOrEmpty(model.Concept))
                {

                    PdfPTable tableConcept = new PdfPTable(2);
                    float[] columnWidthsConcepts = { 2f, 6f }; // Ancho relativo de cada columna
                    tableConcept.SetWidths(columnWidthsConcepts);

                    tableConcept.SpacingAfter = 25f;
                    tableConcept.SpacingBefore = 25f;

                    tableConcept.AddCell(new PdfPCell(new Phrase("En Concepto de ", font))
                    {
                        Border = PdfCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        PaddingBottom = 10f,
                        PaddingTop = 5f
                    });

                    tableConcept.AddCell(new PdfPCell(new Phrase(model.Concept, textFont))
                    {
                        Border = PdfCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        PaddingTop = 7f
                    });

                    paragraphTotal.Add(tableConcept);
                }

                paragraph.Add(paragraphTotal);

            }
            //------------------------------------------------------
            //------------------------------------------------------

            return paragraph;

        }

        public async Task<Paragraph> DetalleComprobanteCompra(DtoRequestDetallePDF model)
        {

            Paragraph paragraph = new Paragraph();
            PdfPTable table = new PdfPTable(5);

            // Establecer el ancho de las columnas
            float[] columnWidths = { 2f, 6f, 3f, 1f, 3f }; // Ancho entre columnas
            table.SetWidths(columnWidths);

            //Le agrego color a la letra de la tabla y tamaño
            BaseColor black = BaseColor.Black;
            Font font = FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD, black);


            table.AddCell(new PdfPCell(new Phrase("Cantidad", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });

            PdfPCell productoCell = new PdfPCell(new Phrase("Producto", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            };

            table.AddCell(productoCell);
            table.AddCell(new PdfPCell(new Phrase("Precio Unitario", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });


            table.AddCell(new PdfPCell(new Phrase("Iva", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });

            table.AddCell(new PdfPCell(new Phrase("Importe", font))
            {
                Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });


            var resumen = model;

            if (resumen.ReceiptDetails != null)
            {
                foreach (var item in resumen.ReceiptDetails)
                {

                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.ProductName, textFont))
                    {
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });


                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", item.Price), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });

                    if (item.Iva != null)
                    {
                        table.AddCell(new PdfPCell(new Phrase((item.Iva % 1 == 0) ? ((int)item.Iva).ToString() : item.Iva.ToString(CultureInfo.InvariantCulture), textFont))
                        {
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            Border = PdfPCell.NO_BORDER,
                            PaddingTop = 10f
                        });
                    }

                    table.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", (item.Price * item.Quantity)), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER,
                        PaddingTop = 10f
                    });
                }

            }

            Tabla(resumen);

            paragraph.Add(table);

            Paragraph saltoDeLinea1 = new Paragraph("");
            paragraph.Add(saltoDeLinea1);

            paragraph.Add(Tabla(resumen));

            return paragraph;

        }

        public Paragraph Tabla(DtoRequestDetallePDF model)
        {

            Paragraph paragraph = new Paragraph();


            PdfPTable table2 = new PdfPTable(6);
            float[] columnWidths2 = { 1f, 1f, 1f, 1f, 1f, 1f }; // Ancho relativo de cada columna
            table2.SetWidths(columnWidths2);


            PdfPCell emptyCell = new PdfPCell()
            {
                Border = PdfPCell.NO_BORDER
            };

            var resumen = model;

            var subTotal = resumen.Total - resumen.IvaTotal;

            if (mostrarIvaA == true)
            {
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(new PdfPCell(new Phrase("Conc. No Gravado ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase("$" + resumen.ConcNoGravado.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Perc. Iva ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase("$" + resumen.PercIva.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Perc. Ing. Bruto ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase("$" + resumen.PercIngBrutos.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Iva Total: ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,

                });
                table2.AddCell(new PdfPCell(new Phrase("$" + resumen.IvaTotal.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });


                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("SubTotal: ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase("$" + subTotal.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });


                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Total:", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + resumen.Total.ToString()), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

            }
            else
            {
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(new PdfPCell(new Phrase("Conc. No Gravado ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase("" + resumen.ConcNoGravado.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Perc. Iva ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase("" + resumen.PercIva.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Perc. Ing. Bruto ", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase("" + resumen.PercIngBrutos.ToString(), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });


                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);
                table2.AddCell(emptyCell);

                table2.AddCell(new PdfPCell(new Phrase("Total:", textFont))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Border = PdfCell.NO_BORDER,
                });
                table2.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + resumen.Total.ToString()), textFont))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
            }


            paragraph.Add(table2);

            return paragraph;
        }

        public async Task<Paragraph> DetallePresupuestoYRemito(DtoRequestDetallePDF model)
        {

            Paragraph paragraph = new Paragraph();
            PdfPTable table = new PdfPTable(3);

            // Establecer el ancho de las columnas
            float[] columnWidths = { 6f, 2f, 3f }; // Ancho entre columnas
            table.SetWidths(columnWidths);

            //Le agrego color a la letra de la tabla y tamaño
            BaseColor black = BaseColor.Black;
            Font font = FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, black);
            Font textFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

            PdfPCell productoCell = new PdfPCell(new Phrase("Producto", font))
            {
                Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER | PdfCell.LEFT_BORDER | PdfCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            };

            table.AddCell(productoCell);
            table.AddCell(new PdfPCell(new Phrase("Cantidad", font))
            {
                Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER | PdfCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });
            table.AddCell(new PdfPCell(new Phrase("Precio Unidad", font))
            {
                Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER | PdfCell.TOP_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingBottom = 10f,
                PaddingTop = 5f
            });



            var resumen = model;
            //Presupuesto
            if (resumen.BudgetDetails != null)
            {
                foreach (var item in resumen.BudgetDetails)
                {

                    table.AddCell(new PdfPCell(new Phrase(item.ProductName, textFont))
                    {
                        Border = PdfPCell.RIGHT_BORDER | PdfCell.BOTTOM_BORDER | PdfCell.LEFT_BORDER,
                        BorderColor = BaseColor.Gray,
                        Padding = 5f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.RIGHT_BORDER | PdfCell.BOTTOM_BORDER,
                        BorderColor = BaseColor.Gray,
                        Padding = 5f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.Price.ToString("N2", CultureInfo.InvariantCulture), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.RIGHT_BORDER | PdfCell.BOTTOM_BORDER,
                        BorderColor = BaseColor.Gray,
                        Padding = 5f
                    });

                }
            }
            //var subTotal = resumen.Total * resumen.Quantity;

            //Remito
            if (resumen.DeliveryNotesDetails != null && resumen.DeliveryNotesDetails.Any())
            {
                foreach (var item in resumen.DeliveryNotesDetails)
                {

                    table.AddCell(new PdfPCell(new Phrase(item.ProductName, textFont))
                    {
                        Border = PdfPCell.RIGHT_BORDER | PdfCell.BOTTOM_BORDER | PdfCell.LEFT_BORDER,
                        BorderColor = BaseColor.Gray,
                        Padding = 5f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.RIGHT_BORDER | PdfCell.BOTTOM_BORDER,
                        BorderColor = BaseColor.Gray,
                        Padding = 5f
                    });

                    table.AddCell(new PdfPCell(new Phrase(item.Price.ToString("N2", CultureInfo.InvariantCulture), textFont))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.RIGHT_BORDER | PdfCell.BOTTOM_BORDER,
                        BorderColor = BaseColor.Gray,
                        Padding = 5f
                    });
                }
            }


            table.AddCell(paragraph);


            PdfPCell emptyCell = new PdfPCell()
            {
                Border = PdfPCell.NO_BORDER
            };

            Paragraph paragraphDetalle = new Paragraph();
            paragraphDetalle.Add(table);

            // Agregar espacio vertical entre las tablas
            Paragraph paragraphSaltoDeLinea = new Paragraph();
            paragraphSaltoDeLinea.Add(new Paragraph(" "));

            Paragraph paragraphTotal = new Paragraph();

            PdfPTable table2 = new PdfPTable(4);


            float[] columnWidths2 = { 1f, 2f, 1f, 1f }; // Ancho relativo de cada columna
            table2.SetWidths(columnWidths2);



            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);

            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);

            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);
            table2.AddCell(new PdfPCell(new Phrase("Total:", font))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = PdfPCell.LEFT_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                Padding = 5f
            });
            table2.AddCell(new PdfPCell(new Phrase(resumen.Total.ToString("N2", CultureInfo.InvariantCulture), font))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = PdfPCell.RIGHT_BORDER | PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER,
                Padding = 5f
            });
            table2.AddCell(emptyCell);
            table2.AddCell(emptyCell);

            paragraphTotal.Add(table2);
            paragraph.Add(paragraphDetalle);
            paragraph.Add(paragraphTotal);

            return paragraph;

        }

        public async Task<Paragraph> Observacion(string observacion)
        {
            BaseColor black = BaseColor.Black;
            Font fontText = FontFactory.GetFont(FontFactory.HELVETICA, 8);

            Paragraph paragraph = new Paragraph();

            if (!string.IsNullOrEmpty(observacion))
            {
                PdfPTable table = new PdfPTable(1);

                Phrase textoIzquierda = new()
                {
                    new Chunk("Observación: "+ observacion, fontText),
                    Chunk.Newline,
                };


                // Primera columna: paragraph
                PdfPCell cell1 = new PdfPCell(textoIzquierda)
                {
                    Border = PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER,
                    PaddingTop = 10f,
                    PaddingBottom = 10f,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                table.AddCell(cell1);
                paragraph.Add(table);
            }

            return paragraph;
        }

        public Paragraph CabeceraArca(DtoRequestInvoice invoice)
        {
            BaseColor black = BaseColor.Black;
            Font fontTitle = FontFactory.GetFont(FontFactory.HELVETICA, 12, Font.BOLD, black);
            Font fontText = FontFactory.GetFont(FontFactory.HELVETICA, 8);
            Font fontTextBold = FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD, black);
            Paragraph paragraph = new Paragraph();

            string titulo = _configuration.GetSection("Pdf:Name").Value;
            string dni = _configuration.GetSection("Pdf:Cuit").Value;
            string direccion = _configuration.GetSection("Pdf:Direccion").Value;
            string nombre_apellido = _configuration.GetSection("Pdf:Nombre").Value;
            string email = _configuration.GetSection("Pdf:Email").Value;

            string imagePath = Path.Combine(_Env.ContentRootPath, "Assets", "dantesLogo1.png");

            #region PRIMERA-CABECERA-LOGO

            PdfPTable tablaCabecera = new PdfPTable(3);

            Phrase columnaUno = new();
            columnaUno.Add(new Chunk(titulo, fontTitle));
            PdfPCell celdaCabeceraIzquierda = new PdfPCell(columnaUno) { VerticalAlignment = Element.ALIGN_CENTER, Border = PdfPCell.NO_BORDER };

            Phrase columnaDos = new();
            columnaDos.Add(new Chunk(MapInvoiceType(invoice.Type), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.Black)));
            PdfPCell celdaCabeceraCentro = new PdfPCell(columnaDos) { VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER, Border = PdfPCell.NO_BORDER };

            tablaCabecera.AddCell(celdaCabeceraIzquierda);
            tablaCabecera.AddCell(celdaCabeceraCentro);
            iTextSharp.text.Image image2 = iTextSharp.text.Image.GetInstance(imagePath);
            image2.ScaleToFit(40f, 40f);
            PdfPCell celdaCabeceraDerecha = new PdfPCell(image2) { VerticalAlignment = Element.ALIGN_CENTER, HorizontalAlignment = Element.ALIGN_CENTER, Border = PdfPCell.NO_BORDER };

            tablaCabecera.AddCell(celdaCabeceraDerecha);
            float[] columnWidthsCabecera = { 4f, 1f, 4f };
            tablaCabecera.SetWidths(columnWidthsCabecera);

            #endregion

            #region SEGUNDA-CABECERA

            PdfPTable tablaEncabezado = new PdfPTable(2);

            #region IZQUIERDA

            Phrase phraseIzq = new Phrase(12f);

            Chunk razonSocial = new(titulo, fontText);
            phraseIzq.Add(new Chunk("Razón Social: ", fontTextBold));
            phraseIzq.Add(new Chunk(razonSocial));
            phraseIzq.Add(Chunk.Newline);
            phraseIzq.Add(new Chunk("Domicilio Comercial: ", fontTextBold));
            phraseIzq.Add(new Chunk(direccion, fontText));
            phraseIzq.Add(Chunk.Newline);
            phraseIzq.Add(new Chunk("Condición frente al IVA: ", fontTextBold));
            phraseIzq.Add(new Chunk("IVA Responsable Inscripto", fontText));
            phraseIzq.Add(Chunk.Newline);

            PdfPCell celdaIzquierda = new PdfPCell(phraseIzq)
            {
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 10f,
                VerticalAlignment = Element.ALIGN_LEFT,
                PaddingBottom = 10f
            };

            tablaEncabezado.AddCell(celdaIzquierda);
            #endregion

            #region DERECHA

            Phrase phraseDrh = new(5f);

            phraseDrh.Add(new Chunk("Punto de Venta: ", fontTextBold));
            phraseDrh.Add(new Chunk(CustomizationConstant.PuntoDeVenta.ToString().PadLeft(3, '0'), fontText));
            phraseDrh.Add(Chunk.Newline);
            phraseDrh.Add(new Chunk("Comp. N°: ", fontTextBold));
            phraseDrh.Add(new Chunk(invoice.InvoiceNumber.ToString(), fontText));
            phraseDrh.Add(Chunk.Newline);
            phraseDrh.Add(new Chunk("Fecha de Emisión: ", fontTextBold));
            phraseDrh.Add(new Chunk(invoice.DateTime.ToString("dd/MM/yyyy"), fontText));
            phraseDrh.Add(Chunk.Newline);
            phraseDrh.Add(new Chunk("CUIT: ", fontTextBold));
            phraseDrh.Add(new Chunk(dni, fontText));
            phraseDrh.Add(Chunk.Newline);

            PdfPCell celdaDerecha = new PdfPCell(phraseDrh)
            {
                Border = PdfPCell.NO_BORDER,
                PaddingTop = 10f,
                VerticalAlignment = Element.ALIGN_LEFT,
                PaddingBottom = 10f
            };

            tablaEncabezado.AddCell(celdaDerecha);
            #endregion

            float[] columnWidths = { 6f, 2f };
            tablaEncabezado.SetWidths(columnWidths);

            #endregion           

            paragraph.Add(tablaCabecera);
            paragraph.Add(tablaEncabezado);

            #region CABECERA-CLIENTE

            PdfPTable tablaCliente = new PdfPTable(2);


            // Mover la declaración fuera del bucle
            Phrase textoIzquierda = new()
            {
                new Chunk("Cliente: ", fontTextBold),
                new Chunk(invoice.CustomerName.ToUpper().Trim(), fontText),
                Chunk.Newline,
                Chunk.Newline,
                new Chunk("Dirección: ", fontTextBold),
                new Chunk(invoice.CustomerAddress, fontText),
                Chunk.Newline,
            };

            // Primera columna: paragraph
            PdfPCell cell1 = new PdfPCell(textoIzquierda)
            {
                Border = PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 10f
            };

            //Segunda Columna
            Phrase textoDerecha = new();
            if (string.IsNullOrEmpty(invoice.CustomerCuit))
            {
                textoDerecha = new() { new Chunk("CUITs: ", fontTextBold) };
            }
            else
            {
                textoDerecha = new()
                {
                new Chunk("CUIT: ", fontTextBold),
                new Chunk(invoice.CustomerCuit, fontText),
                Chunk.Newline,
                Chunk.Newline,
                new Chunk("Condicion: ", fontTextBold),
                new Chunk(MapCondicion(invoice.Type), fontText)
                };
            }

            PdfPCell cell2 = new PdfPCell(textoDerecha)
            {
                Border = PdfPCell.TOP_BORDER | PdfPCell.BOTTOM_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Padding = 10f
            };

            tablaCliente.AddCell(cell1);
            tablaCliente.AddCell(cell2);
            paragraph.Add(tablaCliente);

            #endregion


            return paragraph;
        }


        #region Private

        private static string MapInvoiceType(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A or ETypeReceipt.ResponsableMonotrinuto => "A",
                ETypeReceipt.B => "B",
                _ => "B"
            };
        }

        private static string MapCondicion(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A => "Responsable Inscripto",
                ETypeReceipt.ResponsableMonotrinuto => "Responsable Monotributo",
                ETypeReceipt.B => "Consumidor final",
                ETypeReceipt.EXENTO => "Excento",
                _ => ""
            };
        }

        private PdfPTable CrearTablaDetalle()
        {
            PdfPTable table = new PdfPTable(5);
            table.SetWidths(new float[] { 2f, 6f, 3f, 1f, 2f });

            var font = FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.BOLD, BaseColor.Black);

            table.AddCell(new PdfPCell(new Phrase("Cantidad", font)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER, PaddingBottom = 10f, PaddingTop = 5f });
            table.AddCell(new PdfPCell(new Phrase("Producto", font)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER, PaddingBottom = 10f, PaddingTop = 5f });
            table.AddCell(new PdfPCell(new Phrase("Precio Unitario", font)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER, PaddingBottom = 10f, PaddingTop = 5f });
            table.AddCell(new PdfPCell(new Phrase("IVA", font)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER, PaddingBottom = 10f, PaddingTop = 5f });
            table.AddCell(new PdfPCell(new Phrase("Importe", font)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = PdfPCell.BOTTOM_BORDER | PdfPCell.TOP_BORDER, PaddingBottom = 10f, PaddingTop = 5f });

            return table;
        }

        private void AgregarFilaDetalle(PdfPTable table, DtoResponseInvoiceDetail item)
        {
            var textFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);

            table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), textFont)) { HorizontalAlignment = Element.ALIGN_CENTER, Border = PdfPCell.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase(item.ProductName, textFont)) { Border = PdfPCell.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase(item.Price.ToString("F2"), textFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = PdfPCell.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase(MapInvoiceIvaToString(item.Iva), textFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = PdfPCell.NO_BORDER });
            table.AddCell(new PdfPCell(new Phrase((item.Price * item.Quantity).ToString("F2"), textFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Border = PdfPCell.NO_BORDER });
        }

        private PdfPTable CrearTablaTotales(DtoRequestInvoice invoice)
        {
            Font fontTextBoldIvas = FontFactory.GetFont(FontFactory.HELVETICA, 9, Font.BOLD, BaseColor.Black);
            #region Total con IVA

            PdfPTable totalIva = new PdfPTable(4);
            totalIva.SpacingBefore = 30f;
            PdfPCell emptyCell = new PdfPCell()
            {
                Border = PdfPCell.NO_BORDER
            };

            if ((ETypeReceipt)invoice.Type == ETypeReceipt.A || (ETypeReceipt)invoice.Type == ETypeReceipt.ResponsableMonotrinuto)
            {
                if (invoice.Iva10 != 0)
                {
                    totalIva.AddCell(emptyCell);
                    totalIva.AddCell(emptyCell);
                    totalIva.AddCell(new PdfPCell(new Phrase("Iva 10: ", fontTextBoldIvas))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfCell.NO_BORDER
                    });

                    //table2.AddCell(emptyCell);
                    totalIva.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + Math.Round(invoice.Iva10.Value, 2)), fontTextBoldIvas))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfCell.NO_BORDER
                    });
                }

                if (invoice.Iva21 != 0)
                {
                    totalIva.AddCell(emptyCell);
                    totalIva.AddCell(emptyCell);
                    totalIva.AddCell(new PdfPCell(new Phrase("Iva 21: ", fontTextBoldIvas))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfCell.NO_BORDER
                    });


                    totalIva.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + Math.Round(invoice.Iva21.Value, 2)), fontTextBoldIvas))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfCell.NO_BORDER
                    });

                }

                if (invoice.Iva27 != 0)
                {

                    totalIva.AddCell(emptyCell);
                    totalIva.AddCell(emptyCell);

                    totalIva.AddCell(new PdfPCell(new Phrase("Iva 27: ", fontTextBoldIvas))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfCell.NO_BORDER
                    });

                    totalIva.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + Math.Round(invoice.Iva27.Value, 2)), fontTextBoldIvas))
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfCell.NO_BORDER
                    });
                }

                totalIva.CompleteRow();

                totalIva.AddCell(emptyCell);
                totalIva.AddCell(emptyCell);
                totalIva.AddCell(emptyCell);
                totalIva.AddCell(emptyCell);

                var subTotal = invoice.Total - invoice.IvaTotal;

                totalIva.AddCell(emptyCell);
                totalIva.AddCell(emptyCell);
                totalIva.AddCell(new PdfPCell(new Phrase("IvaTotal: ", fontTextBoldIvas))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                totalIva.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + invoice.IvaTotal.ToString()), fontTextBoldIvas))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,

                });

                totalIva.AddCell(emptyCell);
                totalIva.AddCell(emptyCell);
                totalIva.AddCell(new PdfPCell(new Phrase("SubTotal: ", fontTextBoldIvas))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
                totalIva.AddCell(new PdfPCell(new Phrase("$" + subTotal.ToString(), fontTextBoldIvas))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

                totalIva.AddCell(emptyCell);
                totalIva.AddCell(emptyCell);

                totalIva.AddCell(new PdfPCell(new Phrase("Total:", fontTextBoldIvas))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
                totalIva.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + invoice.Total.ToString()), fontTextBoldIvas))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });

            }

            #endregion

            #region Total sin IVA

            if ((ETypeReceipt)invoice.Type == ETypeReceipt.B || (ETypeReceipt)invoice.Type == ETypeReceipt.EXENTO)
            {
                totalIva.AddCell(emptyCell);
                totalIva.AddCell(emptyCell);

                totalIva.AddCell(new PdfPCell(new Phrase("Total:", fontTextBoldIvas))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
                totalIva.AddCell(new PdfPCell(new Phrase(string.Format("{0,7:##.00}", "$" + invoice.Total.ToString()), fontTextBoldIvas))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Border = PdfCell.NO_BORDER,
                });
            }

            #endregion
            return totalIva;
        }

        private static string MapInvoiceIvaToString(decimal iva)
        {
            return (decimal)iva switch
            {
                10.5m => "10.5",
                21 => "21",
                27 => "27",
                _ => ""
            };
        }
        #endregion

    }
}
