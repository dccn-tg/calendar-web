﻿using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Dccn.Calendar.Web.Configuration
{
    public static class DockerConfigurationExtensions
    {
        public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder, string fileName = "secrets.json")
        {
            if (IsRunningInContainer())
            {
                builder.AddJsonFile(Path.Combine("/run/secrets", fileName), true);
            }

            return builder;
        }

        private static bool IsRunningInContainer()
        {
            var value = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
            return value != null && (value == "1" || value == "true");
        }
    }
}