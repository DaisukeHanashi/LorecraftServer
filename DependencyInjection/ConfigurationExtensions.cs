using Lorecraft_API.InternalModel;
using Lorecraft_API.Middleware;

namespace Lorecraft_API.DependencyInjection
{
    public static class ConfigurationExtensions
    {
         private const int MINUTES = 120; 
         private const int DAYS = 5; 
          public static TokenAuthentication GetTokenAuthentication(this IConfiguration configuration) => new()
            {
                SecretKey = configuration.GetSection("TokenAuthentication:SecretKey").Value
                ?? throw new Exception("Unavailable secret key!"),
                Audience = configuration.GetSection("TokenAuthentication:Audience").Value
                ?? throw new Exception("Unavailable Audience key!"),
                TokenPath = configuration.GetSection("TokenAuthentication:TokenPath").Value
                ?? throw new Exception("Unavailable Token Path!"),
                CookieName = configuration.GetSection("TokenAuthentication:CookieName").Value
                ?? throw new Exception("Unavailabe Cookie Name!"),
                ExpirationMinutes = int.Parse(configuration.GetSection("TokenAuthentication:ExpirationMinutes").Value ?? $"{MINUTES}"),
                Issuer = configuration.GetSection("TokenAuthentication:Issuer").Value 
                ?? throw new Exception("Unavailable Issuer!"),
                ExpirationDays = int.Parse(configuration.GetSection("TokenAuthentication:ExpirationDays").Value ?? $"{DAYS}")
            };

            public static void UseRunners(this IApplicationBuilder app, IWebHostEnvironment env){
                app.UseDeveloperExceptionPage();
                app.UseStaticFiles(); 
                //app.UseSession();
                app.UseHttpsRedirection();
                app.UseCors("AllowLocal");
                app.UseAuthentication(); 
                app.UseAuthorization();
                app.UseMiddleware<TokenizationMiddleware>(); 


            }
    }
}