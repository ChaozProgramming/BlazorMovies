using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;

namespace BlazorMovies.Server.Helpers
{
    public class IdentityProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<IdentityUser> claimsFactory;
        private readonly UserManager<IdentityUser> userManager;

        public IdentityProfileService(IUserClaimsPrincipalFactory<IdentityUser> claimsFactory, UserManager<IdentityUser> userManager) 
        {
            this.claimsFactory = claimsFactory;
            this.userManager = userManager;
        }

        /// Include the claims
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = context.Subject.GetSubjectId();
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var claimsPrincipal = await claimsFactory.CreateAsync(user);
                var claims = claimsPrincipal.Claims.ToList();

                var claimsDb = await userManager.GetClaimsAsync(user);
                claims.AddRange(claimsDb);

                context.IssuedClaims = claims;
            }
        }

        // Check if user exist
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userId = context.Subject.GetSubjectId();
            var user = await userManager.FindByIdAsync(userId);
            context.IsActive = user != null;
        }
    }
}
