using System.Reflection;
using pillont.LoggerInterceptors.Factory;

namespace pillont.LoggerInterceptors.Logic.Notify.Contexts
{
    public class BaseLogContext
    {
        public LogAttribute Attribute { get; set; }
        public object CalledObject { get; set; }
        public MethodInfo Method { get; set; }
    }
}