using System;
using Castle.DynamicProxy;

namespace pillont.LoggerInterceptors.Logic.Notify
{
    internal class LoggerInterceptor : IInterceptor
    {
        public Action<string, Exception> OnError { get; }
        public Action<string> OnLog { get; }

        public LoggerInterceptor(Action<string> onLog, Action<string, Exception> onError)
        {
            OnLog = onLog ?? throw new ArgumentNullException(nameof(onLog));
            OnError = onError ?? throw new ArgumentNullException(nameof(onError));
        }

        public void Intercept(IInvocation invocation)
        {
            var service = new LogProxyService(OnLog, OnError)
            {
                Target = invocation.InvocationTarget
            };

            service.ApplyCallLogs(invocation.Method, invocation.Arguments);

            try
            {
                invocation.Proceed();
                service.ApplyResultLogs(invocation.Method, invocation.ReturnValue);
            }
            catch (Exception e)
            {
                service.ApplyErrorLogs(invocation.Method, e);
                throw;
            }
        }
    }
}