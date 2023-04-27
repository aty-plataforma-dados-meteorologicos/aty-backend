using AtyBackend.API.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AtyBackend.API.Helpers
{
    public class UsersHelpers
    {

        //public GetEmail(string email)
        //{
        //    var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        //    // Se o email não estiver presente no token, retorna um erro
        //    if (userEmail is null)
        //    {
        //        return BadRequest("O token JWT não contém o email do usuário.");
        //    }

        //    // find user to get id
        //    var user = await _userManager.FindByEmailAsync(userEmail);
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var role = roles.FirstOrDefault();

        //    if (role == UserRoles.User)
        //    {
        //        await _userManager.RemoveFromRolesAsync(user, roles);
        //        _userManager.AddToRoleAsync(user, UserRoles.Maintainer).Wait();
        //    }
        //}

    }
}
