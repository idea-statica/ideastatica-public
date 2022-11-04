using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Importers;
using System;
using System.Threading;

namespace IdeaStatica.BimApiLink.Scoping
{
	public class BimLinkScope : IScope, IDisposable
	{
		private static readonly AsyncLocal<BimLinkScope> _current = new AsyncLocal<BimLinkScope>();
		internal static BimLinkScope Current => _current.Value ?? throw new InvalidOperationException();

		public IBimApiImporter BimApiImporter { get; }

		public CountryCode CountryCode { get; }

		public object UserData { get; }

		public BimLinkScope(IBimApiImporter bimApiImporter, CountryCode countryCode, object userData)
		{
			BimApiImporter = bimApiImporter;
			CountryCode = countryCode;
			UserData = userData;

			_current.Value = this;
		}

		public void Dispose()
		{
			_current.Value = null;
			GC.SuppressFinalize(this);
		}
	}
}