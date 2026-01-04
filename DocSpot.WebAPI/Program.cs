namespace DocSpot.WebAPI
{
    using DocSpot.Core.Automapper;
    using DocSpot.Infrastructure.Data;
    using DocSpot.WebAPI.Automapper;
    using DocSpot.WebAPI.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.OpenApi.Models;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationDbContext(builder.Configuration);

            builder.Services.AddApplicationIdentity();

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddAutoMapper(
                typeof(WebMappingProfile).Assembly,
                typeof(CoreMappingProfile).Assembly);

            builder.Services.AddApplicationAuthentication(builder.Configuration);

            // Configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApplicationSwaggerGen();

            var app = builder.Build();

            // Run Migration
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            // Make Kestrel listen on Railway’s PORT
            // This is needed when deploying to Railway.app - it provides the port via an environment variable
            var port = Environment.GetEnvironmentVariable("PORT");
            if (!string.IsNullOrEmpty(port))
            {
                builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
