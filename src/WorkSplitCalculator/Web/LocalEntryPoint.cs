using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Web
{
    /// <summary>
    /// The Main function can be used to run the ASP.NET Core application locally using the Kestrel webserver.
    /// </summary>
    public class LocalEntryPoint
    {
        public static void Main(string[] args)
        {
            Build(args).Run();
        }

        public static IWebHost Build(string[] args) => CreateHostBuilder(args).Build();

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>();
    }
}
