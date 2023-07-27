using Microsoft.EntityFrameworkCore;
using Npgsql;
using PFMdotnet.Database;
using PFMdotnet.Database.Repositories;
using PFMdotnet.Database.Repositories.Impl;
using PFMdotnet.Services;
using PFMdotnet.Services.Impl;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using PFMdotnet.Helpers.JsonConverter;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddScoped<ITransactionService, TransactionServiceImpl>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepositoryImpl>();
builder.Services.AddScoped<ICategoryService, CategoryServiceImpl>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepositoryImpl>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(CreateConnectionString(builder.Configuration));
});


builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
InitializeDatabase(app);
app.MapControllers();

app.Run();


string CreateConnectionString(IConfiguration configuration)
{
    var username = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? configuration["Database:Username"];
    var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? configuration["Database:Password"];
    var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? configuration["Database:Name"];
    var host = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? configuration["Database:Host"];
    var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";

    var connBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = host,
        Port = int.Parse(port),
        Username = username,
        Password = password,
        Database = dbName,
        Pooling = true
    };

    return connBuilder.ConnectionString;
}

static void InitializeDatabase(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
    }
}