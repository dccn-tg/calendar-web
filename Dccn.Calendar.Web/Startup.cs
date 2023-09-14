using Dccn.Calendar.Web.Configuration;
using Dccn.Calendar.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dccn.Calendar.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddRazorPages();

            services.AddScoped<ICalendarService, CalendarService>();
            services.Configure<CalendarOptions>(Configuration.GetSection("CalendarService"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use((context, next) =>
            {
                if (!context.Request.Headers.TryGetValue("X-Forwarded-PathBase", out var pathBaseValues))
                {
                    return next();
                }

                var pathBase = new PathString(pathBaseValues);

                context.Request.PathBase = pathBase;
                if (context.Request.Path.StartsWithSegments(pathBase, out var pathRemainder))
                {
                    context.Request.Path = pathRemainder;
                }

                return next();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
