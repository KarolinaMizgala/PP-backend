using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WizardShopAPI.Models;
using WizardShopAPI.Services;
using WizardShopAPI.Storage;
using Swashbuckle.AspNetCore.Filters;

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

//swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"Bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//connect to DB
var connectionString = builder.Configuration.GetConnectionString("WizardDB");
builder.Services.AddDbContext<WizardShopDbContext>(options => options.UseSqlServer(connectionString));

//blob storage services
builder.Services.AddTransient<IAzureReviewStorage, AzureReviewStorage>();
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
