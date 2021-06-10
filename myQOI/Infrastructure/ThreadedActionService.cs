using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using SpeedtestNetCli.Configurations;

namespace SpeedtestNetCli.Infrastructure
{
    public abstract class ThreadedActionService : IService, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger("Speedtest Application");

        private readonly object _syncRoot = new object();
        private bool _isRunning;
        private bool _isDisposed;
        public event EventHandler Aborted;

        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        private readonly SpeedtestConfiguration _speedtestConfiguration;

        protected ThreadedActionService(SpeedtestConfiguration speedtestConfiguration)
        {
            _speedtestConfiguration = speedtestConfiguration;
        }

        public void Start()
        {
            lock (_syncRoot)
            {
                CheckDisposed(ToString(), "Start()");
                TryRunningTask();
                Log.Debug("Started Start()");
            }
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                CheckDisposed(ToString(), "Stop()");
                Log.Info("Stopping...");
                TryCancellingTask();
                Log.Info("Stopped");
            }
        }

        protected void OnAbort(EventArgs e)
        {
            lock (_syncRoot)
            {
                Aborted?.Invoke(this, e);
                Log.Info("On Abort");
            }
        }

        protected abstract void Run();

        private void TryRunningTask()
        {
            try
            {
                RunTask();
                Log.Info("Task Ran");
            }
            catch (Exception e)
            {
                Log.Fatal($"Start failed: {e}");
                CancelTask();
                throw;
            }
        }

        private void RunTask()
        {
            if (_isRunning)
            {
                Log.Fatal("Already running");
                throw new InvalidOperationException("Already running");
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _speedtestConfiguration.CancellationToken = _cancellationTokenSource.Token;
            _task = new Task(
                Run);
            _task.ContinueWith(task => OnAbort(new EventArgs()), TaskContinuationOptions.NotOnCanceled);
            _task.Start();
            _isRunning = true;
            Log.Info("Started Run Task");
        }

        private void TryCancellingTask()
        {
            try
            {
                Log.Info("Task Cancelled");
                CancelTask();
            }
            catch (Exception e)
            {
                Log.Fatal($"Stop failed: {e}");
                throw;
            }
        }

        private void CancelTask()
        {
            if (!_isRunning)
            {
                Log.Fatal("Not running");
                return;
            }

            if (_cancellationTokenSource != null)
            {
                TryCancellingToken();

                if (_task != null)
                {
                    TryWaitForTask();
                    _task.Dispose();
                    _task = null;
                }

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            Log.Info("Cancel Task Ran");
            _isRunning = false;
        }

        private void TryWaitForTask()
        {
            try
            {
                _task.Wait();
                Log.Info("Wait Task Ran");
            }
            catch (AggregateException ae)
            {
                LogInnerExceptions(ae);
            }
        }

        private void TryCancellingToken()
        {
            try
            {
                _cancellationTokenSource.Cancel();
                Log.Info("Cancel Token Source");
            }
            catch (AggregateException ae)
            {
                LogInnerExceptions(ae);
            }
        }

        private void LogInnerExceptions(AggregateException ae)
        {
            foreach (var e in ae.Flatten().InnerExceptions)
            {
                Log.Fatal($"Caught exception: {e}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            Log.Info("Dispose is True");
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManagedObjects)
        {
            if (_isDisposed)
            {
                return;
            }
            if (disposeManagedObjects)
            {
                if (_isRunning)
                {
                    Log.Info("Stopped in Disposed");
                    Stop();
                }
            }
            _isDisposed = true;
        }

        protected void CheckDisposed(string className, string method)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(className, $"Object already disposed in: {method}");
            }
        }
    }

}
