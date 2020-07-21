using AssertHelper.Utils.Cache;
using System;
using System.Linq;
using System.Reflection;

namespace pillont.LoggerInterceptors.Logic.CollectAttributes
{
    /// <summary>
    /// cache to collect attribute about method
    /// </summary>
    internal class AttributeCache : BaseReflectionCache<MethodInfo, AttributeCollection>
    {
        /// <summary>
        /// object holder the method
        /// </summary>
        public object Target { get; set; }

        private MethodAttributeCollector MethodAttributeCollector { get; }
        private ParameterAttributeCollector ParameterAttributeCollector { get; }

        public AttributeCache()
        {
            MethodAttributeCollector = new MethodAttributeCollector();
            ParameterAttributeCollector = new ParameterAttributeCollector();
        }

        protected override AttributeCollection CollectToPopulateCache(MethodInfo method)
        {
            Type[] parameterTypes = method.GetParameters()
                                            .Select(paramInfo => paramInfo.ParameterType)
                                            .ToArray();

            // NOTE : search also in implementation signature
            var currentMethod = Target.GetType()
                                        .GetMethod(method.Name, parameterTypes);

            var currentParamAttr = ParameterAttributeCollector.ParamAssertsCollect(currentMethod);
            var interfaceParamAttr = ParameterAttributeCollector.ParamAssertsCollect(method);
            var currentMethodAttributes = MethodAttributeCollector.CollectMethodAttr(currentMethod);
            var interfaceMethodAttributes = MethodAttributeCollector.CollectMethodAttr(method);

            return new AttributeCollection()
            {
                CurrentMethod = currentMethod,
                InterfaceMethod = method,
                CurrentMethodAttributes = currentMethodAttributes,
                InterfaceMethodAttributes = interfaceMethodAttributes,
                InterfaceParamAttr = interfaceParamAttr,
                CurrentParamAttr = currentParamAttr,
            };
        }
    }
}
