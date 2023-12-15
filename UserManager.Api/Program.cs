using MongoDB.Driver;
using UserManager.Api.Configurations;
using UserManager.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json")
           .Build();

var userManagerDatabaseConfigurationSection = configuration.GetSection("UserManagerDatabase");
builder.Services.Configure<UserManagerDatabaseConfiguration>(userManagerDatabaseConfigurationSection);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var mongoClient = new MongoClient(userManagerDatabaseConfigurationSection.GetValue<string>("ConnectionString"));
builder.Services.AddSingleton<IMongoClient>(mongoClient);

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
