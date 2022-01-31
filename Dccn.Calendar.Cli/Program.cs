using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Dccn.Calendar.Cli
{
    [UsedImplicitly]
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var options = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .Build()
                .Get<CalendarOptions>();

            var client = new CalendarClient(options.ExchangeUrl, options.Username, options.Password);
            var calendars = (await client.ListCalendarsAsync()).OrderBy(c => c.Name).ToList();
            using (var writer = new JsonTextWriter(Console.Out))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;

                await writer.WriteStartObjectAsync();
                foreach (var calendar in calendars)
                {
                    if (!string.IsNullOrWhiteSpace(options.Filter) && !calendar.Name.Contains(options.Filter, StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }

                    await writer.WritePropertyNameAsync(Regex.Replace(calendar.Name, "\\W+", "_").Trim('_'));

                    await writer.WriteStartObjectAsync();

                    await writer.WritePropertyNameAsync("Name");
                    await writer.WriteValueAsync(calendar.Name);

                    await writer.WritePropertyNameAsync("Location");
                    await writer.WriteNullAsync();

                    await writer.WritePropertyNameAsync("Source");
                    if (options.Source != null)
                    {
                        await writer.WriteValueAsync(options.Source);
                    }
                    else
                    {
                        await writer.WriteNullAsync();
                    }

                    await writer.WritePropertyNameAsync("ExchangeId");
                    await writer.WriteValueAsync(calendar.Id);

                    await writer.WritePropertyNameAsync("ShowInTodayOverview");
                    await writer.WriteValueAsync(false);

                    await writer.WriteEndObjectAsync();
                }

                await writer.WriteEndObjectAsync();
            }

            Console.ReadLine();
        }
    }
}