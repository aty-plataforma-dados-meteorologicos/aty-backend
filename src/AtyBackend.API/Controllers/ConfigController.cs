using AtyBackend.API.Models;
using AtyBackend.Domain.Account;
using AtyBackend.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace AtyBackend.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IAuthenticate _authentication;
        private readonly IConfiguration Configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly bool _configController;

        public ConfigController(IAuthenticate authentication,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _authentication = authentication ??
            throw new ArgumentNullException(nameof(authentication));
            Configuration = configuration;
            _roleManager = roleManager;
            _userManager = userManager;
            _configController = Configuration.GetValue<bool>("API:ConfigController");
        }

        [HttpGet]
        public async Task<ActionResult> Test()
        {
            return _configController ? Ok("Permitido criar roles e usuários iniciais") : NotFound();
        }

        [HttpGet("SeedRoles")]
        public async Task<ActionResult> CreateRoles()
        {
            if (!_configController)
            {
                return NotFound();
            }

            var added = SeedRoles();
            return Ok(
                new object[]
                {
                    new { RolesCreated = added }
                });
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult> CreateUsers([FromBody] RegisterModel userInfo)
        {
            if (!_configController)
            {
                return NotFound();
            }

            try
            {
                var result = await _authentication.RegisterUser(userInfo.Email, userInfo.Password);

                if (result)
                {
                    var user = await _userManager.FindByEmailAsync(userInfo.Email);

                    user.IsEnabled = true;
                    await _userManager.UpdateAsync(user);

                    switch (userInfo.Role)
                    {
                        case UserRoles.Admin:
                            _userManager.AddToRoleAsync(user, UserRoles.Admin).Wait();
                            break;
                        case UserRoles.Manager:
                            _userManager.AddToRoleAsync(user, UserRoles.Manager).Wait();
                            break;
                        case UserRoles.User:
                            _userManager.AddToRoleAsync(user, UserRoles.User).Wait();
                            break;
                        default:
                            _userManager.AddToRoleAsync(user, UserRoles.Guest).Wait();
                            break;
                    }

                    return Created("User", new ApplicationUserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Role = userInfo.Role,
                        IsEnabled = user.IsEnabled
                    });
                }

                throw new Exception("Invalid user.");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
                return BadRequest(ModelState);
            }
        }

        private List<string> SeedRoles()
        {
            List<string> added = new List<string>();
            var roles = typeof(UserRoles).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.IsLiteral && !f.IsInitOnly)
                .Select(f => f.GetRawConstantValue().ToString())
                .ToList();

            foreach (var role in roles)
            {
                if (!_roleManager.RoleExistsAsync(role).Result)
                {
                    IdentityRole identityRole = new IdentityRole();
                    identityRole.Name = role;
                    identityRole.NormalizedName = role.ToUpper();
                    _ = _roleManager.CreateAsync(identityRole).Result;

                    added.Add(role);
                }
            }

            return added;
        }
    }
}
