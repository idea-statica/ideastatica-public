using Dlubal.RSTAB8;
using System;

namespace IdeaRstabPlugin
{
	internal class LicenceLock : IDisposable
	{
		private readonly IApplication _application;

		public LicenceLock(IModel model)
		{
			_application = model.GetApplication();
			_application.LockLicense();
		}

		public void Dispose()
		{
			_application.UnlockLicense();
		}
	}
}