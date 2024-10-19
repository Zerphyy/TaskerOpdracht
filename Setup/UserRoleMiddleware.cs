using Setup.Data;
using System.Security.Claims;

namespace Setup
{
    public class UserRoleMiddleware
    {
        private readonly RequestDelegate _next;

        public UserRoleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, WebpageDBContext dbContext)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = dbContext.Speler?.Find(userId);
                if (user != null)
                {
                    context.Items["UserRole"] = user.Rol;
                    context.Items["UserEmail"] = user.Email;
                }
            }

            await _next(context);
        }
    }
}
