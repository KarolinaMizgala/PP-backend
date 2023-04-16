using Microsoft.EntityFrameworkCore;
using WizardShopAPI.Models;
using WizardShopAPI.Services;
using WizardShopAPI.Storage;

var builder = WebApplication.CreateBuilder(args);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//cors
builder.Services.AddCors(policy => policy.AddDefaultPolicy(build =>
{
    build.WithOrigins("https://localhost:3000").AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed((host) => true).AllowCredentials();
}));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//connect to DB
var connectionString = builder.Configuration.GetConnectionString("WizardDB");
builder.Services.AddDbContext<WizardShopDbContext>(options => options.UseSqlServer(connectionString));

//blob storage service 
builder.Services.AddTransient<IAzureStorage, AzureStorage>();

//delete all jpg files saved in solution folder
builder.Services.AddHostedService<RemoveJpgService>();

builder.Services.AddMvc();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
