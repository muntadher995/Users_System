using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UserSystem.API.Services;
 
// Ensure you have the necessary using directives for your project

var builder = WebApplication.CreateBuilder(args);

// ── Add DbContext ────────────────────────────────────────────────────────
builder.Services.AddDbContext<UserSystemDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Add Scoped Services ──────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();

// ── Authentication with JWT ───────────────────────────────────────────────
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer("Bearer", opts =>
   {


    
           

            opts.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
           )
       };

       opts.Events = new JwtBearerEvents
       {


           OnChallenge = context =>
           {
               context.HandleResponse(); // Prevent default behavior

               // Handle 401 Unauthorized
               if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
               {
                   context.Response.ContentType = "application/json";
                   var result = System.Text.Json.JsonSerializer.Serialize(new
                   {
                       message = "Unauthorized. Token is missing, invalid, or expired."
                   });
                   return context.Response.WriteAsync(result);
               }

               return Task.CompletedTask;
           },
           OnForbidden = context =>
           {
               // Handle 403 Forbidden
               context.Response.StatusCode = StatusCodes.Status403Forbidden;
               context.Response.ContentType = "application/json";

               var result = System.Text.Json.JsonSerializer.Serialize(new
               {
                   message = "Forbidden. You do not have permission to access this resource."
               });

               return context.Response.WriteAsync(result);
           },

           OnMessageReceived = context =>
           {
               var token = context.Request.Headers["Authorization"].FirstOrDefault();

               if (!string.IsNullOrEmpty(token) && !token.StartsWith("Bearer "))
               {
                   context.Token = token; // accept raw token
               }

               return Task.CompletedTask;
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


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserSystem", Version = "v1" });

    // 🔒 Add JWT Bearer Auth to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
                     
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── Authorization ────────────────────────────────────────────────────────
builder.Services.AddAuthorization();

// ── Swagger / OpenAPI ────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();


// ── Controllers ─────────────────────────────────────────────────────────
builder.Services.AddControllers();

var app = builder.Build();
await DbSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
