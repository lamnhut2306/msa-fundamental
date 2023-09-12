using MSA.Common.Mongo.Extensions;
using MSA.ProductService.Entities;
using MSA.Common.PostgresMassTransit.MassTransit;
using MSA.Common.Security.Authentication;
using MSA.Common.Security.Authorization;
using MSA.ProductService.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongo()
                .AddRepositories<Product>("Product")
                .AddMassTransitWithRabbitMQ()
                .AddMSAAuthentication()
                .AddMSAAuthorization(opt =>
                {
                    opt.AddPolicy("read_access", policy =>
                    {
                        policy.RequireClaim("scope", "productapi.read");
                    });
                });

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwagger(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => 
    { 
        options.OAuthClientId("product-swagger");
        options.OAuthScopes("profile", "openid");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
