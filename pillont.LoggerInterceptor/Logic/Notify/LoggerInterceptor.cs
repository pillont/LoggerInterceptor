﻿using System;
using System.Reactive.Subjects;
using Castle.DynamicProxy;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;

namespace pillont.LoggerInterceptors.Logic.Notify
{
    internal class LoggerInterceptor : IInterceptor
    {
        public ISubject<BaseLogContext> LogSubject { get; }

        public LoggerInterceptor(ISubject<BaseLogContext> logSubject)
        {
            LogSubject = logSubject ?? throw new ArgumentNullException(nameof(logSubject));
        }

        public void Intercept(IInvocation invocation)
        {
            var service = new LogProxyService(LogSubject)
            {
                Target = invocation.InvocationTarget
            };

            service.ApplyCallLogs(invocation);

            try
            {
                invocation.Proceed();
                service.ApplyResultLogs(invocation);
            }
            catch (Exception e)
            {
                service.ApplyErrorLogs(invocation, e);
                throw;
            }
        }
    }
}