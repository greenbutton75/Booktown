using ApiGateway.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// env.ContentRootPath
builder.Configuration/*.SetBasePath("C:\\Users\\Valentin Kolesov\\source\\repos\\Booktown\\APIGateway\\")*/.AddJsonFile("Ocelot.json");//.AddEnvironmentVariables();
builder.Services.AddOcelot();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ErrorWrapperMiddleware>();
app.UseOcelot();

app.Run();
