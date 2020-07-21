using System;

namespace pillont.LoggerInterceptors.Factory
{
    [Flags]
    public enum LogAction
    {
        None = 0,

        OnCall = 1 << 0,
        OnEnd = 1 << 1,

        All = ~0,
    }
}