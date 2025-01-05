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
            

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Video Rental", Version = "v1" });

            });

            builder.Services.AddControllers();
            //builder.Services.AddScoped<MovieService>();
            //builder.Services.AddScoped<UserService>();
            //builder.Services.AddScoped<RentalService>();

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
                options.SwaggerEndpoint("../swagger/v1/swagger.json", "VideoR");
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Video Rental API v1");
            });


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}