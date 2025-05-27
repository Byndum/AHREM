
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
            #region API setup
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? Environment.GetEnvironmentVariable("MariaDBConnectionString");

            // Add services to the container.
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile(connectionString, optional: true)
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
                {
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
