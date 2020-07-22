using System;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;

namespace pillont.LoggerInterceptors.Logic
{
    public abstract class BaseLogObserver : IObserver<BaseLogContext>
    {
        void IObserver<BaseLogContext>.OnCompleted()
        { }

        void IObserver<BaseLogContext>.OnError(Exception error)
        { }

        public abstract void OnErrorLog(ErrorLogContext error);

        void IObserver<BaseLogContext>.OnNext(BaseLogContext value)
        {
            {
                if (value is StartLogContext start)
                    OnStartFunction(start);

                if (value is ErrorLogContext error)
                    OnErrorLog(error);

                if (value is VoidResultLogContext voidResult)
                    OnVoidEndFunction(voidResult);

                if (value is ResultLogContext result)
                    OnResultEndFunction(result);
            }
        }

        public abstract void OnResultEndFunction(ResultLogContext result);

        public abstract void OnStartFunction(StartLogContext start);

        public abstract void OnVoidEndFunction(VoidResultLogContext voidResult);
    }
}