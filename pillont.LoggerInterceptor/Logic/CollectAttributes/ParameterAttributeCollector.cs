using AssertHelper.Attributes;
using pillont.LoggerInterceptors.Factory;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pillont.LoggerInterceptors.Logic.CollectAttributes
{
    /// <summary>
    /// used to colled attribute on method parameters
    /// </summary>
    internal class ParameterAttributeCollector
    {
        public Dictionary<LogAttribute, ParameterInfo> ParamAssertsCollect(MethodInfo method)
        {
            return method.GetParameters()
                                    .SelectMany(param => param.GetCustomAttributes<LogAttribute>().Select(attr => new { Param = param, Attri = attr }))
                                    .Distinct()
                                    .ToDictionary(pair => pair.Attri, pair => pair.Param);
        }
    }
}