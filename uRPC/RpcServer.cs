using System;
using System.Net;
using System.Threading;

namespace uRPC
{
    /// <summary>
    /// Receives and responds to RPCs.
    /// </summary>
    public class RpcServer
    {
        private readonly HttpListener _httpListener;
        private readonly Thread _listenThread;
        private bool _isRunning;

        /// <summary>
        /// Invoked whenever the RPC server has a message that can be outputted to any logging system.
        /// </summary>
        public event Action<string, RpcLogSeverity> LogMessageDispatched;
        
        /// <param name="uriPrefix">The URI prefix to be used by the HttpListener.</param>
        public RpcServer(string uriPrefix)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(uriPrefix);

            _listenThread = new Thread(ListenThreadRoutine);

            _isRunning = false;
        }

        /// <summary>
        /// Starts the RPC server using threading internally.
        /// </summary>
        public void Start()
        {
            _isRunning = true;

            _httpListener.Start();
            _listenThread.Start();

            Log("RPC server started.", RpcLogSeverity.Information);
        }
        
        /// <summary>
        /// Starts the RPC server using C# async internally.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void StartAsync()
        {
            Log("Attempted to start the RpcServer using C# async which is not implemented yet.",
                RpcLogSeverity.Error);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the RPC server.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _httpListener.Stop();
        }

        private void Log(string message, RpcLogSeverity severity)
        {
            LogMessageDispatched?.Invoke(message, severity);
        }

        private void ListenThreadRoutine()
        {
            while (_isRunning)
            {
                try
                {
                    var context = _httpListener.GetContext(); // Thread blocking
                    Log(context.Request.RawUrl, RpcLogSeverity.Information);
                    context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    context.Response.Close();
                }
                catch
                {
                    // ignored
                }
            }
            
            Log("RPC server listen thread exiting.", RpcLogSeverity.Information);
        }
    }
}