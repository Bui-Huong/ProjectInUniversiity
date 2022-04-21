using System.Security.Claims;
using AssetManagementWebApi.Repositories.EFContext;

namespace AssetManagementWebApi.CustomMiddleware;
public class CheckDisable
{
    private readonly RequestDelegate _next;

    public CheckDisable(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context,AssetManagementDBContext dbContext )
    {
        await Task.Run(async () =>
         {
             if (context.User.Identity.IsAuthenticated)
             {
                 ClaimsIdentity? claimsIdentity = context.User.Identity as ClaimsIdentity;


                 var userData = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
                 var userCode = userData.Split(",")[0];
                 var user = dbContext.AppUser.FirstOrDefault(x => x.Code == userCode);

                 if (user == null || user.IsDisabled)
                 {

                     context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                     context.Response.ContentType = "application/json";
                     await context.Response.WriteAsync(claimsIdentity.ToString());
                     return;
                 }


             }
             await _next(context);
         }
         );
    }
}