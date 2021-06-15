using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using log4net;

// ReSharper disable AccessToDisposedClosure

namespace SpeedtestNetCli.Infrastructure
{
    public class ActionServiceRunner : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IService _service;
        private bool _isDisposed;

        public ActionServiceRunner(IService service)
        {
            _service = service;
        }

        public void Dispose()
        {
            Dispose(true);
            Log.Info("Dispose is True in Action Service Runner");
            GC.SuppressFinalize(this);
        }

        public int Run()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(ToString());
            }

            try
            {
                if (Environment.UserInteractive)
                {
                    RunOnConsole();
                }
                else
                {
                    RunAsSystemService();
                }
            }
            catch (Exception e)
            {
                Log.Fatal($"Caught exception: {e}");
                return 1;
            }

            return 0;
        }


        protected virtual void Dispose(bool disposing)
        {
            Log.Info("_Service Stopped under virtual void dispose");
            if (!_isDisposed && !disposing)
            {
                _service.Stop();
               
                _isDisposed = true;
            }
        }

        private void RunOnConsole()
        {
            using (var cancel = new ManualResetEvent(false))
            {
                ConsoleCancelEventHandler consoleCancelEventHandler = (sender, e) =>
                {
                    if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                    {
                        e.Cancel = true;
                    }

                    cancel.Set();
                };

                Console.CancelKeyPress += consoleCancelEventHandler;
                Console.TreatControlCAsInput = false;

                try
                {
                    EventHandler serviceAbortedEventHandler = (sender, e) => cancel.Set();
                    _service.Aborted += serviceAbortedEventHandler;

                    try
                    {
                        _service.Start();
                        cancel.WaitOne();
                        Log.Info("Stopped from Run On console");
                        _service.Stop();
                        
                    }
                    finally
                    {
                        _service.Aborted -= serviceAbortedEventHandler;
                    }
                }
                finally
                {
                    Console.CancelKeyPress -= consoleCancelEventHandler;

                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }
                }
            }
        }

        private void RunAsSystemService()
        {
            using (var callbackService = new ActionService())
            {
                EventHandler serviceAbortedEventHandler = (sender, e) =>
                {
                    callbackService.ExitCode = 1;
                    callbackService.Stop();
                    Log.Error("Aborted");
                };

                callbackService.StartAction = () =>
                {
                    _service.Aborted += serviceAbortedEventHandler;
                    _service.Start();
                    Log.Info("Started Action Call Back");
                };

                callbackService.StopAction = () =>
                {
                    Log.Info("Stop Action Stop()");
                    _service.Aborted -= serviceAbortedEventHandler;
                    _service.Stop();
                   
                };

                ServiceBase.Run(callbackService);
            }
        }
    }
}
