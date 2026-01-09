namespace DocSpot.WebAPI
{
    using DocSpot.Core.Automapper;
    using DocSpot.Core.Contracts;
    using DocSpot.Infrastructure.Data;
    using DocSpot.Infrastructure.Data.Models;
    using DocSpot.WebAPI.Automapper;
    using DocSpot.WebAPI.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using Resend;
    using static DocSpot.Core.Constants;

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

            // Make Kestrel listen on Railway’s PORT
            // This is needed when deploying to Railway.app - it provides the port via an environment variable
            var port = Environment.GetEnvironmentVariable("PORT");
            if (int.TryParse(port, out var p))
            {
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(p);
                });
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                    policy.WithOrigins(FrontendBaseUrlRemote, FrontendBaseUrlRemoteWWW, FrontendBaseUrlLocal)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                );
            });

            // Register Resend Email Service
            builder.Services.AddOptions();
            builder.Services.AddHttpClient<ResendClient>();
            builder.Services.Configure<ResendClientOptions>(o =>
            {
                o.ApiToken = Environment.GetEnvironmentVariable("RESEND_APITOKEN")!;
            });
            builder.Services.AddTransient<IResend, ResendClient>();

            var app = builder.Build();

            // Run Migration
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("Frontend");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            // manually run: POST /api/admin/holidays/sync/2026 to sync holidays for year 2026
            app.MapPost("/api/admin/holidays/sync/{year:int}", 
                async (int year, IHolidaysService sync, CancellationToken ct) =>
                {
                    var changes = await sync.SyncYearAsync("BG", year, ct);
                    return Results.Ok(new { year, country = "BG", changes });
                }
            );

            app.Run();
        }
    }
}
