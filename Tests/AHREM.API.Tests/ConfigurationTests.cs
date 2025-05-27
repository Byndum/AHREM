using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace AHREM.API.Tests
{

    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void ShouldLoadRequiredConfigKey()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var allowedHosts = config["AllowedHosts"];
            Assert.That(allowedHosts, Is.EqualTo("*"), "Expected AllowedHosts to be '*' from config");
        }

        [Test]
        public void ShouldThrowIfRequiredJsonFileMissing()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("nonexistent.json", optional: false);

            Assert.Throws<FileNotFoundException>(() => {
                var config = builder.Build();
                var _ = config["AnyKey"];
            });
        }
    }
}
