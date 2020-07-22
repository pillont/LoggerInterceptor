using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Castle.DynamicProxy;
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

        public void ApplyCallLogs(IInvocation invocation)
        {
            AttributeCollection attributeCollection = AttributeCache.CollectWithCache(invocation.Method);
            MethodInfo currentMethod = attributeCollection.CurrentMethod;
            MethodInfo interfaceMethod = attributeCollection.InterfaceMethod;

            ApplyLogOnMethod(invocation, attributeCollection.InterfaceMethodAttributes);

            if (currentMethod != interfaceMethod)
            {
                ApplyLogOnMethod(invocation, attributeCollection.CurrentMethodAttributes);
            }
        }

        internal void ApplyErrorLogs(IInvocation invocation, Exception e)
        {
            AttributeCollection attributeCollection = AttributeCache.CollectWithCache(invocation.Method);
            var attribute = attributeCollection.CurrentMethodAttributes
                                    .Union(
                                        attributeCollection.InterfaceMethodAttributes)
                                    .Distinct()
                                    .FirstOrDefault();
            Service.ApplyErrorLogs(invocation, e, attribute);
        }

        internal void ApplyResultLogs(IInvocation invocation)
        {
            AttributeCollection attributeCollection = AttributeCache.CollectWithCache(invocation.Method);
            MethodInfo currentMethod = attributeCollection.CurrentMethod;
            MethodInfo interfaceMethod = attributeCollection.InterfaceMethod;

            var attribute = attributeCollection.InterfaceMethodAttributes.FirstOrDefault(at => at.Action.HasFlag(LogAction.OnEnd));
            if (attribute != null)
            {
                Service.ApplyLogOnResult(invocation, attribute);
                return;
            }

            attribute = attributeCollection.CurrentMethodAttributes.FirstOrDefault(attr => attr.Action.HasFlag(LogAction.OnEnd));
            if (currentMethod == interfaceMethod
            || attribute == null)
                return;

            Service.ApplyLogOnResult(invocation, attribute);
        }

        private void ApplyLogOnMethod(IInvocation invocation, IEnumerable<LogAttribute> allAttributes)
        {
            foreach (var attr in allAttributes)
            {
                Service.ApplyLogOnValue(invocation, attr);
            }
        }
    }
}