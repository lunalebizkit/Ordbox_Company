using AutoMapper;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.Services.ARCA.Enum;
using Ordbox.Services.Models.Dtos.DtoRequest;

namespace Ordbox.Services.Mapper
{
    public class PDFMapperProfile : Profile
    {
        public PDFMapperProfile()
        {
            CreateMap<Invoice, DtoRequestCabeceraPrintPDF>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.InvoiceNumber))
                .ForMember(dest => dest.RelatedNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.CustomerAddress))
                .ForMember(dest => dest.CustomerCuit, opt => opt.MapFrom(src => src.CustomerCuit))
                .ForMember(dest => dest.Observacion, opt => opt.MapFrom(src => src.Observation))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.CAEExpirationDate, opt => opt.MapFrom(src => src.CAEExpirationDate))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => MapInvoiceType(src.Type)))
                .ForMember(dest => dest.ArcaType, opt => opt.MapFrom(src => MapARCAInvoiceDocumentType(src.Type)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.IvaTotal, opt => opt.MapFrom(src => src.IvaTotal))
                .ForMember(dest => dest.CAE, opt => opt.MapFrom(src => src.CAE))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.InvoiceDetails));

            CreateMap<CreditMemo, DtoRequestCabeceraPrintPDF>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.CreditMemoNumber))
                .ForMember(dest => dest.RelatedNumber, opt => opt.MapFrom(src => src.InvoiceNumber))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.CustomerAddress))
                .ForMember(dest => dest.CustomerCuit, opt => opt.MapFrom(src => src.CustomerCuit))
                .ForMember(dest => dest.Observacion, opt => opt.MapFrom(src => src.Observation))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.CAEExpirationDate, opt => opt.MapFrom(src => src.CAEExpirationDate))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => MapCreditDocumentType(src.Type)))
                .ForMember(dest => dest.ArcaType, opt => opt.MapFrom(src => MapARCACreditDocumentType(src.Type)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.IvaTotal, opt => opt.MapFrom(src => src.IvaTotal))
                .ForMember(dest => dest.CAE, opt => opt.MapFrom(src => src.CAE))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.CreditMemoDetail));

            CreateMap<DebitMemo, DtoRequestCabeceraPrintPDF>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.DebitMemoNumber))
                .ForMember(dest => dest.RelatedNumber, opt => opt.MapFrom(src => src.InvoiceNumber))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.CustomerAddress))
                .ForMember(dest => dest.CustomerCuit, opt => opt.MapFrom(src => src.CustomerCuit))
                .ForMember(dest => dest.Observacion, opt => opt.MapFrom(src => src.Observation))
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.CAEExpirationDate, opt => opt.MapFrom(src => src.CAEExpirationDate))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => MapDebitDocumentType(src.Type)))
                .ForMember(dest => dest.ArcaType, opt => opt.MapFrom(src => MapARCADebitDocumentType(src.Type)))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
                .ForMember(dest => dest.IvaTotal, opt => opt.MapFrom(src => src.IvaTotal))
                .ForMember(dest => dest.CAE, opt => opt.MapFrom(src => src.CAE))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.DebitMemoDetails));

            CreateMap<InvoiceDetail, DtoRequestDetallePrintPDF>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Iva, opt => opt.MapFrom(src => src.Iva));

            CreateMap<CreditMemoDetail, DtoRequestDetallePrintPDF>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Iva, opt => opt.MapFrom(src => src.Iva));

            CreateMap<DebitMemoDetails, DtoRequestDetallePrintPDF>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Iva, opt => opt.MapFrom(src => src.Iva));
        }

        #region Private Methods
        private static string MapInvoiceType(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A or ETypeReceipt.ResponsableMonotrinuto => "Factura A",
                ETypeReceipt.B => "Factura B",
                _ => "Factura"
            };
        }

        private static string MapCreditDocumentType(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A or ETypeReceipt.ResponsableMonotrinuto => "Nota Credito A",
                ETypeReceipt.EXENTO or ETypeReceipt.B => "Nota Credito B",
                _ => "Nota Credito",
            };
        }

        private static string MapDebitDocumentType(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A or ETypeReceipt.ResponsableMonotrinuto => "Nota Debito A",
                ETypeReceipt.EXENTO or ETypeReceipt.B => "Nota Debito B",
                _ => "Nota Debito",
            };
        }

        private static int MapARCACreditDocumentType(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A or ETypeReceipt.ResponsableMonotrinuto => (int)EInvoiceType.NotaCreditoA,
                ETypeReceipt.EXENTO or ETypeReceipt.B => (int)EInvoiceType.NotaCreditoB,
                _ => invoiceType,
            };
        }

        private static int MapARCAInvoiceDocumentType(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A or ETypeReceipt.ResponsableMonotrinuto => (int)EInvoiceType.FacturaA,
                ETypeReceipt.EXENTO or ETypeReceipt.B => (int)EInvoiceType.FacturaB,
                _ => invoiceType,
            };
        }

        private static int MapARCADebitDocumentType(int invoiceType)
        {
            return (ETypeReceipt)invoiceType switch
            {
                ETypeReceipt.A or ETypeReceipt.ResponsableMonotrinuto => (int)EInvoiceType.NotaDebitoA,
                ETypeReceipt.EXENTO or ETypeReceipt.B => (int)EInvoiceType.NotaDebitoB,
                _ => invoiceType,
            };
        }
        #endregion
    }
}
