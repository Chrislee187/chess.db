using System;
using Microsoft.AspNetCore.Http;

namespace chess.db.webapi.Middleware
{
    public class GlobalExceptionOptions
    {
        public Action<HttpContext, Exception, PublicErrorDetails> AddResponseDetails { get; set; }
    }
}