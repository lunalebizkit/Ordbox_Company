using Ordbox.Services.Common;
using Ordbox.Services.Services;
using Ordbox.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Ordbox.Api.Filter;
using Ordbox.Domain.Enum;
using Ordbox.Domain.Model;
using Ordbox.Services.Models.Dtos.DtoRequest;

namespace Ordbox.Api.Controllers.UpdatePriceProduct
{
    public class UpdatePriceProductController : ApiBaseController
    {
        private readonly ProductService _service;
        public UpdatePriceProductController(ProductService service)
        {
            _service = service;
        }

        /// <summary>
        /// Devuelve un Producto filtrando por Nombre del Producto, Marca, Categoria, Estado y Proveedor.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAccess(Permission = new EPermission[] { EPermission.ListUpdatePrice })]
        [Route("[action]")]
        public async Task<IActionResult> List([FromBody] RequestPaginatedData<ProductFilter> filter)
        {
            return Return(await _service.ListProduct(filter).ConfigureAwait(false));
        }

        /// <summary>
        /// Selecciona un tipo de precio y le agrega al total el valor que se quiera colocar.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [AllowAccess(Permission = new EPermission[] { EPermission.EditUpdatePrice })]
        [Route("[action]")]
        public async Task<IActionResult> UpdatePriceProduct([FromBody] DtoUpdatePriceProduct model)
        {

            return Return(await _service.UpdatePriceProduct(model).ConfigureAwait(false));
        }
    }
}
