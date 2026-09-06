
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Ordbox.Api.Extension;
using Ordbox.Api.Model;
using Ordbox.Domain.Model;
using Ordbox.SDK.Jwt;
using Ordbox.SDK.Security;
using Ordbox.Services.Common;
using Ordbox.Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Usuario = Ordbox.Domain.Model.User;

namespace Ordbox.Api.Controllers.Authentication
{
    public class AccessController : ApiBaseController
    {
        private readonly PeriodService _periodService;

        public AccessController(PeriodService periodService)
        {
            _periodService = periodService;
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Auth([FromBody] LoginModel model,
            [FromServices] UserService service,
            [FromServices] Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var usuario = await service.GetUserLogin(model.UserName, model.Password);
            if (!usuario.Success)
            {
                return Forbid();
                 
            }

            var permission = usuario.Data.Rol.PermissionXRols.Select(y => y.Permission.EnumPermission).ToArray();

            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Data.UserName),
                    new Claim(ClaimTypes.Role, usuario.Data.Rol.Id.ToString()),
                    new Claim("UserId", usuario.Data.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Data.Id.ToString()),
                    new Claim("CompanyId", usuario.Data.CompanyId?.ToString() ?? string.Empty),
                    new Claim(UserExtension.claimPermission, JsonConvert.SerializeObject(permission))
                };

            var token = JWTService.CreateDefaultToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                1,
                configuration["Jwt:SecretKey"],
                authClaims);

            var refreshToken = JWTService.GenerateToken();

            AuthRefresh authRefresh = new AuthRefresh()
            {
                UserId = usuario.Data.Id,
                TokenHash = JWTService.HashToken(refreshToken),
                CreatedOn = DateTimeOffset.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };

            await service.AddAuthRefresh(authRefresh).ConfigureAwait(false);

            await Task.Run(async () =>
            {
                await _periodService.CreateOrReplaceMonthlyPeriod();
            }).ConfigureAwait(false);

            return Ok(new
            {
                id = usuario.Data.Id,
                userName = usuario.Data.UserName,
                firstName = usuario.Data.FirstName,
                rol = usuario.Data.Rol.Key,
                permission,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                refreshToken
            });

        }


        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenModel model, [FromServices] UserService service, [FromServices] IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(model.RefreshToken))
            {
                return Unauthorized();
            }

            // Hash del token recibido
            var tokenHash = JWTService.HashToken(model.RefreshToken);

            // Buscar el refresh token en BD
            var authRefresh = await service.GetAuthRefreshByHash(tokenHash);

            if (authRefresh == null || !authRefresh.Success)
            {
                return Unauthorized();
            }

            if (authRefresh.Data.RevokedAt != null)
            {
                return Unauthorized();
            }

            if (authRefresh.Data.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                return Unauthorized();
            }

            // Obtener usuario
            var usuario = await service.GetUserById(authRefresh.Data.UserId);

            if (usuario == null)
            {
                return Unauthorized();
            }

            // Crear permisos nuevamente
            var permission = usuario.Data.Rol.PermissionXRols
                .Select(y => y.Permission.EnumPermission)
                .ToArray();

            List<Claim> authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Data.UserName),
                    new Claim(ClaimTypes.Role, usuario.Data.Rol.Id.ToString()),
                    new Claim("UserId", usuario.Data.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("CompanyId", usuario.Data.CompanyId?.ToString() ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Data.Id.ToString()),
                    new Claim(UserExtension.claimPermission, JsonConvert.SerializeObject(permission))
                };

            var token = JWTService.CreateDefaultToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                1,
                configuration["Jwt:SecretKey"],
                authClaims);

            authRefresh.Data.RevokedAt = DateTimeOffset.UtcNow;

            await service.UpdateAuthRefresh(authRefresh.Data);

            // Crear nuevo refresh token
            string newRefreshToken = JWTService.GenerateToken();

            AuthRefresh newAuthRefresh = new()
            {
                UserId = usuario.Data.Id,
                TokenHash = JWTService.HashToken(newRefreshToken),
                CreatedOn = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddDays(7)
            };

            await service.AddAuthRefresh(newAuthRefresh);

            return Ok(new
            {
                id = usuario.Data.Id,
                userName = usuario.Data.UserName,
                firstName = usuario.Data.FirstName,
                rol = usuario.Data.Rol.Key,
                permission,

                token = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                expiration = token.ValidTo,

                refreshToken = newRefreshToken
            });
        }

        private static ClaimsIdentity GenerateClaims(OperationResponse<Usuario> usuario)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.Name, usuario.Data.FirstName));
            claims.AddClaim(new Claim(ClaimTypes.Role, usuario.Data.Rol.Key));
            var permission = usuario.Data.Rol.PermissionXRols.Select(y => y.Permission.EnumPermission).ToArray();
            claims.AddClaim(new Claim(UserExtension.claimPermission,  JsonConvert.SerializeObject(permission)));


            return claims;
        }
    }
}
