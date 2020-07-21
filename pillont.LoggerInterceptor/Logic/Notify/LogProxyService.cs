using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using pillont.LoggerInterceptors.Factory;
using pillont.LoggerInterceptors.Logic.CollectAttributes;

namespace pillont.LoggerInterceptors.Logic.Notify
{
    /// <summary>
    /// service to apply logic of Log interceptor
    /// </summary>
    public class LogProxyService
    {
        /// <summary>
        /// object proxied
        /// flat pass of cache taget
        /// </summary>
        public object Target
        {
            get => AttributeCache.Target;
            set => AttributeCache.Target = value;
        }

        /// <summary>
        /// service to apply logic of Log
        /// </summary>
        internal LogAttributeService Service { get; }

        /// <summary>
        /// cache of method attributes
        /// collect attribut and store there to avoid collect each time
        /// </summary>
        private AttributeCache AttributeCache { get; }

        public LogProxyService(Action<string> onLog, Action<string, Exception> onError)
        {
            Service = new LogAttributeService(onLog, onError);
            AttributeCache = new AttributeCache();
        }

        public void ApplyCallLogs(MethodInfo method, object[] parameters)
        {
            AttributeCollection attributeCollection = AttributeCache.CollectWithCache(method);
            MethodInfo currentMethod = attributeCollection.CurrentMethod;
            MethodInfo interfaceMethod = attributeCollection.InterfaceMethod;

            ApplyLogOnMethod(attributeCollection.InterfaceMethod, attributeCollection.InterfaceMethodAttributes, parameters);

            if (currentMethod != interfaceMethod)
            {
                ApplyLogOnMethod(currentMethod, attributeCollection.CurrentMethodAttributes, parameters);
            }
        }

        internal void ApplyErrorLogs(MethodInfo method, Exception e)
        {
            Service.ApplyErrorLogs(method, e);
        }

        internal void ApplyResultLogs(MethodInfo method, object returnValue)
        {
            if (method.ReturnType == typeof(void))
                return;

            AttributeCollection attributeCollection = AttributeCache.CollectWithCache(method);
            MethodInfo currentMethod = attributeCollection.CurrentMethod;
            MethodInfo interfaceMethod = attributeCollection.InterfaceMethod;

            if (attributeCollection.InterfaceMethodAttributes.Any(attr => attr.Action.HasFlag(LogAction.OnEnd)))
            {
                Service.ApplyLogOnResult(method, returnValue);
                return;
            }

            if (currentMethod == interfaceMethod
            || !attributeCollection.CurrentMethodAttributes.Any(attr => attr.Action.HasFlag(LogAction.OnEnd)))
                return;

            Service.ApplyLogOnResult(method, returnValue);
        }

        private void ApplyLogOnMethod(MethodInfo method, IEnumerable<LogAttribute> allAttributes, object[] allParameters)
        {
            foreach (var attr in allAttributes)
            {
                Service.ApplyLogOnValue(attr, method, allParameters);
            }
        }
    }
}