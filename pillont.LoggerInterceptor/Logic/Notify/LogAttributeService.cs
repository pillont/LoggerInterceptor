using System;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
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

        internal void ApplyErrorLogs(IInvocation invocation, Exception e, LogAttribute attr)
        {
            var ctx = new ErrorLogContext()
            {
                Method = invocation.Method,
                Exception = e,
                Attribute = attr,
                CalledObject = invocation.Proxy,
            };

            LogSubject.OnNext(ctx);
        }

        internal void ApplyLogOnResult(IInvocation invocation, LogAttribute attr)
        {
            if (invocation.ReturnValue is Task taskResult)
            {
                taskResult.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        ApplyErrorLogs(invocation, t.Exception, attr);
                        return;
                    }

                    Type resultType = invocation.Method.ReturnType;
                    var resultProp = resultType.GetProperty("Result");
                    if (resultProp is null)
                    {
                        // CASE : TASK without result
                        NotifyVoidResultLogs(invocation, attr);
                        return;
                    }

                    // CASE : TASK with result
                    var result = resultProp.GetGetMethod().Invoke(t, null);
                    NotifyResultLogs(invocation, attr, result);
                });
                return;
            }

            if (invocation.Method.ReturnType == typeof(void))
            {
                // CASE : synchrone void function
                NotifyVoidResultLogs(invocation, attr);
                return;
            }
            // CASE : synchrone result function
            NotifyResultLogs(invocation, attr);
        }

        internal void ApplyLogOnValue(IInvocation invocation, LogAttribute attr)
        {
            if (!attr.Action.HasFlag(LogAction.OnCall))
            {
                return;
            }

            var ctx = new StartLogContext()
            {
                CalledObject = invocation.Proxy,
                Method = invocation.Method,
                Attribute = attr,
                Arguments = invocation.Arguments
            };

            LogSubject.OnNext(ctx);
        }

        /// <param name="result">
        /// override default result
        /// used to collect Task result
        /// </param>
        private void NotifyResultLogs(IInvocation invocation, LogAttribute attr, object result = null)
        {
            var ctx = new ResultLogContext()
            {
                CalledObject = invocation.Proxy,
                Method = invocation.Method,
                Attribute = attr,
                Result = result ?? invocation.ReturnValue,
            };

            LogSubject.OnNext(ctx);
        }

        private void NotifyVoidResultLogs(IInvocation invocation, LogAttribute attr)
        {
            var ctx = new VoidResultLogContext()
            {
                CalledObject = invocation.Proxy,
                Method = invocation.Method,
                Attribute = attr,
            };

            LogSubject.OnNext(ctx);
        }
    }
}