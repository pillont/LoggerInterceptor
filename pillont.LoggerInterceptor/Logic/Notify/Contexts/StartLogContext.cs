using System.Reflection;

namespace pillont.LoggerInterceptors.Logic.Notify.Contexts
{
    public class StartLogContext : BaseLogContext
    {
        public ParameterInfo[] Parameters { get; set; }
        public object[] Values { get; set; }
    }
}