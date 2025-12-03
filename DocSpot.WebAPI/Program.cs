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

            builder.Services.AddApplicationServices();

            builder.Services.AddAutoMapper(
                typeof(WebMappingProfile).Assembly,
                typeof(CoreMappingProfile).Assembly);

            builder.Services.AddApplicationAuthentication(builder.Configuration);

            // Configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApplicationSwaggerGen();

            var app = builder.Build();

            // Run Migration
            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    db.Database.Migrate();
            //}

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
