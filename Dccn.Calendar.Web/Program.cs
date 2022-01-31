using Dccn.Calendar.Web.Configuration;
using JetBrains.Annotations;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Dccn.Calendar.Web
{
    [UsedImplicitly]
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        [UsedImplicitly]
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => builder.AddDockerSecrets())
                .UseStartup<Startup>();
        }
    }
}