namespace IdeaStatiCa.Plugin
{
    [Obsolete("This class will be replaced by IdeaConnectionController controller.")]
    public class IdeaConnectionControllerGrpc : IdeaConnectionController
    {
        private IdeaConnectionControllerGrpc(string ideaInstallDir, IPluginLogger logger) : base(ideaInstallDir, logger)
        {
        }

        /// <summary>
        /// Creates connection and starts IDEA StatiCa connection application.
        /// Call OpenProject after this method to open specific project.
        /// </summary>
        /// <param name="ideaInstallDir">IDEA StatiCa installation directory.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>A controller object.</returns>
        public new static IConnectionController Create(string ideaInstallDir, IPluginLogger logger)
        {
            IdeaConnectionControllerGrpc connectionController = new IdeaConnectionControllerGrpc(ideaInstallDir, logger);
            connectionController.OpenConnectionClient();
            return connectionController;
        }

        public int OpenProject(string fileName)
        {
            GrpcClient.MyBIM.OpenProject(fileName);
            return 1;
        }

        public int CloseProject()
        {
            GrpcClient.MyBIM.CloseProject();
            return 1;
        }

        private void OpenConnectionClient()
        {
            OpenConnectionClientGrpc();
        }

        private void OpenConnectionClientGrpc()
        {
            int processId = Process.GetCurrentProcess().Id;
            GrpcPort = PortFinder.FindPort(Constants.MinGrpcPort, Constants.MaxGrpcPort);
            string connChangedEventName = string.Format(Constants.ConnectionChangedEventFormat, processId);
            CurrentItemChangedEvent = new EventWaitHandle(false, EventResetMode.AutoReset, connChangedEventName);

            string applicationExePath = Path.Combine(IdeaInstallDir, Constants.IdeaConnectionAppName);

            if (!File.Exists(applicationExePath))
            {
                throw new ArgumentException($"IdeaConnectionController.OpenConnectionClient - file '{applicationExePath}' doesn't exist");
            }

            Process connectionProc = new Process();
            string eventName = string.Format("IdeaStatiCaEvent{0}", processId);
            using (EventWaitHandle syncEvent = new EventWaitHandle(false, EventResetMode.AutoReset, eventName))
            {
                connectionProc.StartInfo = new ProcessStartInfo(applicationExePath, $"-cmd:automation-{processId} {IdeaStatiCa.Plugin.Constants.GrpcControlPortParam}:{GrpcPort} user-mode 192");
                connectionProc.EnableRaisingEvents = true;
                connectionProc.Start();

                if (!syncEvent.WaitOne(StartTimeout))
                {
                    throw new TimeoutException($"Time out - process '{applicationExePath}' doesn't set the event '{eventName}'");
                }
            }

            IdeaStatiCaProcess = connectionProc;
            var grpcClient = new GrpcClient(Logger);

            var grpcClientTask = grpcClient.StartAsync(processId.ToString(), GrpcPort);

            GrpcClient = new AutomationHostingGrpc<IAutomation, IAutomation>(null, grpcClient, Logger);
            GrpcClient.RunAsync(processId.ToString());

            IdeaStatiCaProcess.Exited += CalculatorProcess_Exited;
        }

        private void CalculatorProcess_Exited(object sender, EventArgs e)
        {
            if (IdeaStatiCaProcess == null)
            {
                return;
            }

            IdeaStatiCaProcess.Dispose();
            IdeaStatiCaProcess = null;
            CalculatorUrl = null;
            //ConnectionAppClient = null;

            if (ConnectionAppExited != null)
            {
                ConnectionAppExited(this, e);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ConnectionControllerFactory()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
