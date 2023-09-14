using MSA.BankService.Data;
using MSA.BankService.Domain;
using MSA.Common.PostgresMassTransit.PostgresDB;
using MSA.Common.PostgresMassTransit.MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddPostgres<BankDbContext>()
    .AddPostgresRepositories<BankDbContext, Payment>()
    .AddPostgresUnitofWork<BankDbContext>()
    .AddMassTransitWithRabbitMQ();

builder.Services.AddControllers(opt => {
    opt.SuppressAsyncSuffixInActionNames = false;
});
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
