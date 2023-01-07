using BlazorMovies.Server.Data;
using BlazorMovies.Server.Helpers;
using BlazorMovies.Shared.DTO;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorMovies.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public UserController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
            
        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Users.AsQueryable();
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage);
            return await queryable.Paginate(paginationDTO)
                .Select(x => new UserDTO { Email = x.Email, UserId = x.Id }).ToListAsync();
        }
            
        [HttpGet("roles")]
        public async Task<ActionResult<List<RoleDTO>>> Get()
        {
            return await context.Roles
                .Select(x => new RoleDTO { RoleName = x.Name }).ToListAsync();
        }

        [HttpPost("assignRole")]
        public async Task<ActionResult> AssignRole(EditRoleDTO editRoleDTO)
        {
            bool foundRole = false;
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);

            if(user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                foreach (Claim c in claims)
                {
                    if (c.Type == JwtClaimTypes.Role && c.Value == editRoleDTO.RoleName)
                    {
                        foundRole = true;
                        break;
                    }
                }

                /*
                var roles = await userManager.GetRolesAsync(user);
                foreach (string r in roles)
                {
                    if (r == editRoleDTO.RoleName)
                    {
                        foundRole = true;
                        break;
                    }
                }
                */

                if (!foundRole)
                    //await userManager.AddToRoleAsync(user, editRoleDTO.RoleName);
                    await userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Role, editRoleDTO.RoleName));
            }

            return NoContent();
        }

        [HttpPost("removeRole")]
        public async Task<ActionResult> RemoveRole(EditRoleDTO editRoleDTO)
        {
            var user = await userManager.FindByIdAsync(editRoleDTO.UserId);
            if(user != null)
                await userManager.RemoveClaimAsync(user, new Claim(JwtClaimTypes.Role, editRoleDTO.RoleName));

            return NoContent();
        }
    }
}
