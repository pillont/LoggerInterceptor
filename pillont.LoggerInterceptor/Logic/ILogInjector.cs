using System.Text;
using pillont.LoggerInterceptors.Logic.Notify.Contexts;

namespace pillont.LoggerInterceptors.Logic
{
    public interface ILogInjector
    {
        StringBuilder Inject(string message, ErrorLogContext error);

        StringBuilder Inject(string message, ResultLogContext result);

        StringBuilder Inject(string message, StartLogContext start);

        StringBuilder Inject(string message, VoidResultLogContext result);
    }
}