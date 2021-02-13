using IdeaStatiCa.Plugin.Grpc.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nito.AsyncEx.Synchronous;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin
{
    /// <summary>
    /// Grpc Hosting implementation of <see cref="AutomationHosting{MyInterface, ClientInterface}"/>
    /// </summary>
    public class AutomationHostingGrpc<MyInterface, ClientInterface> : IBIMPluginClient<ClientInterface>, IDisposable where MyInterface : class where ClientInterface : class
    {
        private Task hostingTask;
        private CancellationTokenSource tokenSource;
        private ManualResetEvent mre;
        private MyInterface automation;
        private Process bimProcess = null;
        private int myAutomatingProcessId;
        private readonly string EventName;
        private GrpcServiceBasedReflectionClient<ClientInterface> grpcClient;
        private readonly ILogger ideaLogger = null;

        /// <summary>
        /// My BIM object.
        /// </summary>
        public ClientInterface MyBIM
        {
            get
            {
                if (grpcClient == null || !grpcClient.IsConnected)
                {
                    return null;
                }

                return grpcClient.Service;
            }
        }

        /// <summary>
        /// Port on which the Grpc server is running.
        /// </summary>
        public int GrpcPort { get; private set; }

        /// <summary>
        /// Current automation status.
        /// </summary>
        public AutomationStatus Status { get; private set; }

        /// <summary>
        /// Determines whether automation hosting is connected to Grpc server.
        /// </summary>
        public bool IsConnected { get { return (grpcClient?.IsConnected).Value; } }

        /// <summary>
        /// Triggered when BIM status changes.
        /// </summary>
        public event ISEventHandler BIMStatusChanged;

        public AutomationHostingGrpc(MyInterface hostedService,
            int grpcPort,
            ILogger logger = null,
            string eventName = Constants.DefaultPluginEventName)
        {
            //ideaLogger = Diagnostics.IdeaDiagnostics.GetLogger("ideastatica.plugin.automationhostinggrpc");
            ideaLogger = logger ?? NullLogger.Instance;
            Status = AutomationStatus.Unknown;
            automation = hostedService;
            EventName = eventName;
            GrpcPort = grpcPort;
            mre = new ManualResetEvent(false);
        }

        /// <summary>
        /// Starts the <see cref="AutomationHostingGrpc{MyInterface, ClientInterface}".
        /// </summary>
        /// <param name="id">Client id</param>
        /// <returns></returns>
        public Task RunAsync(string id)
        {
            if (hostingTask != null)
            {
                Debug.Fail("Task is running");
                return Task.CompletedTask;
            }

            tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            hostingTask = Task.Run(() =>
            {
                try
                {
                    RunServer(id, token);
                }
                catch (Exception e)
                {
                    ideaLogger.LogError("RunAsync  RunServer failed", e);
                    throw;
                }
            }, token);

            return hostingTask;
        }

        public void Stop()
        {
            if (hostingTask != null)
            {
                tokenSource.Cancel();
                var stopRes = mre.WaitOne();

                Debug.Assert(stopRes, "Cannot stop");
            }
        }

        protected virtual void RunServer(string id, System.Threading.CancellationToken cancellationToken)
        {
            ideaLogger.LogInformation("Calling RunServer");

            mre.Reset();

            bool isBimRunning = false;

            if (!string.IsNullOrEmpty(id))
            {
                myAutomatingProcessId = int.Parse(id);
                ideaLogger.LogInformation($"RunServer - processId == '{myAutomatingProcessId}'");

                bimProcess = Process.GetProcessById(myAutomatingProcessId);
                bimProcess.EnableRaisingEvents = true;
                bimProcess.Exited += new EventHandler(BimProcess_Exited);

                // initialize grpc client
                grpcClient = new GrpcServiceBasedReflectionClient<ClientInterface>(id, GrpcPort);                
                grpcClient.ConnectAsync().WaitAndUnwrapException();

                Status |= AutomationStatus.IsClient;
                isBimRunning = true;
            }

            if (!isBimRunning)
            {
                bimProcess = null;
                myAutomatingProcessId = -1;
            }

            NotifyBIMStatusChanged(AppStatus.Started);

            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            ideaLogger.LogInformation($"RunServer - Automation Service has been stopped");

            grpcClient?.DisconnectAsync().WaitAndUnwrapException();

            NotifyBIMStatusChanged(AppStatus.Finished);

            mre.Set();
        }

        protected virtual void NotifyBIMStatusChanged(AppStatus newStatus)
        {
            BIMStatusChanged?.Invoke(this, new ISEventArgs() { Status = newStatus });
        }

        private void BimProcess_Exited(object sender, EventArgs e)
        {
            bimProcess.Exited -= new EventHandler(BimProcess_Exited);
            Status &= ~AutomationStatus.IsClient;
            bimProcess.Dispose();
            bimProcess = null;
            myAutomatingProcessId = -1;

            Stop();
        }

        #region IDisposable
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
