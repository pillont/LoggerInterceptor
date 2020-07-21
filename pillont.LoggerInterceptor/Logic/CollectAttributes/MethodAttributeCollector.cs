using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using pillont.LoggerInterceptors.Factory;

namespace pillont.LoggerInterceptors.Logic.CollectAttributes
{
    /// <summary>
    /// used to colled attribute on method
    /// also used to collect attribute on properties
    /// </summary>
    internal class MethodAttributeCollector
    {
        /// <summary>
        /// prefix of properties's setter method
        /// </summary>
        private const string SetterPrefix = "set_";

        public List<LogAttribute> CollectMethodAttr(MethodInfo method)
        {
            string methodName = method.Name;
            bool IsNotSetterMethod = !methodName.StartsWith(SetterPrefix);

            return IsNotSetterMethod
                        ? CollectAttributesOnPropertySetter(method)
                        : CollectAttributesOnSampleMethod(method, methodName);
        }

        private List<LogAttribute> CollectAttributesOnPropertySetter(MethodInfo method)
        {
            return method.GetCustomAttributes<LogAttribute>()
                            .ToList();
        }

        private List<LogAttribute> CollectAttributesOnSampleMethod(MethodInfo method, string methodName)
        {
            var propName = methodName.Substring(SetterPrefix.Length);
            var allAttr = method.DeclaringType.GetProperty(propName).GetCustomAttributes<LogAttribute>()
                            .ToList();
            return allAttr;
        }
    }
}