
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using TeamTaskManagement.API.Middleware;
using TeamTaskManagement.Application.Interfaces;
using TeamTaskManagement.Application.Interfaces.Services;
using TeamTaskManagement.Application.Services;
using TeamTaskManagement.Infrastructure.Data;
using TeamTaskManagement.Infrastructure.Repositories;
using TeamTaskManagement.Infrastructure.Services;

namespace TeamTaskManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);


            // Create logs directory if it doesn't exist
            var logDirectory = @"C:\TeamTaskManagement\Logs";
            Directory.CreateDirectory(logDirectory);



      
            Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "TeamTaskManagement")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: Path.Combine(logDirectory, "application-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 50 * 1024 * 1024, 
        rollOnFileSizeLimit: true,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1),
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} - {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: Path.Combine(logDirectory, "errors-.log"),
        restrictedToMinimumLevel: LogEventLevel.Error,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 90,
        fileSizeLimitBytes: 100 * 1024 * 1024, 
        rollOnFileSizeLimit: true,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1),
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext} - {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

            try
            {
                Log.Information("Starting Team Task Management API");
                Log.Information("Logs directory: {LogDirectory}", logDirectory);

                builder.Host.UseSerilog();

                

                // Add services to the container
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                // Register repositories
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<ITeamRepository, TeamRepository>();
                builder.Services.AddScoped<ITaskRepository, TaskRepository>();
                builder.Services.AddScoped<ITeamUserRepository, TeamUserRepository>();

                // Register application services
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<ITeamService, TeamService>();
                builder.Services.AddScoped<ITaskService, TaskService>();

                // Register infrastructure services
                builder.Services.AddScoped<IJwtService, JwtService>();
                builder.Services.AddScoped<IPasswordService, PasswordService>();

                // Configure JWT authentication
                var jwtSettings = builder.Configuration.GetSection("Jwt");
                var secretKey = jwtSettings["SecretKey"];

                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });

                builder.Services.AddControllers();

                // Configure Swagger
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "Team Task Management API",
                        Version = "v1",
                        Description = "A secure RESTful API for team-based task management"
                    });

                    // Configure JWT authentication in Swagger
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
        });
                });

                // Add CORS if needed
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Team Task Management API v1");
                    });
                }

                app.UseMiddleware<GlobalExceptionMiddleware>();

                app.UseHttpsRedirection();
                app.UseCors("AllowAll");


                app.UseSerilogRequestLogging(options =>
                {
                    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                    options.GetLevel = (httpContext, elapsed, ex) => ex != null
                        ? LogEventLevel.Error
                        : httpContext.Response.StatusCode > 499
                            ? LogEventLevel.Error
                            : LogEventLevel.Information;
                    options.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
                    {
                        diagCtx.Set("RequestHost", httpCtx.Request.Host.Value);
                        diagCtx.Set("RequestScheme", httpCtx.Request.Scheme);
                        diagCtx.Set("UserAgent", httpCtx.Request.Headers.UserAgent.FirstOrDefault());
                        diagCtx.Set("RemoteIP", httpCtx.Connection.RemoteIpAddress?.ToString());
                    };
                });


                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                

                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.Information("Shutting down Team Task Management API");
                Log.CloseAndFlush();
            }
        }
    }
}
