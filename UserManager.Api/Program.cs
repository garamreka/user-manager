using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using UserManager.Api.Authentication;
using UserManager.Api.Configurations;
using UserManager.Api.Middlewares;
using UserManager.Api.Repositories;
using UserManager.Api.Services;

var builder = WebApplication.CreateBuilder(args);

IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json")
           .Build();

var userManagerDatabaseConfigurationSection = configuration.GetSection("UserManagerDatabase");
builder.Services.Configure<UserManagerDatabaseConfiguration>(userManagerDatabaseConfigurationSection);

var authenticationConfigurationSection = configuration.GetSection("Authentication");
builder.Services.Configure<AuthenticationConfiguration>(authenticationConfigurationSection);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() 
    { 
        Title = "UserManager.Api", 
        Version = "v3" 
    });
    options.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic authorization header using Bearer scheme"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id  = "basicAuth"
                }
            },
            new string[] {}
        }
    });
    options.EnableAnnotations();
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var mongoClient = new MongoClient(userManagerDatabaseConfigurationSection.GetValue<string>("ConnectionString"));
builder.Services.AddSingleton<IMongoClient>(mongoClient);

builder.Services.AddAutoMapper(typeof(UserMapperProfile));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
    ("BasicAuthentication", null);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
