using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SmartWatering.Core;
using SmartWatering.Core.Hubs;
using SmartWatering.Core.Models.OpenWeatherSettings;
using SmartWatering.Core.Services.SendRecomendationServices;
using SmartWatering.DAL.DbContext;
using System.Text;

namespace SmartWatering.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
        builder.Services.AddHttpClient();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSignalR();

        builder.Services.AddDbContext<SwDbContext>(
            d => d.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnectionString")));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.WithOrigins("http://192.168.31.68:3000", "http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWTSettings:ApiKey"))),
                    };
                });

        builder.Services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddJob<SendRecomendationServiceJob>(j => j
                .WithIdentity("YourJobName")
                .Build());

            q.AddTrigger(t => t
                .WithIdentity("YourTriggerName")
                .ForJob("YourJobName")
                //.WithCronSchedule("0 * * ? * *"));
                .WithCronSchedule("0 0 0/3 1/1 * ? *"));
        });

        builder.Services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        CoreServiceConfiguration.ConfigureServices(builder.Services);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        app.UseAuthorization();

        app.MapHub<MessageHub>("ws");

        app.MapControllers();

        app.Run();
    }
}
