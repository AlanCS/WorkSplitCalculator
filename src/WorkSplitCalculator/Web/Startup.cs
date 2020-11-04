using BizCover.Api.Cars.Infrastructure;
using Boilerplate.Infrastructure.Extensions;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;

namespace Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services
                .AddControllers();

            services
                .ThrowBadRequestOnBadModelValidation()
                .AddHealthChecks();

            // Add S3 to the ASP.NET Core dependency injection framework.
            //services.AddAWSService<Amazon.S3.IAmazonS3>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionFilter>();

            app.UseRewriter(new RewriteOptions().AddRewrite("^$", "healthcheck", true));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.AddCustomHealthCheck(env);
                endpoints.MapControllers();
            });
        }
    }
}
