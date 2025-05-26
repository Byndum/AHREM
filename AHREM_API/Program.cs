
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

namespace AHREM_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.variables.json", optional: true)
                .AddEnvironmentVariables();
            builder.Services.AddAuthentication();

            builder.Services.AddScoped<DBService>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Get a list of all device (for admins).
            app.MapGet("/GetAllDevices", (DBService dBService) =>
            {
                return Results.Ok(dBService.GetAllDevices);
            });

            // Removes device with given ID.
            app.MapGet("/RemoveDevice", (int? id, DBService dBService) =>
            {
                if (id != null)
                {
                    return Results.Ok(dBService.DeleteDevice(id.Value));
                }
                return Results.BadRequest("No device with given ID!");
            });
            
            // Adds new device to database.
            app.MapPost("/AddDevice", (Device device, DBService dBService) =>
            {
                var test = dBService.AddDevice(device);

                if (!test)
                {
                    return Results.Problem("Error while trying to add new device!");
                }

                return Results.Ok("The device has been added!");
            });

            app.Run();
        }
    }
}
