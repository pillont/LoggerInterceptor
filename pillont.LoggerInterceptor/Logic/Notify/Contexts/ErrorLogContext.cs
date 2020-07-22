using System;

namespace pillont.LoggerInterceptors.Logic.Notify.Contexts
{
    public class ErrorLogContext : BaseLogContext
    {
        public Exception Exception { get; set; }
    }
}