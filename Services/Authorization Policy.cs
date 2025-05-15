/*using Microsoft.AspNetCore.Authorization;

namespace Ai_LibraryApi.Services
{
    public class Authorization_Policy
    {

        public class RoleOrAdminRequirement : IAuthorizationRequirement
        {
            public string RequiredRole { get; }
            public RoleOrAdminRequirement(string requiredRole)
            {
                RequiredRole = requiredRole;
            }
        }
        public class RoleOrAdminHandler : AuthorizationHandler<RoleOrAdminRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleOrAdminRequirement requirement)
            {
                if (context.User.IsInRole("Admin") || context.User.IsInRole(requirement.RequiredRole))
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }
        }









    }
}
*/