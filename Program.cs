using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Ai_LibraryApi.API.Services;
using Ai_LibraryApi.Services;
using Ai_LibraryApi.Helper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure Middleware Pipeline
ConfigureMiddleware(app);

app.Run();

// Service Configuration
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Core MVC services
    services.AddControllers();
    services.AddEndpointsApiExplorer();

    // Entity Framework Core
    services.AddDbContext<Ai_LibraryApiDbContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // Custom Services
    services.AddScoped<IAuthService, AuthService>();
    services.AddScoped<FileService>();

    // JWT Authentication
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero
            };

            opts.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "Unauthorized. Token is missing, invalid, or expired."
                    });

                    return context.Response.WriteAsync(result);
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var message = context.HttpContext.User.IsInRole("Admin")
                        ? "You do not have permission to access this resource."
                        : "Your role does not grant access. Please contact your administrator.";

                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        success = false,
                        message
                    });

                    return context.Response.WriteAsync(result);
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine("🔴 JWT auth failed: " + context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("✅ JWT token validated successfully.");
                    return Task.CompletedTask;
                }
            };
        });

    // Authorization
     
     services.AddAuthorization(options =>
            {
        options.AddPolicy("UserOrAdmin", policy =>
            policy.RequireAssertion(context =>
                context.User.HasClaim(c =>
                    (c.Type == ClaimTypes.Role && (c.Value == "User" || c.Value == "Admin"))
                )
                  ));
                });


    // Swagger
    ConfigureSwagger(services);
}

// Swagger Configuration
void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ai_LibraryApi", Version = "v1" });

        // JWT Bearer setup
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Enter 'Bearer {your token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
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
                    },
                    Scheme = "bearer",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                Array.Empty<string>()
            }
        });
    });
}

// Middleware Pipeline
void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseStaticFiles();
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
}










