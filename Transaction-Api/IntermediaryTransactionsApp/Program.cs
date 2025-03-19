
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using IntermediaryTransactionsApp.Config;
using IntermediaryTransactionsApp.Db;
using IntermediaryTransactionsApp.Events;
using IntermediaryTransactionsApp.Exceptions;
using IntermediaryTransactionsApp.Interface.HistoryInterface;
using IntermediaryTransactionsApp.Interface.IOrderService;
using IntermediaryTransactionsApp.Interface.MessageInterface;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.PolicyAuth;
using IntermediaryTransactionsApp.Service;
using IntermediaryTransactionsApp.Strategies;
using IntermediaryTransactionsApp.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Config mapper
var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MapperConfig()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Config redis
var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

// Register services
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<JwtService>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<RedisService>();
builder.Services.AddSingleton<IAuthorizationHandler, SameUserAuthorizationHandler>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddTransient<IHistoryService, HistoryService>();
builder.Services.AddScoped<IUnitOfWorkPersistDb, UnitOfWorkPersistDb>();

// Register Fee Calculation Services
builder.Services.AddScoped<IFeeCalculationStrategyFactory, FeeCalculationStrategyFactory>();
builder.Services.AddScoped<IFeeCalculationStrategy>(sp => 
	sp.GetRequiredService<IFeeCalculationStrategyFactory>().CreateStrategy(FeeStrategyType.Percentage));
builder.Services.AddScoped<IFeeCalculationService, FeeCalculationService>();

// Register Event Handlers
builder.Services.AddScoped<IOrderEventHandler<OrderBoughtEvent>, OrderBoughtEventHandler>();
builder.Services.AddScoped<IOrderEventDispatcher, OrderEventDispatcher>();

// Config jwt
builder.Services.AddHttpContextAccessor();
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

// Config authorizations with JWT
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminPolicy", policy =>
		policy.RequireRole("Admin"));

	options.AddPolicy("CustomerPolicy", policy =>
		policy.RequireRole("Customer"));

    options.AddPolicy("CustomerOrAdminPolicy", policy =>
        policy.RequireRole("Customer", "Admin"));

    options.AddPolicy("SameUserPolicy", policy =>
		policy.Requirements.Add(new SameUserRequirement()));
});

// Config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers()
    .AddOData(opt => opt
                .Select()
                .Filter()
                .OrderBy()
                .Expand()
                .Count()
                .SetMaxTop(100)
                .AddRouteComponents("odata", EdmModelBuilder.GetEdmModel())
            )
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
	});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseService(builder.Configuration.GetConnectionString("DB"));

var app = builder.Build();

// config handle exception
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
