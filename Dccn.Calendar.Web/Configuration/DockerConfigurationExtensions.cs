using System;
using Microsoft.Extensions.Configuration;

namespace Dccn.Calendar.Web.Configuration
{
    public static class DockerConfigurationExtensions
    {
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder)
        {
            if (IsRunningInContainer())
            {
                builder.AddJsonFile("/run/secrets/secrets.json", true);
            }

            return builder;
        }

        private static bool IsRunningInContainer()
        {
            var value = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
            return !string.Equals(value, "0") || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}