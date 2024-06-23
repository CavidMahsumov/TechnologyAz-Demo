using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace OrderService.Api.Extensions.Registration
{
    public static class AuthRegistration
    {
        public static IServiceCollection ConfigureAuth(this IServiceCollection services,IConfiguration _config)
        {
            var singingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["AuthConfig:Secret"]));

            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = singingKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew=TimeSpan.Zero,
                    RequireExpirationTime=true
                };
            
            })    ;
            return services;
        }
    }
}
