using Events;
using HealthChecks.UI.Client;
using Infrastructure.Core;
using Infrastructure.MessageBrokers;
using Inventory.Consumers;
using Inventory.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

string AppName = typeof(Program).Assembly.GetName().Name;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddCore(typeof(Program));

builder.Services.AddTransient<IInventoryRepository, InventoryRepository>();

//By connecting here we are making sure that our service
//cannot start until redis is ready. This might slow down startup,
//but given that there is a delay on resolving the ip address
//and then creating the connection it seems reasonable to move
//that cost to startup instead of having the first request pay the
//penalty.
builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection"));
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddRedis(builder.Configuration.GetConnectionString("RedisConnection"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCore();

app.MapHealthChecks("/hc", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});


app.UseAuthorization();

app.MapControllers();

app.Run();
