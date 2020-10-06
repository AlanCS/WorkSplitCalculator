using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Healthcheck
{
    public static class Extensions
    {

        private static string EnvName;
        private static DateTime DateStarted = DateTime.Now;
        private static string ServerName = Environment.MachineName;
        private static DateTime? DateLastRun = null;
        private static string LastReport = null;

        public static void AddCustomHealthCheck(this IEndpointRouteBuilder endpoint, IWebHostEnvironment env)
        {
            EnvName = env.EnvironmentName;

            endpoint.MapHealthChecks("/healthcheck", new HealthCheckOptions()
            {
                ResponseWriter = WriteResponse
            });
        }

        /// <summary>

        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static Task WriteResponse(HttpContext context, HealthReport result)
        {
            // copied from https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-3.1
            // then adapted to add app name and version

            context.Response.ContentType = "application/json; charset=utf-8";

            if(!DateLastRun.HasValue || DateTime.Now >= DateTime.Now.AddMinutes(-5))
                LastReport = WriteReport(result);
            
            return context.Response.WriteAsync(LastReport);
        }

        private static string WriteReport(HealthReport result)
        {
            var options = new JsonWriterOptions
            {
                Indented = true
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    var assembly = Assembly.GetEntryAssembly().GetName();

                    writer.WriteStartObject();
                    writer.WriteString("name", assembly.Name);
                    writer.WriteString("environment", EnvName);
                    writer.WriteString("status", result.Status.ToString());
                    writer.WriteString("version", assembly.Version.ToString()); // this allows us to check if new versions were really deployed
                    writer.WriteString("started", DateStarted.ToString("s"));
                    writer.WriteString("machine", ServerName);
                    writer.WriteStartObject("results");
                    foreach (var entry in result.Entries)
                    {
                        writer.WriteStartObject(entry.Key);
                        writer.WriteString("status", entry.Value.Status.ToString());
                        writer.WriteString("description", entry.Value.Description);
                        writer.WriteStartObject("data");
                        foreach (var item in entry.Value.Data)
                        {
                            writer.WritePropertyName(item.Key);
                            JsonSerializer.Serialize(
                                writer, item.Value, item.Value?.GetType() ??
                                typeof(object));
                        }
                        writer.WriteEndObject();
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                var json = Encoding.UTF8.GetString(stream.ToArray());
                return json;
            }            
        }
    }
}
