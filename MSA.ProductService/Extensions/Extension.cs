using Humanizer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using static System.Reflection.Metadata.BlobBuilder;
using System.Security.AccessControl;
using System.Xml.Linq;
using MSA.Common.Contracts.Settings;

namespace MSA.ProductService.Extensions
{
    public static class Extension
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
                                 { "productapi.read", "Access read operations" },
                                 { "productapi.write", "Access write operations" }
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
