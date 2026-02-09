using CarpetBG.Domain.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CarpetBG.Infrastructure.Authorization;

internal sealed class AuthorizationOptionsSetup : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        options.AddPolicy(PolicyConstants.AdminOnly,
            p => p.RequireRole(RoleType.Admin.ToString(), RoleType.SuperAdmin.ToString()));

        options.AddPolicy(PolicyConstants.OperatorAccess,
            p => p.RequireRole(
                RoleType.Admin.ToString(),
                RoleType.SuperAdmin.ToString(),
                RoleType.Manager.ToString()));

        options.AddPolicy(PolicyConstants.CustomerAccess,
            p => p.RequireRole(
                RoleType.Admin.ToString(),
                RoleType.SuperAdmin.ToString(),
                RoleType.Manager.ToString(),
                RoleType.User.ToString()));

        options.AddPolicy(PolicyConstants.SuperAdminOnly,
            p => p.RequireRole(RoleType.SuperAdmin.ToString()));
    }
}
