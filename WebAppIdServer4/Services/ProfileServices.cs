using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebAppIdServer4.Model.Entity;

namespace WebAppIdServer4
{
    public class ProfileServices : IProfileService
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ProfileServices(
            UserManager<UserEntity> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            var user = await _userManager.FindByIdAsync(subjectId);
            context.IssuedClaims = await GetClaimsFromUserAsync(user);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Claim>> GetClaimsFromUserAsync(UserEntity user)
        {
            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName,user.UserName)
            };

            var role = await _userManager.GetRolesAsync(user);
            role.ToList().ForEach(f =>
            {
                claims.Add(new Claim(JwtClaimTypes.Role, f));
            });

            claims.Add(new Claim("姓名", "tom"));
            return claims;
        }

    }
}
