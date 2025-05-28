using AHREM_API;
using AHREM_API.Models;
using AHREM_API.Services;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MySqlConnector;

namespace AHREM.API.Tests
{
    public class EndPointTests
    {
        private DBService GetDbService()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Variables.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            return new DBService(config);
        }

        [Test]
        public void AddDevice_ShouldReturnTrue_WhenDeviceIsValid()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ConnectionStrings:DefaultConnection", "Server=localhost;Database=air_monitor_db;Uid=root;Pwd=1234;"}
                })
                .Build();

            var dbService = new DBService(config);
            var device = new Device { IsActive = true, Firmware = "v1.0", MAC = "00:11:22:33:44:55" };

            var result = dbService.AddDevice(device);

            Assert.That(result, Is.True);
        }

        [Test]
        public void GetAllDevices_ShouldReturnDevices_WhenTheyExist()
        {
            var dbService = GetDbService();
            var devices = dbService.GetAllDevices();

            Assert.That(devices, Is.Not.Null);
            Assert.That(devices.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void DeleteDevice_ShouldReturnTrue_WhenDeviceExists()
        {
            var dbService1 = GetDbService();
            var newDevice = new Device { IsActive = true, Firmware = "test", MAC = "AA:BB:CC:DD:EE:01" };
            dbService1.AddDevice(newDevice);

            var dbService2 = GetDbService();
            var insertedDevice = dbService2.GetAllDevices().Last();

            var dbService3 = GetDbService();
            var result = dbService3.DeleteDevice(insertedDevice.ID.Value);

            Assert.That(result, Is.True);
        }

        [Test]
        public void DeleteDevice_ShouldReturnFalse_WhenDeviceDoesNotExist()
        {
            var dbService = GetDbService();
            var result = dbService.DeleteDevice(-1); // nonexistent

            Assert.That(result, Is.False); // fixed
        }

        [Test]
        public async Task GetAllDevices_Endpoint_ShouldReturnOkAndJson()
        {
            var appFactory = new WebApplicationFactory<Program>();
            var client = appFactory.CreateClient();

            var response = await client.GetAsync("/GetAllDevices");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var json = await response.Content.ReadAsStringAsync();
            var devices = JsonSerializer.Deserialize<List<Device>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.That(devices, Is.Not.Null);
            Assert.That(devices.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task AddDevice_Endpoint_ShouldReturnSuccess()
        {
            var appFactory = new WebApplicationFactory<Program>();
            var client = appFactory.CreateClient();

            var device = new Device
            {
                IsActive = true,
                Firmware = "1.2.3",
                MAC = "00:AA:BB:CC:DD:EE"
            };

            var json = JsonSerializer.Serialize(device);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/AddDevice", content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var responseText = await response.Content.ReadAsStringAsync();
            Assert.That(responseText, Does.Contain("The device has been added"));
        }

        [Test]
        public async Task RemoveDevice_Endpoint_ShouldRemoveSuccessfully()
        {
            var appFactory = new WebApplicationFactory<Program>();
            var client = appFactory.CreateClient();

            var device = new Device
            {
                IsActive = true,
                Firmware = "ToDelete",
                MAC = "11:22:33:44:55:66"
            };

            var json = JsonSerializer.Serialize(device);
            var postContent = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("/AddDevice", postContent);

            var getResponse = await client.GetAsync("/GetAllDevices");
            var jsonText = await getResponse.Content.ReadAsStringAsync();
            var allDevices = JsonSerializer.Deserialize<List<Device>>(jsonText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var lastDevice = allDevices.Last();

            var deleteResponse = await client.DeleteAsync($"/RemoveDevice?id={lastDevice.ID}");

            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var msg = await deleteResponse.Content.ReadAsStringAsync();
            Assert.That(msg, Does.Contain("Device removed successfully"));
        }

        [Test]
        public async Task RemoveDevice_Endpoint_ShouldReturnBadRequest_WhenIdNotFound()
        {
            var appFactory = new WebApplicationFactory<Program>();
            var client = appFactory.CreateClient();

            // Use an ID that likely does not exist
            int nonexistentId = -999;

            var response = await client.DeleteAsync($"/RemoveDevice?id={nonexistentId}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.EqualTo("\"No device with given ID!\""));
        }
        [Test]
        public async Task AddDevice_ShouldReturnProblem_WhenFirmwareTooLong()
        {
            var appFactory = new WebApplicationFactory<Program>();
            var client = appFactory.CreateClient();

            var device = new Device
            {
                IsActive = true,
                Firmware = new string('X', 300), // overly long string
                MAC = "22:33:44:55:66:77"
            };

            var json = JsonSerializer.Serialize(device);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/AddDevice", content);
            var responseText = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            Assert.That(responseText, Does.Contain("Error while trying to add new device!"));
        }

        public class NullConnectionDbService : DBService
        {
            public NullConnectionDbService() : base(new ConfigurationBuilder().Build())
            {
                typeof(DBService)
                    .GetField("_connection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .SetValue(this, null);
            }
        }

        [Test]
        public void GetAllDevices_ShouldReturnEmptyList_WhenTableIsEmpty()
        {
            var dbService = GetDbService();

            var existing = dbService.GetAllDevices();
            foreach (var d in existing)
            {
                dbService.DeleteDevice(d.ID.Value);
            }

            var result = dbService.GetAllDevices();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddDevice_ShouldThrow_WhenMacIsNull()
        {
            var dbService = GetDbService();
            var device = new Device { IsActive = true, Firmware = "1.0", MAC = null };
            Assert.Throws<MySqlException>(() => dbService.AddDevice(device));
        }

        [Test]
        public void AddDevice_ShouldThrow_WhenFirmwareTooLong()
        {
            var dbService = GetDbService();
            var device = new Device
            {
                IsActive = true,
                Firmware = new string('X', 1000),
                MAC = "01:02:03:04:05:06"
            };

            Assert.Throws<MySqlException>(() => dbService.AddDevice(device));
        }

        [Test]
        public void RemoveDeviceFromGivenRoom_MockUp()
        {
            var dbService = GetDbService();
            var deviceData = new DeviceData
            {
                ID = 1,
                RoomName = "LectureRoom1",
                Temperature = 17f,
                Humidity = 10f,
                Radon = 2f,
                PPM = 20f,
                AirQuality = 30,
                DeviceID = 1,
                TimeStamp = DateTime.Now
            };

            Assert.Throws<MySqlException>(() => dbService.DeleteDevice(deviceData.DeviceID));
        }

        [Test]
        public void DeleteDevice_ShouldReturnFalse_WhenConnectionIsNull()
        {
            var dbService = new NullConnectionDbService();
            var result = dbService.DeleteDevice(1);
            Assert.That(result, Is.False);
        }

        [Test]
        public void AddDevice_ShouldReturnFalse_WhenConnectionIsNull()
        {
            var dbService = new NullConnectionDbService();
            var device = new Device { IsActive = true, Firmware = "1.0", MAC = "11:22:33:44:55:66" };
            var result = dbService.AddDevice(device);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetAllDevices_ShouldReturnNull_WhenConnectionIsNull()
        {
            var dbService = new NullConnectionDbService();
            var result = dbService.GetAllDevices();
            Assert.That(result, Is.Null);
        }

        [Test]
        public void AddDevice_ShouldThrow_WhenMacIsEmpty()
        {
            var dbService = GetDbService();
            var device = new Device { IsActive = true, Firmware = "1.0", MAC = "" };
            Assert.Throws<MySqlException>(() => dbService.AddDevice(device));
        }

        [Test]
        public void AddDevice_ShouldThrow_WhenFirmwareIsNull()
        {
            var dbService = GetDbService();
            var device = new Device { IsActive = true, Firmware = null, MAC = "00:11:22:33:44:55" };
            Assert.Throws<MySqlException>(() => dbService.AddDevice(device));
        }

        [Test]
        public void AddDevice_ShouldThrow_WhenDeviceIsNull()
        {
            var dbService = GetDbService();
            Assert.Throws<NullReferenceException>(() => dbService.AddDevice(null));
        }

        [Test]
        public void DeleteDevice_ShouldThrow_WhenIdIsNegative()
        {
            var dbService = GetDbService();
            Assert.DoesNotThrow(() =>
            {
                var result = dbService.DeleteDevice(-12345);
                Assert.That(result, Is.False);
            });
        }

        [Test]
        public void DeleteDevice_ShouldReturnFalse_WhenNoRowsAffected()
        {
            var dbService = GetDbService();

            // Try a large number unlikely to exist
            var result = dbService.DeleteDevice(999999);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetAllDevices_ShouldIncludeInsertedDevice()
        {
            var dbService = GetDbService();

            var newDevice = new Device
            {
                IsActive = true,
                Firmware = "vTest",
                MAC = $"00:00:{Guid.NewGuid().ToString("N").Substring(0, 10)}"
            };

            dbService.AddDevice(newDevice);

            var devices = dbService.GetAllDevices();
            Assert.That(devices.Any(d => d.MAC == newDevice.MAC), Is.True);
        }

        [Test]
        public void AddDevice_ShouldSucceed_WithMinimalValidData()
        {
            var dbService = GetDbService();
            var device = new Device
            {
                IsActive = false,
                Firmware = "0.0.1",
                MAC = "11:11:11:11:11:11"
            };

            var result = dbService.AddDevice(device);
            Assert.That(result, Is.True);
        }
    }
}