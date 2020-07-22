using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;

namespace pillont.LoggerInterceptors.Logic
{
    public class LogInjector : ILogInjector
    {
        public const string ArgumentsKey = "{{ARGUMENTS}}";
        public const string CalledObjectKey = "{{CALLED_OBJECT}}";
        public const string ExceptionKey = "{{EXCEPTION}}";
        public const string MethodNameKey = "{{METHOD_NAME}}";
        public const string ResultKey = "{{RESULT}}";

        public StringBuilder Inject(string message, ErrorLogContext error)
        {
            var builder = BaseInject(message, error);
            builder.Replace(ExceptionKey, JsonSerializer.Serialize(error.Exception));
            return builder;
        }

        public StringBuilder Inject(string message, ResultLogContext result)
        {
            var builder = BaseInject(message, result);
            builder.Replace(ResultKey, JsonSerializer.Serialize(result.Result));
            return builder;
        }

        public StringBuilder Inject(string message, StartLogContext start)
        {
            var builder = BaseInject(message, start);

            var paramsStrList = new List<string>();
            int index = 0;

            foreach (var p in start.Method.GetParameters())
            {
                var value = start.Arguments[index];

                paramsStrList.Add($"{p.Name} : {JsonSerializer.Serialize(value)}");
                index++;
            }

            if (paramsStrList.Any())
            {
                var paramsStr = string.Join(",", paramsStrList);
                builder.Replace(ArgumentsKey, $"[ {paramsStr} ]");
            }

            return builder;
        }

        public StringBuilder Inject(string message, VoidResultLogContext result)
        {
            var builder = BaseInject(message, result);
            return builder;
        }

        private StringBuilder BaseInject(string message, BaseLogContext context)
        {
            var builder = new StringBuilder(message);
            builder.Replace(MethodNameKey, context.Method.Name);
            builder.Replace(CalledObjectKey, JsonSerializer.Serialize(context.CalledObject));

            return builder;
        }
    }
}