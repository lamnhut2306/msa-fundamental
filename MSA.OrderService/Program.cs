using MSA.OrderService.Domain;
using MSA.OrderService.Infrastructure.Data;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.OrderService.Services;
using MSA.Common.PostgresMassTransit.MassTransit;
using MSA.OrderService.StateMachine;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MSA.OrderService.Infrastructure.Saga;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MSA.Common.Contracts.Settings;

var builder = WebApplication.CreateBuilder(args);
var serviceSetting = builder.Configuration.GetSection(nameof(PostgresDbSetting)).Get<PostgresDbSetting>();

// Add services to the container.
builder.Services
.AddPostgres<MainDbContext>()
.AddPostgresRepositories<MainDbContext, Order>()
.AddPostgresRepositories<MainDbContext, Product>()
     .AddPostgresUnitofWork<MainDbContext>()
     //.AddMassTransitWithRabbitMQ();
     .AddMassTransitWithPostgresOutbox<MainDbContext>(cfg => {
         cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
            .EntityFrameworkRepository(r => {
                 r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
         
                 r.LockStatementProvider = new PostgresLockStatementProvider();
         
                 r.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) => {
                     builder.UseNpgsql(serviceSetting.ConnectionString, n => {
                         n.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                         n.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                         });
                     });
            });
     });

builder.Services.AddHttpClient<IProductService, ProductService>(cfg => {
    cfg.BaseAddress = new Uri("https://localhost:5002");
});

builder.Services.AddControllers(opt => {
    opt.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
