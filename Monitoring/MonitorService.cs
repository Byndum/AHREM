using Serilog;
using OpenTelemetry.Trace;
using Serilog.Core;
using System.Reflection;
using System.Diagnostics;
using OpenTelemetry;

namespace Monitoring
{
    public static class MonitorService
    {
        public static ILogger Log => Serilog.Log.Logger;
        /*
        public static readonly string ServiceName = Assembly.GetCallingAssembly().GetName().Name ?? "UnknownService";
        public static TracerProvider? TracerProvider; // Fixed declaration to allow assignment  
        public static ActivitySource ActivitySource = new ActivitySource(ServiceName, "1.0.0");
        */
        static MonitorService()
        {
            Log.Information("MonitorService initializing...");
            /*
            // OpenTelemetry  
            TracerProvider = Sdk.CreateTracerProviderBuilder() // Fixed method call to use Sdk.CreateTracerProviderBuilder  
                .AddConsoleExporter()
                .AddZipkinExporter(config =>
                {
                    config.Endpoint = new Uri("http://localhost:9411");
                })
                .AddSource(ActivitySource.Name) // Fixed to use the correct ActivitySource name  
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName)) // Fixed to use correct ServiceName  
                .Build();
            */
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }
    }
}
