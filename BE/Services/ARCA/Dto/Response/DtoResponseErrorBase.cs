namespace Ordbox.Services.ARCA.Dto.Response
{
    public class DtoResponseErrorBase
    {
        public List<DtoResponseError> Errors { get; set; } = new();
        public List<DtoResponseError> Events { get; set; } = new();
    }
}
