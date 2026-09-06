using Ordbox.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Ordbox.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class ApiBaseController : ControllerBase
    {
        [NonAction]
        internal string GetJti()
        {
            return User.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
        }

        public IActionResult Return<T>(OperationResponse<T> response)
        {
            if (response.Success)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Exception.Info);
        }
    }
}
