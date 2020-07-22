using System;
using System.Reactive.Subjects;
using Castle.DynamicProxy;
using pillont.LoggerInterceptors.Logic.Notify;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;

namespace pillont.LoggerInterceptors.Factory
{
    public class LoggerProxyFactory
    {
        public ISubject<BaseLogContext> LogSubject { get; }

        public LoggerProxyFactory(ISubject<BaseLogContext> subject)
        {
            LogSubject = subject;
        }

        public T CreateForClass<T>(T target) where T : class
        {
            return new ProxyGenerator()
                               .CreateClassProxyWithTarget<T>(target, new LoggerInterceptor(LogSubject));
        }

        public T CreateForInterface<T>(T target) where T : class
        {
            return new ProxyGenerator()
                               .CreateInterfaceProxyWithTarget<T>(target, new LoggerInterceptor(LogSubject));
        }
    }
}