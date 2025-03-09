namespace MosefakApp.API.Filters.Authentication
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var user = context.User.Identity;

            if (user is null || !user.IsAuthenticated)
                return;

            var hasPermission = context.User.Claims.Any(c => c.Value == requirement.Permission && c.Type == Permissions.Type);

            if (!hasPermission)
                return;

            context.Succeed(requirement);
            return;
        }
    }
}
