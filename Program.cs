using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Propertia.Validators;
using System;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Read allowed origins from environment (set in Azure App Service Application Settings)
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? new[] { "http://localhost:3000", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
// ✅ Add Swagger JWT Support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your JWT token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<PropertiaContext>(options =>
{
    var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__ConnectionString") 
        ?? builder.Configuration.GetConnectionString("ConnectionString");

    if (!string.IsNullOrEmpty(connStr))
    {
        connStr = connStr.Trim('"', '\'', ' '); // Clean up accidentally pasted quotes or spaces
        
        if (connStr.StartsWith("•"))
        {
            Console.Error.WriteLine("=========================================================================");
            Console.Error.WriteLine("CRITICAL ERROR: You copied the hidden dots (••••••••) from the Render dashboard!");
            Console.Error.WriteLine("You MUST click the small 'Copy' icon next to the Internal Database URL.");
            Console.Error.WriteLine("=========================================================================");
            Environment.Exit(1);
        }
        else if (!connStr.StartsWith("postgres://") && !connStr.StartsWith("postgresql://") && !connStr.Contains("=") && Environment.GetEnvironmentVariable("RENDER") == "true")
        {
            Console.Error.WriteLine("=========================================================================");
            Console.Error.WriteLine("CRITICAL ERROR: The ConnectionStrings__ConnectionString provided is INVALID!");
            Console.Error.WriteLine($"You provided: '{connStr}'");
            Console.Error.WriteLine("This looks like just the Hostname. You MUST provide the full 'Internal Database URL' which starts with 'postgres://' !!");
            Console.Error.WriteLine("=========================================================================");
            Environment.Exit(1);
        }
        else if (connStr.Contains("SQLEXPRESS") && Environment.GetEnvironmentVariable("RENDER") == "true")
        {
            Console.Error.WriteLine("WARNING: SQL Server connection string detected on Render. Database connection will likely fail.");
        }

        // Check if the connection string is a URL (like Render provides) instead of a standard format
        if (connStr.StartsWith("postgres://") || connStr.StartsWith("postgresql://"))
        {
            var databaseUri = new Uri(connStr);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builderDb = new Npgsql.NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
                Username = userInfo[0],
                Password = userInfo.Length > 1 ? userInfo[1] : "",
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = Npgsql.SslMode.Prefer,
                TrustServerCertificate = true
            };
            connStr = builderDb.ToString();
        }
    }
    else
    {
        Console.WriteLine("CRITICAL WARNING: ConnectionStrings__ConnectionString is NULL. Check Render Environment Variables.");
    }
        
    options.UseNpgsql(connStr);
});
var app = builder.Build();
app.UseCors("AllowReactApp");
app.UseStaticFiles();

// Apply any pending migrations automatically on startup (Critical for Render deployment)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PropertiaContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
// Swagger enabled in all environments for demo/testing purposes
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
