using Microsoft.OpenApi.Models;

using MSA.Common.Contracts.Settings;

namespace MSA.BankService
{
    public static class Extensions
    {
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var srvUrlsSetting = configuration.GetSection(nameof(ServiceUrlSetting)).Get<ServiceUrlSetting>();
            services.AddSwaggerGen(options =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{srvUrlsSetting.IdentityServiceUrl}/connect/authorize"),
                            TokenUrl = new Uri($"{srvUrlsSetting.IdentityServiceUrl}/connect/token"),
                            Scopes = new Dictionary<string, string>
                             {
                                 { "bankapi.read", "Access read operations" },
                                 { "bankapi.write", "Access write operations" }
                             }
                        }
                    },
                    Type = SecuritySchemeType.OAuth2
                };

                options.AddSecurityDefinition("OAuth", scheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference { Id = "OAuth", Type = ReferenceType.SecurityScheme }
                        },
                        new List<string> { }
                    }
                 });
            });
        }
    }
}
