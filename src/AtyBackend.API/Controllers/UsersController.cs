using AtyBackend.API.Email;
using AtyBackend.API.Helpers;
using AtyBackend.API.Models;
using AtyBackend.Application.DTOs;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Account;
using AtyBackend.Domain.Entities;
using AtyBackend.Domain.Model;
using AtyBackend.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reactive.Subjects;


namespace AtyBackend.API.Controllers;

//[Authorize(Roles = "Admin,Manager")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IAuthenticate _authentication;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ISendEmail _sendEmail;

    public UsersController(IAuthenticate authentication,
        IConfiguration configuration,
        ISendEmail sendEmail,
        UserManager<ApplicationUser> userManager)
    {
        _authentication = authentication ??
            throw new ArgumentNullException(nameof(authentication));
        _userManager = userManager;
        _configuration = configuration;
        _sendEmail = sendEmail;
    }

    [HttpPost]
    public async Task<ActionResult> RegisterUser([FromBody] RegisterModel userInfo)
    {
        try
        {
            var result = await _authentication.RegisterUser(userInfo.Email, userInfo.Password);

            if (result)
            {
                var user = await _userManager.FindByEmailAsync(userInfo.Email);

                user.Name = userInfo.Name;
                user.IsEnabled = true;
                user.IsDeleted = false;
                user.Type = userInfo.Type;
                await _userManager.UpdateAsync(user);

                _userManager.AddToRoleAsync(user, UserRoles.User).Wait();
                return StatusCode(201);
            }

            throw new Exception("Invalid user.");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null || user.IsDeleted)
            {
                return NotFound("User not found");
            }

            // Somente torna o usuário inativo
            user.IsDeleted = true;
            user.RefreshToken = null;
            user.RefreshTokenExpiration = null;
            var result = await _userManager.UpdateAsync(user);

            // Opção de deletar de verdade o usuário
            //var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return NoContent();
            }

            throw new Exception("Invalid user.");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    public async Task<ActionResult<ApiResponsePaginated<ApplicationUserDTO>>> GetUsers([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var paginated = new ApiResponsePaginated<ApplicationUserDTO>(pageNumber, pageSize);
        var total = await _userManager.Users.Where(i => i.IsDeleted == false).CountAsync();

        try
        {
            var users = await _userManager.Users
                .Where(i => i.IsDeleted == false)
                .OrderByDescending(i => i.Id)
                .Skip((paginated.PageNumber - 1) * paginated.PageSize)
                .Take(paginated.PageSize)
                .ToListAsync();

            if (users != null)
            {
                var usersDTO = users.Select(user => new ApplicationUserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Type = user.Type,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    IsEnabled = user.IsEnabled
                });

                var paginatedDtos = new Paginated<ApplicationUserDTO>(paginated.PageNumber, paginated.PageSize, total, usersDTO.ToList());
                paginated.AddData(paginatedDtos, Request);

                return paginated.Data.Count() < 1 ? NotFound("Empty page") : paginated.TotalItems < 1 ? NotFound("Sensors not found") : Ok(paginated);
            }

            return NotFound("Users is null");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationUserDTO>> GetUserById(string id)
    {
        try
        {
            var user = await _userManager.Users
                .Where(i => i.Id == id)
                .Where(i => i.IsDeleted == false)
                .FirstOrDefaultAsync();

            return user is null ? NotFound("User not found") : Ok(new ApplicationUserDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Type = user.Type,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    IsEnabled = user.IsEnabled
                });
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApplicationUserDTO>> UpdateUser([FromBody] ApplicationUserDTO userDTO)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userDTO.Id);

            if (user is null || user.IsDeleted)
            {
                throw new NullReferenceException("Invalid email address");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (userDTO.Role != role)
            {
                switch (userDTO.Role)
                {
                    case UserRoles.Admin:
                        _userManager.AddToRoleAsync(user, UserRoles.Admin).Wait();
                        break;
                    case UserRoles.Manager:
                        _userManager.AddToRoleAsync(user, UserRoles.Manager).Wait();
                        break;
                    case UserRoles.Maintainer:
                        _userManager.AddToRoleAsync(user, UserRoles.Maintainer).Wait();
                        break;
                    case UserRoles.User:
                        _userManager.AddToRoleAsync(user, UserRoles.User).Wait();
                        break;
                    default:
                        throw new Exception("Invalid role");
                }

                // remove tokens
                user.RefreshToken = null;
                user.RefreshTokenExpiration = null;

                await _userManager.RemoveFromRolesAsync(user, roles);
            }

            if (userDTO.IsEnabled != user.IsEnabled)
            {

                user.IsEnabled = userDTO.IsEnabled;
                user.RefreshToken = null;
                user.RefreshTokenExpiration = null;
            }

            await _userManager.UpdateAsync(user);

            return Ok(new ApplicationUserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                IsEnabled = user.IsEnabled
            });
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    [HttpGet("Roles")]
    public ActionResult<List<string>> GetUserRoles()
    {
        return Ok(GetRoles());
    }

    [HttpPut("Password")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> ChangePassword([FromBody] LoginModel userInfo)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(userInfo.Email);


            if (user is not null && !user.IsDeleted && user.IsEnabled)
            {
                if (await _userManager.CheckPasswordAsync(user, userInfo.Password))
                {
                    throw new Exception("The new password must be different of the current password");
                }

                user.RefreshToken = null;
                user.RefreshTokenExpiration = null;
                await _userManager.UpdateAsync(user);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, userInfo.Password);

                if (result.Succeeded)
                {
                    return Ok($"Password for user {userInfo.Email} was changed successfully");
                }

                throw new Exception("Password change failed");
            }

            throw new Exception("User not found");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    [HttpPost("ResetPassword")]
    public async Task<ActionResult> ResetPassword([FromBody] RequestResetPassword requestResetPassword)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(requestResetPassword.Email);
            if (user is not null && !user.IsDeleted && user.IsEnabled)
            {
                _ = int.TryParse(_configuration["ResetPassword:ResetPasswordCodeValidityInMinutes"],
                    out int resetPasswordCodeValidityInMinutes);
                var resetPasswordCodeExpiration = DateTime.UtcNow.AddMinutes(resetPasswordCodeValidityInMinutes);

                user.ResetPasswordCodeExpiration = resetPasswordCodeExpiration;
                user.ResetPasswordCode = ResetPasswordCode();

                await _userManager.UpdateAsync(user);

                var result = await SendResetPasswordEmail(user.Email, user.ResetPasswordCode.Value, resetPasswordCodeValidityInMinutes);

                return result ? Ok($"Password reset email sent to {requestResetPassword.Email}") : BadRequest("Error sending email");
            }
            throw new Exception("Invalid user");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    [HttpPut("ResetPassword")]
    public async Task<ActionResult> NewPassword([FromBody] ResetPassword requestResetPassword)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(requestResetPassword.Email);
            if (user is not null && !user.IsDeleted && user.IsEnabled)
            {
                if(user.ResetPasswordCodeExpiration <= DateTime.UtcNow) { return BadRequest("Reset password code expired"); }

                if (user.ResetPasswordCode != requestResetPassword.Code) { return BadRequest("Invalid reset password code"); }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, requestResetPassword.Password);

                if (result.Succeeded)
                {
                    user.ResetPasswordCodeExpiration = null;
                    user.ResetPasswordCode = null;
                    await _userManager.UpdateAsync(user);

                    return Ok($"Password for user {requestResetPassword.Email} was reseted successfully");
                }
                throw new Exception("Password reset failed");
            }
            throw new Exception("Invalid user");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("Error", e.Message);
            return BadRequest(ModelState);
        }
    }

    private async Task<bool> SendResetPasswordEmail(string email, int code, int codeValidityInMinutes)
    {
        try
        {
            string subject = "ATY - Reset Password Code";
            string message = $"Your reset password code is {code}. Expires in {codeValidityInMinutes} minutes.";
            await _sendEmail.SendEmailAsync(email, subject, message);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    private static int ResetPasswordCode()
    {
        Guid guid = Guid.NewGuid();
        byte[] bytes = guid.ToByteArray();
        int num = BitConverter.ToInt32(bytes, 0) % 1000000;

        if (num < 0) // Garante um número positivo com 6 dígitos
        {
            num = -num;
        }

        return num;
    }

    private static List<string> GetRoles() => UserRoles.ToList();

    #region old
    //private static int TotalPages(double totalItems, double pageSize) => (int)Math.Ceiling(totalItems / pageSize);

    ////condição em que não tem não tem previous
    ////-> pageNumber = 1
    ////-> pageNumber > TotalPages
    ////-> pageNumber < 1
    //private static bool HasNextPage(Paginated<ApplicationUserDTO> result) =>
    //    !(result.PageNumber == result.TotalPages || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    ////condição em que não tem next
    ////-> pageNumber = TotalPages
    ////-> pageNumber > TotalPages
    ////-> pageNumber < 1
    //private static bool HasPreviousPage(Paginated<ApplicationUserDTO> result) =>
    //    !(result.PageNumber == 1 || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    //private static string GetPageUrl(Paginated<ApplicationUserDTO> result, string url, bool isNextPage = true) => isNextPage ?
    //    url + "?pageNumber=" + (result.PageNumber + 1) + "&pageSize=" + result.PageSize :
    //    url + "?pageNumber=" + (result.PageNumber - 1) + "&pageSize=" + result.PageSize;
    #endregion
}
