using System;
using Castle.DynamicProxy;
using pillont.LoggerInterceptors.Logic.Notify;

namespace pillont.LoggerInterceptors.Factory
{
    public class LoggerProxyFactory
    {
        public event Action<string, Exception> OnError;

        public event Action<string> OnLog;

        public T CreateForClass<T>(T target) where T : class
        {
            return new ProxyGenerator()
                               .CreateClassProxyWithTarget<T>(target, new LoggerInterceptor(OnLogFnc, OnErrorFnc));
        }

        public T CreateForInterface<T>(T target) where T : class
        {
            return new ProxyGenerator()
                               .CreateInterfaceProxyWithTarget<T>(target, new LoggerInterceptor(OnLogFnc, OnErrorFnc));
        }

        private void OnErrorFnc(string str, Exception e)
        {
            if (OnError == null)
            {
                throw new InvalidOperationException("Thanks to subscribe error log action on LoggerProxyFactory");
            }

            OnError.Invoke(str, e);
        }

        private void OnLogFnc(string str)
        {
            if (OnLog == null)
            {
                throw new InvalidOperationException("Thanks to subscribe log action on LoggerProxyFactory");
            }

            OnLog.Invoke(str);
        }
    }
}