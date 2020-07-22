using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using pillont.LoggerInterceptors.Factory;
using pillont.LoggerInterceptors.Logic.CollectAttributes;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;

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

        public LogProxyService(ISubject<BaseLogContext> logSubject)
        {
            Service = new LogAttributeService(logSubject);
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
            AttributeCollection attributeCollection = AttributeCache.CollectWithCache(method);
            var attribute = attributeCollection.CurrentMethodAttributes
                                    .Union(
                                        attributeCollection.InterfaceMethodAttributes)
                                    .Distinct()
                                    .FirstOrDefault();
            Service.ApplyErrorLogs(method, e, attribute);
        }

        internal void ApplyResultLogs(MethodInfo method, object returnValue)
        {
            AttributeCollection attributeCollection = AttributeCache.CollectWithCache(method);
            MethodInfo currentMethod = attributeCollection.CurrentMethod;
            MethodInfo interfaceMethod = attributeCollection.InterfaceMethod;

            var attribute = attributeCollection.InterfaceMethodAttributes.FirstOrDefault(at => at.Action.HasFlag(LogAction.OnEnd));
            if (attribute != null)
            {
                Service.ApplyLogOnResult(method, returnValue, attribute);
                return;
            }

            attribute = attributeCollection.CurrentMethodAttributes.FirstOrDefault(attr => attr.Action.HasFlag(LogAction.OnEnd));
            if (currentMethod == interfaceMethod
            || attribute == null)
                return;

            Service.ApplyLogOnResult(method, returnValue, attribute);
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