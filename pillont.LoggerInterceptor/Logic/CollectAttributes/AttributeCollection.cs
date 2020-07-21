using AssertHelper.Attributes;
using pillont.LoggerInterceptors.Factory;
using System.Collections.Generic;
using System.Reflection;

namespace pillont.LoggerInterceptors.Logic.CollectAttributes
{
    internal class AttributeCollection
    {
        /// <summary>
        /// method of the implementing object
        /// </summary>
        public MethodInfo CurrentMethod { get; internal set; }

        /// <summary>
        /// attribute on the method of the implementing object
        /// </summary>
        public IEnumerable<LogAttribute> CurrentMethodAttributes { get; set; }

        /// <summary>
        /// attribute on the params of the method in the implementing object
        /// </summary>
        public Dictionary<LogAttribute, ParameterInfo> CurrentParamAttr { get; internal set; }

        /// <summary>
        /// method of the interface
        /// </summary>
        public MethodInfo InterfaceMethod { get; internal set; }

        /// <summary>
        /// attribute on the method of the interface
        /// </summary>
        public IEnumerable<LogAttribute> InterfaceMethodAttributes { get; set; }

        /// <summary>
        /// attribute on the params of the method in the interface
        /// </summary>
        public Dictionary<LogAttribute, ParameterInfo> InterfaceParamAttr { get; internal set; }
    }
}