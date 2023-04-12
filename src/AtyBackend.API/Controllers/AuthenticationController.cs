using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AtyBackend.API.Models;
using AtyBackend.Domain.Account;
using AtyBackend.Infrastructure.Data.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AtyBackend.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticate _authentication;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthenticationController(IAuthenticate authentication,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _authentication = authentication ??
            throw new ArgumentNullException(nameof(authentication));
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<ActionResult<UserToken>> Login([FromBody] LoginModel userInfo)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            if (user is null)
            {
                return NotFound("Usuário não cadastrado");
            }

            if (await _authentication.Authenticate(userInfo.Email, userInfo.Password))
            {
                return await GenerateTokenAsync(userInfo);
            }

            //if (user is null || !user.IsEnabled || user.IsDeleted)
            //{
            //    return BadRequest("Invalid Login attempt.");
            //}

            //var result = await _authentication.Authenticate(userInfo.Email, userInfo.Password);
            //if (result)
            //{
            //    return await GenerateTokenAsync(userInfo);
            //}

            //ModelState.AddModelError("Error", "Invalid Login attempt");
            return BadRequest("Tentativa de acesso inválida");
        }
        catch (Exception e)
        {
            //ModelState.AddModelError("Error", e.Message);
            return BadRequest(e.Message);
        }
    }

    [HttpPost("TokenRefresh")]
    public async Task<ActionResult<UserToken>> TokenRefresh([FromBody] TokenRefreshModel tokenRefresh)
    {
        try
        {
            string? accessToken = tokenRefresh.AccessToken;
            string? refreshToken = tokenRefresh.RefreshToken;
            var user = await GetUserByToken(accessToken);

            ValidateRefreshToken(refreshToken, user);

            if (user.IsEnabled && !user.IsDeleted)
            {
                var userInfo = new LoginModel();
                userInfo.Email = user.Email;
                var newTokens = await GenerateTokenAsync(userInfo);

                return Ok(newTokens);
            }

            return BadRequest("Invalid user");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    [Authorize]
    [HttpPost("RevokeToken")]
    public async Task<IActionResult> RevokeUserToken([FromBody] TokenRefreshModel tokenRefresh)
    {
        try
        {
            string accessToken = tokenRefresh.AccessToken;
            string refreshToken = tokenRefresh.RefreshToken;
            var user = await GetUserByToken(accessToken);

            ValidateRefreshToken(refreshToken, user);

            user.RefreshToken = null;
            user.RefreshTokenExpiration = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("RevokeAllTokens")]
    public async Task<IActionResult> RevokeAll()
    {
        try
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return NoContent();
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    private void ValidateRefreshToken(string? refreshToken, ApplicationUser? user)
    {
        if (refreshToken == null ||
            user == null ||
            user.RefreshToken != refreshToken ||
            user.RefreshTokenExpiration <= DateTime.UtcNow)
        {
            throw new Exception("Invalid refresh token.");
        }
    }

    private async Task<ApplicationUser> GetUserByToken(string? accessToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);

        if (principal == null)
        {
            throw new Exception("Invalid access token/refresh token.");
        }

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        var email = token.Claims.First(c => c.Type == "email").Value;

        return await _userManager.FindByNameAsync(email);
    }

    private async Task<UserToken> GenerateTokenAsync(LoginModel userInfo)
    {
        var user = await _userManager.FindByEmailAsync(userInfo.Email);

        if (user.IsDeleted || !user.IsEnabled)
        {
            throw new Exception("Invalid user");
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim("email", userInfo.Email),
            //new Claim("meuvalor", "o que voce quiser"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        //gerar chave privada para assinar o token
        // same authSigningKey
        var privateKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

        //gerar a assinatura digital
        var credentials = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

        //definir o tempo de expiração
        _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out
                int tokenValidityInMinutes);
        var tokenExpiration = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);

        //gerar o token
        JwtSecurityToken token = new JwtSecurityToken(
            //emissor
            issuer: _configuration["Jwt:Issuer"],
            //audiencia
            audience: _configuration["Jwt:Audience"],
            //claims
            claims: claims,
            //data de expiracao
            expires: tokenExpiration,
            //assinatura digital
            signingCredentials: credentials
            );

        // Refresh token
        string refreshToken = GenerateRefreshToken();
        //definir o tempo de expiração
        _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"],
                    out int refreshTokenValidityInMinutes);
        var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiration = refreshTokenExpiration;

        await _userManager.UpdateAsync(user);

        return new UserToken()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            TokenExpiration = tokenExpiration,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = refreshTokenExpiration
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                               .GetBytes(_configuration["JWT:SecretKey"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                        out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                  !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                 StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
