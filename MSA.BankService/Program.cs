using MSA.BankService.Data;
using MSA.BankService.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.Common.PostgresMassTransit.MassTransit;
using MSA.Common.Security.Authentication;
using MSA.Common.Security.Authorization;
using MSA.BankService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddPostgres<BankDbContext>()
    .AddPostgresRepositories<BankDbContext, Payment>()
    .AddPostgresUnitofWork<BankDbContext>()
    .AddMassTransitWithRabbitMQ()
    .AddMSAAuthentication()
    .AddMSAAuthorization(opt => {
        opt.AddPolicy("read_access", policy =>
        {
            policy.RequireClaim("scope", "bankapi.read");
        });
    });

builder.Services.AddControllers(opt => {
    opt.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("bank-swagger");
        options.OAuthScopes("profile", "openid");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
