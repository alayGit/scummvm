using ManagedCommon.Enums.Logging;
using ManagedCommon.Interfaces;
using System.Diagnostics;
using System.Threading.Tasks;

internal abstract class LoggingInterceptor<T>: Ice.DispatchInterceptor where T : Ice.Object
{
    internal T Servant { get; private set; }
    protected ILogger Logger { get; private set; }

    internal LoggingInterceptor(T servant, ILogger logger)
    {
        Servant = servant;
        Logger = logger;
    }

    public override Task<Ice.OutputStream> dispatch(Ice.Request request)
    {
        Task<Ice.OutputStream> task = Servant.ice_dispatch(request);

        task?.ContinueWith(
            t =>
            {
                if (t.IsFaulted)
                {
                    LogMessage(t.Exception?.ToString());
                }
            }
        );

        return task;

    }

    protected abstract void LogMessage(string error);

}
