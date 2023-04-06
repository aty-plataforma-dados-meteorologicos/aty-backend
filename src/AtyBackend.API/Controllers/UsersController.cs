using AtyBackend.API.Models;
using AtyBackend.Application.ViewModels;
using AtyBackend.Domain.Account;
using AtyBackend.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace AtyBackend.API.Controllers;

//[Authorize(Roles = "Admin,Manager")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IAuthenticate _authentication;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(IAuthenticate authentication, UserManager<ApplicationUser> userManager)
    {
        _authentication = authentication ??
            throw new ArgumentNullException(nameof(authentication));
        _userManager = userManager;
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

                user.IsEnabled = true;
                user.IsDeleted = false;
                user.Type = userInfo.Type;
                await _userManager.UpdateAsync(user);

                _userManager.AddToRoleAsync(user, UserRoles.User).Wait();

                // return only a 201 created
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
    public async Task<ActionResult<Paginated<ApplicationUserDTO>>> GetUsers([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var pageNumberNotNull = pageNumber is null ? 1 : pageNumber.Value;
        var pageSizeNotNull = pageSize is null ? 10 : pageSize.Value;

        var totalUsers = await _userManager.Users.Where(i => i.IsDeleted == false).CountAsync();
        var totalPages = (totalUsers > 0 && totalUsers < pageSizeNotNull) ? 1 : TotalPages(totalUsers, pageSizeNotNull);

        var url = "https://" + Request.Host.ToString() + Request.Path.ToString();
        url = Request.IsHttps ? url : url.Replace("https", "http");

        try
        {
            var users = await _userManager.Users
                .Where(i => i.IsDeleted == false)
                .OrderByDescending(i => i.Id)
                .Skip((pageNumberNotNull - 1) * pageSizeNotNull)
                .Take(pageSizeNotNull)
                .ToListAsync();

            if (users != null)
            {
                var usersDTO = users.Select(user => new ApplicationUserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    IsEnabled = user.IsEnabled
                });



                var paginatedUsers = new Paginated<ApplicationUserDTO>
                {
                    PageNumber = pageNumberNotNull,
                    PageSize = pageSizeNotNull,
                    TotalPages = totalPages,
                    TotalItems = totalUsers,
                    Data = usersDTO.ToList()
                };

                paginatedUsers.PreviousPageUrl = HasPreviousPage(paginatedUsers) ? GetPageUrl(paginatedUsers, url, false) : null;
                paginatedUsers.NextPageUrl = HasNextPage(paginatedUsers) ? GetPageUrl(paginatedUsers, url) : null;

                return paginatedUsers;
            }

            throw new Exception("Users is null");
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
            //var user = await _userManager.FindByIdAsync(id);

            var user = await _userManager.Users
                .Where(i => i.Id == id)
                .Where(i => i.IsDeleted == false)
                .FirstOrDefaultAsync();

            return user is null ? NotFound("User not found") : Ok(new ApplicationUserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    IsEnabled = user.IsEnabled
                });
        } catch (Exception e)
        {
            return BadRequest(e.Message);
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

    private static List<string> GetRoles() => UserRoles.ToList();

    private static int TotalPages(double totalItems, double pageSize) => (int)Math.Ceiling(totalItems / pageSize);

    //condição em que não tem não tem previous
    //-> pageNumber = 1
    //-> pageNumber > TotalPages
    //-> pageNumber < 1
    private static bool HasNextPage(Paginated<ApplicationUserDTO> result) =>
        !(result.PageNumber == result.TotalPages || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    //condição em que não tem next
    //-> pageNumber = TotalPages
    //-> pageNumber > TotalPages
    //-> pageNumber < 1
    private static bool HasPreviousPage(Paginated<ApplicationUserDTO> result) =>
        !(result.PageNumber == 1 || result.PageNumber > result.TotalPages || result.PageNumber < 1);

    private static string GetPageUrl(Paginated<ApplicationUserDTO> result, string url, bool isNextPage = true) => isNextPage ?
        url + "?pageNumber=" + (result.PageNumber + 1) + "&pageSize=" + result.PageSize :
        url + "?pageNumber=" + (result.PageNumber - 1) + "&pageSize=" + result.PageSize;
}
