using Infrastructure.Exceptions;
using System;

namespace Infrastructure
{
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message) : base(message)
        {
            HttpStatus = System.Net.HttpStatusCode.BadRequest;
            logLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
        }
    }
}
