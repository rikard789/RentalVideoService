using VideoRentalService.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VideoRentalService.Controllers;
using VideoRentalService.Services;
using System.IdentityModel.Tokens.Jwt;

namespace VideoRentalService1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = false,
                        ValidIssuer = "VideoRentalService",
                        ValidAudience = "VideoRentalUsers",
                        // Custom logic to retrieve the user's signing key
                        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                        {
                            var tokenHandler = new JwtSecurityTokenHandler();
                            var jwtToken = tokenHandler.ReadJwtToken(token);
                            Console.WriteLine($"Token {jwtToken.SigningKey}");
                            // Extract UserId claim from the token
                            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                            if (string.IsNullOrEmpty(userIdClaim)) throw new SecurityTokenException("Invalid token.");
                            SymmetricSecurityKey signingKey = AuthorizationController.UserKeys[userIdClaim];

                            // Return the user's signing key
                            return new[] { signingKey };
                        }
                    };

                    // Custom logic to reject invalidated tokens
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // Ensure the token is extracted properly
                            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                            context.Token = token;
                            return Task.CompletedTask;
                        },
                    };
                });

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Video Rental API", Version = "v1" });
                // Configure JWT Authentication in Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by a space and the JWT token. Example: Bearer <JWT_TOKEN>"
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

            builder.Services.AddControllers();
            builder.Services.AddScoped<MovieService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<RentalService>();

            builder.Services.AddDbContext<VideoRentalServiceContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Video Rental API v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication(); // Enables JWT token validation
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}