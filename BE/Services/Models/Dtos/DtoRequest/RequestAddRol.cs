using Ordbox.Services.Models.Dtos;
using System.ComponentModel.DataAnnotations;


namespace Ordbox.Services.Models.Dtos.DtoRequest
{
    public class RequestAddRol
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Key { get; set; }


    }
}
