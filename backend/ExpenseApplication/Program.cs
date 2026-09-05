using ExpenseApplication.Core.Entities;
using ExpenseApplication.Core.Interfaces;
using ExpenseApplication.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// logs to console, a rolling daily file, and a SQL table in SerilogLogs 
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.MSSqlServer(
            connectionString: context.Configuration.GetConnectionString("DefaultConnection"),
            sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
            {
                TableName = "SerilogLogs",
                AutoCreateSqlTable = true
            });
});


// EF Core + SQL Server for the app's own data
builder.Services.AddDbContext<ExpenseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Identity for user accounts/passwords
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})

.AddEntityFrameworkStores<ExpenseDbContext>()
.AddDefaultTokenProviders();

// make enums (like FormStatus) show up as their name instead of a number in the JSON
builder.Services.AddControllers().AddJsonOptions(options => { 
        options.JsonSerializerOptions.Converters.Add(new
            System.Text.Json.Serialization.JsonStringEnumConverter()
            ); 
});
builder.Services.AddEndpointsApiExplorer();
// Swagger with bearer token for security
builder.Services.AddSwaggerGen(options=>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name="Authorization",
        Type= Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme="Bearer",
        BearerFormat="JWT",
        In= Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
// register the repository/service classes so controllers can ask for them via DI
builder.Services.AddScoped<IUnitOfWork, ExpenseApplication.Infrastructure.Repositories.UnitOfWork>();
builder.Services.AddScoped<ExpenseApplication.Services.ExpenseFormService>();
builder.Services.AddScoped<ExpenseApplication.Services.Reports.AdminReportService>();
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
// Angular calls the api
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy("AllowAngularApp", builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader();

        });
    });

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseMiddleware<ExpenseApplication.Middleware.ExceptionHandlingMiddleware>();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();


