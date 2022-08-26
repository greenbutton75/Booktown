using ApiGateway.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddOcelot($"Configurations/{builder.Environment.EnvironmentName}/", builder.Environment);


SecurityKey securityKey = null;
if (!string.IsNullOrEmpty(builder.Configuration["AppSettings:JWTSecret"]) && string.IsNullOrEmpty(builder.Configuration["AppSettings:JWTRSAPublicKey"]))
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration["AppSettings:JWTSecret"]);
    securityKey = new SymmetricSecurityKey(key);
}
if (!string.IsNullOrEmpty(builder.Configuration["AppSettings:JWTRSAPublicKey"]))
{
    var publicKey = Convert.FromBase64String(builder.Configuration["AppSettings:JWTRSAPublicKey"]);

    RSA rsa = RSA.Create();
    rsa.ImportSubjectPublicKeyInfo(publicKey, out _);
    securityKey = new RsaSecurityKey(rsa);
}




builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("ProviderKey", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.MapInboundClaims = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = securityKey,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true
    };
});
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
