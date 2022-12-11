using System;
using System.Net;

namespace uRPC
{
    /// <summary>
    /// Receives and responds to RPCs.
    /// </summary>
    public class RpcServer
    {
        private readonly HttpListener _httpListener;

        /// <summary>
        /// Invoked whenever the RPC server has a message that can be outputted to a logging system.
        /// </summary>
        public event Action<string, RpcLogSeverity> LogMessageDispatched;
        
        /// <param name="uriPrefix">The URI prefix to be used by the HttpListener.</param>
        public RpcServer(string uriPrefix)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(uriPrefix);
        }

        /// <summary>
        /// Starts the RPC server.
        /// </summary>
        public void Start()
        {
            _httpListener.Start();
            _httpListener.BeginGetContext(OnContextAvailable, _httpListener);
            
            Log("RPC server started.", RpcLogSeverity.Information);
        }

        /// <summary>
        /// Stops the RPC server.
        /// </summary>
        public void Stop()
        {
            _httpListener.Stop();
            Log("RPC server stopped.", RpcLogSeverity.Information);
        }

        private void OnContextAvailable(IAsyncResult asyncResult)
        {
            var context = _httpListener.EndGetContext(asyncResult);
            
            // TODO: Implement middleware

            context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
            context.Response.Close();

            // Wait for then process next context
            _httpListener.BeginGetContext(OnContextAvailable, _httpListener);
        }

        private void Log(string message, RpcLogSeverity severity)
        {
            LogMessageDispatched?.Invoke(message, severity);
        }
    }
}