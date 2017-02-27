using System;

using Microsoft.AspNetCore.Http;

using Guru.Middleware.Abstractions;

namespace Guru.Middleware.Errors
{
    public class CallingContext : ICallingContext
    {
        public CallingContext(HttpContext context, Exception exception)
        {
            Context = context;
            Exception = exception;
        }

        public HttpContext Context { get; private set; }

        public Exception Exception { get; private set; }
    }
}