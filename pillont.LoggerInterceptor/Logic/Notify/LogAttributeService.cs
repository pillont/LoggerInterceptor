using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using pillont.LoggerInterceptors.Factory;

namespace pillont.LoggerInterceptors.Logic.Notify
{
    internal class LogAttributeService
    {
        public Action<string, Exception> OnError { get; }
        public Action<string> OnLog { get; }

        public LogAttributeService(Action<string> onLog, Action<string, Exception> onError)
        {
            OnLog = onLog ?? throw new ArgumentNullException(nameof(onLog));
            OnError = onError ?? throw new ArgumentNullException(nameof(onError));
        }

        internal void ApplyErrorLogs(MethodInfo method, Exception e)
        {
            var builder = GetBuilderForMethod(method);
            builder.Append($" an error applied");
            OnError.Invoke(builder.ToString(), e);
        }

        internal void ApplyLogOnResult(MethodInfo method, object returnValue)
        {
            if (returnValue is Task taskResult)
            {
                taskResult.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        ApplyErrorLogs(method, t.Exception);
                        return;
                    }

                    Type resultType = t.GetType();

                    var resultProp = resultType.GetProperty("Result");
                    if (resultProp is null)
                        return;

                    var result = resultProp.GetGetMethod().Invoke(t, null);
                    NotifyResultLog(method, result);
                });
                return;
            }

            NotifyResultLog(method, returnValue);
        }

        internal void ApplyLogOnValue(LogAttribute attr, MethodInfo method, object[] allParameters)
        {
            if (attr.Action.HasFlag(LogAction.OnCall))
            {
                var builder = GetBuilderForMethod(method);
                builder.Append(" was called");

                var allParams = method.GetParameters();
                if (allParams.Any())
                {
                    var allP = new List<string>();
                    int index = 0;
                    foreach (var p in allParams)
                    {
                        string value = JsonSerializer.Serialize(allParameters[index]);
                        allP.Add($"{p.Name} : {value}");
                        index++;
                    }

                    builder.Append($" with value : [ {string.Join(", ", allP)} ]");
                }

                OnLog(builder.ToString());
            }
        }

        private static StringBuilder GetBuilderForMethod(MethodInfo method)
        {
            return new StringBuilder($"{method.Name}");
        }

        private void NotifyResultLog(MethodInfo method, object returnValue)
        {
            var builder = GetBuilderForMethod(method);
            builder.Append($" return result : {JsonSerializer.Serialize(returnValue)}");
            OnLog(builder.ToString());
        }
    }
}