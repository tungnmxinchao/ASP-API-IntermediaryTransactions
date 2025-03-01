using System.Security.Claims;
using System.Text;
using AutoMapper;
using IntermediaryTransactionsApp.Config;
using IntermediaryTransactionsApp.Db;
using IntermediaryTransactionsApp.Exceptions;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.PolicyAuth;
using IntermediaryTransactionsApp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MapperConfig()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<JwtService>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<RedisService>();
builder.Services.AddSingleton<IAuthorizationHandler, SameUserAuthorizationHandler>();

builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSetting>();
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		RoleClaimType = ClaimTypes.Role,
		ValidIssuer = jwtSettings.Issuer,
		ValidAudience = jwtSettings.Audience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
	};
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy =>
		policy.RequireRole("Admin"));

	options.AddPolicy("SameUserPolicy", policy =>
		policy.Requirements.Add(new SameUserRequirement()));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseService(builder.Configuration.GetConnectionString("DB"));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
