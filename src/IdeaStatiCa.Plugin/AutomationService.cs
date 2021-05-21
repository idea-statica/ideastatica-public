using System;
using System.Diagnostics;

namespace IdeaStatiCa.Plugin
{
	//public class

	public class AutomationService<ClientInterface> : IAutomation, IClientBIM<ClientInterface>, IDisposable
	{
		public AutomationService()
		{
		}

		public ClientInterface BIM { get; set; }

		public virtual string TempWorkingDir => throw new NotImplementedException();

		public string GetTempWorkingDir()
		{
			return TempWorkingDir;
		}

		public virtual string ProjectDir => throw new NotImplementedException();

		public string GetProjectDir()
		{
			return ProjectDir;
		}

		public virtual string OpenProject(string fileName)
		{
			Debug.Fail("Not implemented");
			return "Not implemented";
		}

		public virtual int SelectItem(string itemId)
		{
			Debug.Fail("Not implemented");
			return -1;
		}

		public virtual int RefreshProject()
		{
			Debug.Fail("Not implemented");
			return -1;
		}

		public virtual int CloseProject()
		{
			Debug.Fail("Not implemented");
			return -1;
		}

		public virtual int Shutdown()
		{
			Debug.Fail("Not implemented");
			return -1;
		}

		public virtual int Refresh()
		{
			Debug.Fail("Not implemented");
			return -1;
		}

		public virtual int NotifyChange()
		{
			Debug.Fail("Not implemented");
			return -1;
		}

		public AutomationStatus Status { get; protected set; }

		public AutomationStatus GetStatus()
		{
			return Status;
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
		// ~ConnectedApp()
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

		#endregion IDisposable Support
	}
}