using Events;
using HealthChecks.UI.Client;
using Infrastructure.Core;
using Infrastructure.MessageBrokers;
using Basket.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using Basket.Services;

var builder = WebApplication.CreateBuilder(args);

string AppName = typeof(Program).Assembly.GetName().Name;

// Add services to the container.

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//// ? builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IEventListener>(provider => null);  // TODO remove empty resolution
builder.Services.AddCore(typeof(Program));


builder.Services.AddTransient<IBasketRepository, BasketRepository>();

builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection"));
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddRedis(builder.Configuration.GetConnectionString("RedisConnection"));


var app = builder.Build();


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

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<GreeterService>();
    endpoints.MapGrpcReflectionService();
}); 

app.MapControllers();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();
