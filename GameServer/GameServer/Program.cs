using System.Text;
using GameServer;
using GameServer.Db;
using GameServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

var settings = new Settings();
builder.Configuration.Bind("Settings", settings);
builder.Services.AddSingleton(settings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.BearerKey)),
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false
    };

});
builder.Services.AddDbContext<GameDbContext>(o=>o.UseNpgsql(builder.Configuration.GetConnectionString("Db")));
builder.Services.AddControllers()
    .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();