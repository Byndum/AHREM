using Serilog;
using Serilog.Core;

namespace Monitoring
{
    public static class MonitorService
    {
        public static ILogger Log => Serilog.Log.Logger;

        static MonitorService()
        {
            Log.Information("MonitorService initializing...");

            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }
    }
}
