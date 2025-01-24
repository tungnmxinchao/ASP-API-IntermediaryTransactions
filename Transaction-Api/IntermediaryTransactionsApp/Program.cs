using AutoMapper;
using IntermediaryTransactionsApp.Config;
using IntermediaryTransactionsApp.Db;
using IntermediaryTransactionsApp.Interface.UserInterface;
using IntermediaryTransactionsApp.Service;

var builder = WebApplication.CreateBuilder(args);

var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new MapperConfig()));
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDatabaseService(builder.Configuration.GetConnectionString("DB"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
