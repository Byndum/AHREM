using AHREM_API.Models;
using AHREM_API.Services;
using Microsoft.Extensions.Configuration;

namespace AHREM.API.Tests
{
    public class Tests
    {
        private DBService _dbService;

        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Variables.json")
                .Build();

            _dbService = new DBService(config);
        }

        [Test]
        public void GetAllDevices_ShouldReturnList()
        {
            // Act  
            var devices = _dbService.GetAllDevices();

            // Assert  
            Assert.That(devices, Is.Not.Null); // Updated to use Assert.That with Is.Not.Null  
            Assert.That(devices, Is.InstanceOf<List<Device>>()); // Updated to use Assert.That with Is.InstanceOf  
                                                                 // You can add more asserts depending on your test DB contents  
        }
    }
}
