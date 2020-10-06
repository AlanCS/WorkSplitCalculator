using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection ThrowBadRequestOnBadModelValidation(this IServiceCollection services)
        {
            // copied from https://stackoverflow.com/questions/51145243/how-do-i-customize-asp-net-core-model-binding-errors

            return services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = actionContext =>
                {
                    throw new BadRequestException(actionContext.ModelState.GetMessage());
                };
            });
        }

        public static string JoinWith(this IEnumerable<string> list, string separator = ", ")
        {
            return string.Join(separator, list);
        }

        private static string GetMessage(this ModelStateDictionary modelState)
        {
            // copied and adapted from https://stackoverflow.com/questions/2845852/asp-net-mvc-how-to-convert-modelstate-errors-to-json

            return modelState
                .Where(c => c.Value.HasError())
                .Select(c => c.Value.GetErrorMessages())
                .JoinWith(" ");
        }

        private static bool HasError(this ModelStateEntry modelStateEntry)
        {
            return modelStateEntry.Errors != null && modelStateEntry.Errors.Any();
        }

        private static string GetErrorMessages(this ModelStateEntry modelStateEntry)
        {
            return modelStateEntry.Errors
                .Select(e => e.Exception?.Message ?? e.ErrorMessage)
                .JoinWith();
        }

    }
}
