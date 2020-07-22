using System;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using pillont.LoggerInterceptors.Factory;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;

namespace pillont.LoggerInterceptors.Logic.Notify
{
    internal class LogAttributeService
    {
        public ISubject<BaseLogContext> LogSubject { get; }

        public LogAttributeService(ISubject<BaseLogContext> logSubject)
        {
            LogSubject = logSubject ?? throw new ArgumentNullException(nameof(logSubject));
        }

        internal void ApplyErrorLogs(MethodInfo method, Exception e, LogAttribute attr)
        {
            var ctx = new ErrorLogContext()
            {
                Method = method,
                Exception = e,
                Attribute = attr
            };

            LogSubject.OnNext(ctx);
        }

        internal void ApplyLogOnResult(MethodInfo method, object returnValue, LogAttribute attr)
        {
            if (returnValue is Task taskResult)
            {
                taskResult.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        ApplyErrorLogs(method, t.Exception, attr);
                        return;
                    }

                    Type resultType = method.ReturnType;
                    var resultProp = resultType.GetProperty("Result");
                    if (resultProp is null)
                    {
                        // CASE : TASK without result
                        NotifyVoidResultLogs(method, attr);
                        return;
                    }

                    // CASE : TASK with result
                    var result = resultProp.GetGetMethod().Invoke(t, null);
                    NotifyResultLogs(method, result, attr);
                });
                return;
            }

            if (method.ReturnType == typeof(void))
            {
                // CASE : synchrone void function
                NotifyVoidResultLogs(method, attr);
                return;
            }
            // CASE : synchrone result function
            NotifyResultLogs(method, returnValue, attr);
        }

        internal void ApplyLogOnValue(LogAttribute attr, MethodInfo method, object[] allParameters)
        {
            if (!attr.Action.HasFlag(LogAction.OnCall))
            {
                return;
            }

            var allParams = method.GetParameters();

            var ctx = new StartLogContext()
            {
                Method = method,
                Attribute = attr,
                Parameters = allParams,
                Values = allParameters,
            };

            LogSubject.OnNext(ctx);
        }

        private void NotifyResultLogs(MethodInfo method, object result, LogAttribute attr)
        {
            var ctx = new ResultLogContext()
            {
                Method = method,
                Attribute = attr,
                Result = result,
            };

            LogSubject.OnNext(ctx);
        }

        private void NotifyVoidResultLogs(MethodInfo method, LogAttribute attr)
        {
            var ctx = new VoidResultLogContext()
            {
                Method = method,
                Attribute = attr,
            };

            LogSubject.OnNext(ctx);
        }
    }
}