namespace Ordbox.Services.ARCA.Dto.Response
{
    public class FEParamGetTiposDocResponseDto : DtoResponseErrorBase
    {
        public List<DtoResponseDocumentType> DocumentTypes { get; set; } = new();        
    }
}
