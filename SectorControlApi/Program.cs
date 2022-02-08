using Microsoft.AspNetCore.Authentication;
using NLog;
using SectorControlApi.Helpers.Dapper;
using SectorControlApi.Security;
using SectorControlApi.Services;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(System.String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

TypeMapperInitializer.InitializeTypeMaps();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, UserAuthenticationHandler>("UserAuthentication", null);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

