using VideoRentalService.DBContext;
using Microsoft.EntityFrameworkCore;

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

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<VideoRentalServiceContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<VideoRentalServiceContext>();
            //    if (!dbContext.Database.CanConnect())
            //    {
            //        throw new NotImplementedException("Cannot connect");
            //    }
            //}

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(); // Default start UI from /swagger
                                        //app.UseExceptionHandler("/Error");
                                        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                                        //app.UseHsts();
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
