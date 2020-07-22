namespace pillont.LoggerInterceptors.Logic.Notify.Contexts
{
    public class ResultLogContext : BaseLogContext
    {
        public object Result { get; set; }
        public object CalledObject { get; internal set; }
    }
}