using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Security;
using IdentityServer4.AspNetIdentity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Abp.IdentityServer4
{
    public class AbpIdentityServerUserClaimsPrincipalFactory<TUser, TRole> : UserClaimsFactory<TUser>, ITransientDependency
        where TRole : AbpRole<TUser>, new()
        where TUser : AbpUser<TUser>
    {
        public AbpIdentityServerUserClaimsPrincipalFactory(
            AbpUserManager<TRole, TUser> userManager,
            AbpRoleManager<TRole, TUser> roleManager,
            IOptions<IdentityOptions> optionsAccessor
            ) : base(userManager, roleManager, optionsAccessor)
        {

        }

        [UnitOfWork]
        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var principal = await base.CreateAsync(user);

            if (user.TenantId.HasValue)
            {
                principal.Identities.First().AddClaim(new Claim(AbpClaimTypes.TenantId,user.TenantId.ToString()));
            }

            return principal;
        }
    }
}