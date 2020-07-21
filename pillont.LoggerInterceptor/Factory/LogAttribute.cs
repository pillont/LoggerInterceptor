using System;

namespace pillont.LoggerInterceptors.Factory
{
    /// <summary>
    /// base class to apply assert in method signature
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class LogAttribute : Attribute
    {
        public LogAction Action { get; }

        /// <summary>
        /// Message to show during assert fail
        /// </summary>
        public string Message { get; set; }

        public LogAttribute(LogAction action = LogAction.All)
        {
            Action = action;
        }
    }
}