using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Importers;

namespace IdeaStatica.BimApiLink.Scoping
{
	public class BimLinkScope : IScope, IDisposable
	{
		private static readonly AsyncLocal<BimLinkScope?> _current = new();
		internal static BimLinkScope Current => _current.Value ?? throw new InvalidOperationException();

		public IBimApiImporter BimApiImporter { get; }

		public CountryCode CountryCode { get; }

		public BimLinkScope(IBimApiImporter bimApiImporter, CountryCode countryCode)
		{
			BimApiImporter = bimApiImporter;
			CountryCode = countryCode;

			_current.Value = this;
		}

		public void Dispose()
			=> _current.Value = null;
	}
}