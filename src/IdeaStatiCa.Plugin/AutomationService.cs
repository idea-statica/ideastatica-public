using System;
using System.Diagnostics;
using System.Threading.Tasks;

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

		public virtual string ProjectDir => throw new NotImplementedException();

		/// <summary>
		/// Empty virtual implementation - does nothing
		/// </summary>
		/// <param name="fileName">Project to open</param>
		/// <param name="optionalParamJson"></param>
		/// <returns></returns>
		public virtual Task OpenProjectAsync(string fileName, string optionalParamJson)
		{
			Debug.Fail("Not implemented");
			return Task.CompletedTask;
		}

		/// <summary>
		/// It calls <see cref="OpenProjectAsync(string, string)"/>"/>
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="optionalParamJson"></param>
		public virtual void OpenProject(string fileName, string optionalParamJson)
		{
			OpenProjectAsync(fileName, optionalParamJson).Wait();
		}

		public virtual void SelectItem(string itemId)
		{
			Debug.Fail("Not implemented");
		}

		public virtual Task RefreshProjectAsync()
		{
			Debug.Fail("Not implemented");

			return Task.CompletedTask;
		}

		public void RefreshProject()
		{
			RefreshProjectAsync().Wait();
		}

		public void CloseProject()
		{
			CloseProjectAsync().Wait();
		}

		public virtual Task CloseProjectAsync()
		{
			Debug.Fail("Not implemented");

			return Task.CompletedTask;
		}

		public virtual void Shutdown()
		{
			Debug.Fail("Not implemented");
		}

		public virtual Task RefreshAsync()
		{
			Debug.Fail("Not implemented");

			return Task.CompletedTask;
		}

		public void Refresh()
		{
			RefreshAsync().Wait();
		}

		public virtual void NotifyChange()
		{
			Debug.Fail("Not implemented");
		}

		public AutomationStatus Status { get; protected set; }

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