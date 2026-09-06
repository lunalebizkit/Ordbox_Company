using Ordbox.Domain.Enum;
using Ordbox.SDK.Error;
using Ordbox.Services.ImpresoraFiscal.Printer250F;
using Ordbox.Services.ImpresoraFiscal.Printer250F.Dto;
using Ordbox.Services.Models.Dtos.DtoResponse;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace Ordbox.Services.ImpresoraFiscal.PrinterF250F

{
    public class PrinterF250F : IPrinter
    {
        private readonly PrinterConfig _config;
        private readonly ErrorManager _logger;

        private string ConsultarEstadoEsperaString = JsonConvert.SerializeObject(new { ConsultarEstadoEspera = new { } });
        public PrinterF250F(PrinterConfig config, ErrorManager logger)
        {
            _config = config;
            _logger = logger;
        }


        private async Task<T> RunCommand<T>(object request,bool reintentar = true, int retryWait = 1500)
        {
            bool success = false;
            string responseBody = string.Empty;
            byte getStatusRetry = 0;
            byte getStatusMaxRetry = 10;
            try
            {
                var data = JsonConvert.SerializeObject(request);

                var requestTypeName = request.GetType().Name;

                _logger.LogRequestAndResponseInfo($"-----------request de Impresora---------{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
                _logger.LogRequestAndResponseInfo(data);

                using HttpClient client = new();

                var requestPrinter = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_config.Ip),
                    Content = new StringContent(data, Encoding.UTF8, "application/json"),
                };

                var response = await client.SendAsync(requestPrinter).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                _logger.LogRequestAndResponseInfo("-----------Response de Impresora---------");
                _logger.LogRequestAndResponseInfo(responseBody);

                var responseDeserialized = JsonConvert.DeserializeObject<T>(responseBody);

                if (responseDeserialized != null)
                {
                    using JsonDocument document = JsonDocument.Parse(responseBody);

                    if (document.RootElement.TryGetProperty("ControladorOcupado", out JsonElement controladorOcupado))
                    {
                        using JsonDocument requestType = JsonDocument.Parse(data);

                        if (requestType.RootElement.TryGetProperty(requestTypeName, out JsonElement abrirDocument))
                        {
                            do
                            {
                                responseBody = await GetStatusForPrintWaiting();

                                var responseBodyConverted = JsonConvert.DeserializeObject<T>(responseBody);

                                if (responseBodyConverted != null)
                                {
                                    using JsonDocument statusResponse = JsonDocument.Parse(responseBody);

                                    if (statusResponse.RootElement.TryGetProperty(requestTypeName, out JsonElement abrirDocumento))
                                    {
                                        switch ((string)requestTypeName)
                                        {
                                            case "ObtenerPrimerBloqueReporteElectronico":
                                            case "ObtenerSiguienteBloqueReporteElectronico":
                                                var bloqueElectronicoResponse = JsonConvert.DeserializeObject<DtoResponseObtenerReporteElectronico>(responseBody);
                                                var body = bloqueElectronicoResponse?.BloqueElectronico;

                                                if (body != null && !string.IsNullOrEmpty(body?.Informacion))
                                                {
                                                    success = true;
                                                    break;
                                                }
                                                continue;

                                            case "AbrirDocumento":
                                                if (abrirDocumento.TryGetProperty("NumeroComprobante", out JsonElement numeroComprobante))
                                                {
                                                    success = true;
                                                    break;
                                                }
                                                continue;

                                            default:
                                                continue;

                                        }

                                        success = true;                                            
                                    }
                                }

                                if (success)
                                {
                                    break;
                                }

                                getStatusRetry++;
                                Thread.Sleep(retryWait);
                            } while (!success && getStatusRetry < getStatusMaxRetry);
                        }
                    }            
                            
                }                
                
            return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorsMessages.GetMessage(ErrorsCodes.C_000_MENSAJE_INVALIDO), ex: ex);
                return default;
            }
        }

        public async Task<string> OpenInvoice(ETypeReceipt type, string documentClient, eTypeDocumentClient typeDocument, string address = "")
        {

            var typeDocumemt = type switch
            {
                ETypeReceipt.A => "TiqueFacturaA",
                ETypeReceipt.B => "TiqueFacturaB",
                ETypeReceipt.EXENTO => "TiqueFacturaB",
                _ => throw new NotImplementedException()
            };


            var result = await RunCommand<DtoResponseAbrirDoc>(new AbrirDocumento { AbrirDocumentoBody = new AbrirDocumentoBody { CodigoComprobante = typeDocumemt } },false,3000);

            if (result != null && result.Body != null)
            {
                foreach (var Item in result.Body.Estado.Fiscal)
                {
                    if (Item.ToString().Contains("Error"))
                    {
                        return null;
                    }
                }

            }
            else
            {
                return null;
            }

            return result.Body.NumeroComprobante;
        }

        public async Task SetHeader(string line1, string line2, string line3)
        {
            await SetZona(1, line1);
            await SetZona(2, line2);
            await SetZona(3, line3);
        }

        public async Task SetZona(int numeroLineas, string descripcion)
        {
            var result = await RunCommand<BaseEstado>(new ConfigurarZona
            {
                ConfigurarZonaBody = new ConfigurarZonaBody
                {
                    NumeroLinea = numeroLineas,
                    Descripcion = descripcion
                }
            });
        }

        public async Task<string> OpenND(ETypeReceipt type, string documentClient, eTypeDocumentClient typeDocument, string address = "")
        {

            var typeDocumemt = type switch
            {
                ETypeReceipt.A => "TiqueNotaDebitoA",
                ETypeReceipt.B => "TiqueNotaDebitoB",
                ETypeReceipt.EXENTO => "TiqueNotaDebitoB",
                _ => throw new NotImplementedException()
            };

            var result = await RunCommand<DtoResponseAbrirDoc>(new AbrirDocumento { AbrirDocumentoBody = new AbrirDocumentoBody { CodigoComprobante = typeDocumemt } });

            if (result != null && result.Body != null)
            {
                foreach (var Item in result.Body.Estado.Fiscal)
                {
                    if (Item.ToString().Contains("Error"))
                    {
                        return null;
                    }
                }

            }
            else
            {
                return null;
            }

            return result.Body.NumeroComprobante;
        }

        public async Task<string> OpenNC(ETypeReceipt type, string documentClient, eTypeDocumentClient typeDocument, string address = "")
        {


            var typeDocumemt = type switch
            {
                ETypeReceipt.A => "TiqueNotaCreditoA",
                ETypeReceipt.B => "TiqueNotaCreditoB",
                ETypeReceipt.EXENTO => "TiqueNotaCreditoB",
                _ => throw new NotImplementedException()
            };

            var result = await RunCommand<DtoResponseAbrirDoc>(new AbrirDocumento { AbrirDocumentoBody = new AbrirDocumentoBody { CodigoComprobante = typeDocumemt } });

            if (result != null && result.Body != null)
            {
                foreach (var Item in result.Body.Estado.Fiscal)
                {
                    if (Item.ToString().Contains("Error"))
                    {
                        return null;
                    }
                }

            }
            else
            {
                return null;
            }

            return result.Body.NumeroComprobante;
        }

        public async Task<string> CerrarJornadaFiscal()
        {
            var result = await RunCommand<DtoResponseReporteZ>(new CerrarJornadaFiscal { CerrarJornadaFiscalBody = new CerrarJornadaFiscalBody { Reporte = "ReporteZ" } });

            if (result != null && result.Body != null && result.Body.Estado != null)
            {
                if (result.Body.Estado?.Fiscal != null)
                {
                    foreach (var Item in result.Body?.Estado?.Fiscal)
                    {
                        if (Item.ToString().Contains("Error"))
                        {
                            await CloseFactura(1, "");
                            return null;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return result.Body.Estado?.Fiscal.ToString();

        }

        public async Task<string> PrintItem(string articulo, double cantidad, decimal monto, decimal iva = 21, string codigo = "")
        {

            var result = await RunCommand<DtoResponseImprimirItem>(new ImprimirItem
            {
                ImprimirItemBody = new ImprimirItemBody
                {

                    Descripcion = articulo,
                    Cantidad = cantidad,
                    PrecioUnitario = monto,
                    AlicuotaIVA = iva,
                    CodigoInterno = string.IsNullOrEmpty(codigo) ? "9999999" : codigo,
                }
            });
            if (result != null && result.Body != null)
            {
                foreach (var Item in result.Body.Estado.Fiscal)
                {
                    if (Item.ToString().Contains("Error"))
                    {
                        return null;
                    }
                }

            }
            else
            {
                return null;
            }

            return result.Body.IndiceAuditoria.ToString();
        }

        public async Task<string> CloseFactura(int copias = 1, string email = "", bool withRetry = false)
        {
            byte getStatusRetry = 0;
            byte getStatusMaxRetry = 10;
            DtoResponseCerrarDoc? result;
            do
            {
                result = await RunCommand<DtoResponseCerrarDoc>(new CerrarDocumento
                {
                    CerrarDocumentoBody = new CerrarDocumentoBody
                    {
                        Copias = copias,
                        DireccionEmail = email
                    }
                });

                withRetry = (result == null || (result != null && result.Body == null));

                if (result != null && result.Body != null)
                {
                    foreach (var Item in result.Body.Estado.Fiscal)
                    {
                        if (Item.ToString().Contains("Error"))
                        {
                            return null;
                        }
                    }
                withRetry = false;
                }
                    getStatusRetry++;
                if (withRetry)
                {
                    Thread.Sleep(1000);
                }
            }while(withRetry && getStatusRetry < getStatusMaxRetry);

            return (result.Body.NumeroComprobante != null) ? result.Body.NumeroComprobante : string.Empty;
        }

        public async Task<string> CargarDatosCliente(string customerName, string customerCuit, string customerAddress, ETypeReceipt tipoDocumento)
        {
            await SetHeader(_config.Line1, _config.Line2, _config.Line3);

            var typeDocumemt = tipoDocumento switch
            {
                ETypeReceipt.A => "ResponsableInscripto",
                ETypeReceipt.B => "ConsumidorFinal",
                ETypeReceipt.EXENTO => "ResponsableExento",
                _ => throw new NotImplementedException()
            };

            var typeIva = customerCuit.Length switch
            {
                11 => "TipoCUIT",
                8 => "TipoDNI",
                7 => "TipoDNI",
            };

            var result = await RunCommand<DtoResponseCargarDatosCliente>(new CargarDatosCliente
            {
                CargarDatosClienteBody = new CargarDatosClienteBody
                {
                    RazonSocial = customerName,
                    NumeroDocumento = customerCuit,
                    ResponsabilidadIVA = typeDocumemt,
                    TipoDocumento = typeIva,
                    Domicilio = string.IsNullOrWhiteSpace(customerAddress) ? "-" : customerAddress,
                }
            });

            foreach (var Item in result.Body.Estado.Fiscal)
            {
                if (Item.ToString().Contains("Error"))
                {
                    return null;
                }
            }

            return "Cliente Generado Correctamente";

        }

        public async Task<string> ReimprimirDocumento(int tipoDocumento, string numeroComprobante)
        {
            var result = await RunCommand<DtoResponseReimprimirDoc>(new CopiarComprobante
            {
                CopiarComprobanteBody = new CopiarComprobanteBody
                {
                    CodigoComprobante = tipoDocumento,
                    NumeroComprobante = numeroComprobante
                }
            });
            if (result != null && result.Body != null)
            {
                foreach (var Item in result.Body.Estado.Fiscal)
                {
                    if (Item.ToString().Contains("Error"))
                    {
                        await CloseFactura(1, "");
                        return null;
                    }
                }

            }
            else
            {
                return null;
            }

            return "Comprobante Reimpreso Correctamente";
        }

        public async Task<DtoResponseConsultarVersion> ConsultarVersionImpresora(ConsultarVersion consultarVersion)
        {
            return await RunCommand<DtoResponseConsultarVersion>(
                new ConsultarVersion() { });
        }
        
        public async Task<DtoResponseObtenerReporteElectronico> DownloadPrintReport(ObtenerPrimerBloqueReporteElectronico obtenerPrimerBloque)
        {
            return await RunCommand<DtoResponseObtenerReporteElectronico>(
               obtenerPrimerBloque, true, 6000);
        }
        
        public async Task<DtoResponseObtenerSiguienteBloqueReporteElectronico> DownloadNextBloquePrintReport(DtoObtenerSiguienteBloqueReporteElectronico obtenerSiguienteBloque)
        {
            return await RunCommand<DtoResponseObtenerSiguienteBloqueReporteElectronico>(
               obtenerSiguienteBloque, true, 6000);
        }

        #region Private
        private async Task<string> GetStatusForPrintWaiting()
        {
            using HttpClient client = new();

            var getStatus = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_config.Ip),
                Content = new StringContent(ConsultarEstadoEsperaString, Encoding.UTF8, "application/json"),
            };

            var response = await client.SendAsync(getStatus).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return responseBody;
            
        }
        #endregion
    }
}