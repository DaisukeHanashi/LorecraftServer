using System.Security.Claims;
using Lorecraft_API.Resources;
using Lorecraft_API.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Lorecraft_API.Middleware
{
    public class TokenizationMiddleware(RequestDelegate next, CookieTicketStore store)
    {
        private readonly RequestDelegate _next = next;

        private readonly ISecureDataFormat<AuthenticationTicket> _ticketFormat = store.TicketFormat;


        public async Task Invoke(HttpContext context)
        {
            try
            {
                string? accessToken = await context.GetTokenAsync(Constants.Authorization) ?? context.Request.Cookies[Constants.Authorization];

                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Request.Headers.Append(Constants.Authorization, string.Join(' ', JwtBearerDefaults.AuthenticationScheme, accessToken));

                    if(context.User?.Identity?.IsAuthenticated is true)
                    {
                        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier); 

                        if(!string.IsNullOrEmpty(userId))
                          context.Request.Headers.Append(Constants.Identifier, userId); 


                    }


                }

                await _next.Invoke(context);
            }
            catch (Exception ex) when (ex is SecurityTokenExpiredException || ex is SecurityTokenValidationException)
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new ResultMessage
                {
                    Code = 401,
                    Message = "Invalid or expired token"
                });
            }
        }
    }
}