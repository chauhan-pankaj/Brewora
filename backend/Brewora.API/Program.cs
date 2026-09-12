using System.Text;
using Brewora.API.Middleware;
using Brewora.Infrastructure;
using Brewora.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace Brewora.API;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Do not call UseUrls under IIS / IIS Express — it causes HTTP 500.30.
            var runningInIis = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_IIS_PHYSICAL_PATH"));
            if (!runningInIis && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
            {
                builder.WebHost.UseUrls("http://0.0.0.0:17421");
            }

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Brewora Café & Kitchen API",
                    Version = "v1",
                    Description = "Menu, orders, Razorpay test payments, reservations, and admin APIs."
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT from POST /api/auth/login"
                });
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
                });
            });

            builder.Services.AddBreworaInfrastructure(builder.Configuration);
            builder.Services.AddCors(o => o.AddPolicy("app", p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

            var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
            if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
            {
                jwt.Key = "brewora-dev-signing-key-change-in-production-min-32-chars!!";
            }

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                    };
                });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseCors("app");
            app.Use(async (ctx, next) =>
            {
                Console.WriteLine($"[Brewora] {ctx.Request.Method} {ctx.Request.Path}");
                await next();
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/health", () => Results.Ok(new { success = true, message = "Brewora API" }));
            app.MapGet("/api/health", () => Results.Ok(new { success = true, message = "Brewora API" }));
            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Brewora API failed to start: " + ex);
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            File.WriteAllText(Path.Combine(logDir, "startup-error.txt"), ex.ToString());
            throw;
        }
    }
}
