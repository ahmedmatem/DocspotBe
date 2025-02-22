namespace DocSpot.WebAPI
{
    using DocSpot.WebAPI.Extensions;
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

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddApplicationAuthentication(builder.Configuration);

            // Configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddApplicationSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
        }
    }
}
