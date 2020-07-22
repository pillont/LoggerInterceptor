using System;

namespace pillont.LoggerInterceptors.Factory
{
    /// <summary>
    /// base class to apply assert in method signature
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class LogAttribute : Attribute
    {
        public LogAction Action { get; set; } = LogAction.All;

        public string EnterMessage { get; set; }
        public string ErrorMessage { get; set; }
        public string ExitMessage { get; set; }
    }
}