using Events;
using HealthChecks.UI.Client;
using Infrastructure.Core;
using Infrastructure.MessageBrokers;
using Ratings.Repositories;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Neo4j.Driver;

var builder = WebApplication.CreateBuilder(args);

string AppName = typeof(Program).Assembly.GetName().Name;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMessageBroker(builder.Configuration);
builder.Services.AddCore(typeof(Program));

builder.Services.AddTransient<IRatingsRepository, RatingsRepository>();

builder.Services.AddSingleton<IDriver>(sp =>
{
    IDriver driver = GraphDatabase.Driver(builder.Configuration.GetConnectionString("Neo4jConnection"), AuthTokens.Basic("", ""));
    //IAsyncSession session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
    return driver;
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    //.AddRedis(builder.Configuration.GetConnectionString("RedisConnection"))
    ;


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
