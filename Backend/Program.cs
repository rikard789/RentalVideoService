using VideoRentalService.DBContext;
using Microsoft.EntityFrameworkCore;
using VideoRentalService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using VideoRentalService.Models;
using Microsoft.OpenApi.Models;

namespace VideoRentalService1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddScoped<MovieService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<RentalService>();

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Video Rental", Version = "v1" });

            });

            builder.Services.AddDbContext<VideoRentalServiceContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment() || !app.Environment.IsDevelopment())
                {
                //app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "VideoR");
                    //options.SwaggerEndpoint("/swagger/v1/swagger.json", "Video Rental API v1");
                    //c.RoutePrefix = string.Empty; // To make Swagger available at root URL (optional)
                });
            }

            app.UseHttpsRedirection();
            //app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.MapRazorPages();

            app.MapControllers();

            app.Run();
        }
    }
}