
using AHREM_API.Models;
using AHREM_API.Services;
using Microsoft.AspNetCore.Components.Sections;
using MySqlConnector;
using System.Data;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using FeatureHubSDK;
using Serilog;
using LaunchDarkly.Logging;
using Monitoring;

namespace AHREM_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region API setup
            MonitorService.Log.Information("Starting AHREM API...");
    
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();
            
            // Another random comment
            // random comment
            builder.Services.AddScoped<DBService>();

            //// Feature Hub
            FeatureLogging.DebugLogger += (sender, s) => Console.WriteLine("DEBUG: " + s);
            FeatureLogging.TraceLogger += (sender, s) => Console.WriteLine("TRACE: " + s);
            FeatureLogging.InfoLogger += (sender, s) => Console.WriteLine("INFO: " + s);
            FeatureLogging.ErrorLogger += (sender, s) => Console.WriteLine("ERROR: " + s);
            /*
            var config = new EdgeFeatureHubConfig("http://featurehub:8085", "your-api-key");
            var fh = await config.NewContext().Build();
            bool isAddDevice = fh["AddDevice"].IsEnabled;
            */
            // Add services to the container.
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)                  // base config
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)  // dev/stage/prod override
                .AddJsonFile("appsettings.Variables.json", optional: true)                               // optional override file
                .AddEnvironmentVariables();

            builder.Services.AddAuthentication();

            builder.Services.AddScoped<DBService>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            #endregion

            #region Endpoints
            // Get a list of all device (for admins).
            app.MapGet("/GetAllDevices", (DBService dBService) =>
            {
                return Results.Ok(dBService.GetAllDevices());
            });

            // Removes device with given ID.
            app.MapDelete("/RemoveDevice", (int? id, DBService dBService) =>
            {
                var test = dBService.DeleteDevice(id.Value);

                if (!test)
                {
                    return Results.BadRequest("No device with given ID!");
                }

                return Results.Ok("Device removed successfully!");
            });

            // Adds new device to database.
            app.MapPost("/AddDevice", (Device device, DBService dBService) =>
            {
                try
                {/*
                    if (isAddDevice)
                    {
                        return Results.Problem("This function has been disabled");
                    }*/
                    var test = dBService.AddDevice(device);
                    if (!test)
                    {
                        return Results.Problem("Error while trying to add new device!");
                    }

                    return Results.Ok("The device has been added!");
                }
                catch (Exception)
                {
                    return Results.Problem("Error while trying to add new device!");
                }
            });
            #endregion

            app.Run();
        }
    }
}
