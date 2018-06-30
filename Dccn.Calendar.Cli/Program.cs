using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Dccn.Calendar.Cli
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        public static async Task MainAsync(string[] args)
        {
            var options = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .Build()
                .Get<CalendarOptions>();

            var outputPath = options.OutputPath;

            var client = new CalendarClient(options.ExchangeUrl, options.Username, options.Password);
            if (options.MaxEvents.HasValue)
            {
                client.MaxEvents = options.MaxEvents.Value;
            };

            var today = DateTime.Today;
            var period = TimeSpan.FromDays(1);

            using (var writer = outputPath == null ? Console.Out : new StreamWriter(outputPath))
            {
                foreach (var entry in options.Calendars)
                {
                    var calendarKey = entry.Key;
                    var calendarId = entry.Value;

                    var (success, calendar) = await client.TryGetCalendarByIdAsync(calendarId);
                    if (!success)
                    {
                        Console.Error.WriteLine($"Warning: Could not find calendar for {calendarKey}. Skipped.");
                        continue;
                    }

                    Console.Error.WriteLine($"Retrieving events for '{calendar.Name}' ({calendarKey}).");
                    var events = await calendar.EventsRangeAsync(today, period);

                    foreach (var @event in events)
                    {
                        writer.WriteLine("{0} {1} {2} {3}",
                            @event.UnixTimestamp,
                            calendarKey,
                            @event.DurationMillis,
                            @event.Subject);
                    }
                }
            }
        }
    }
}