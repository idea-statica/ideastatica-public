using IdeaRS.OpenModel;
using IdeaStatiCa.SAF2IOM;
using System.Diagnostics;
using System.IO;

namespace SAF2IOM
{
	public class SafConvertInput : ISafConverterInput
	{
		private Stream SafStream;
		private Stream PersistenceStream;

		public SafConvertInput(CountryCode countryCode, Stream safStream, Stream persistenceStream)
		{
			Debug.Assert(persistenceStream != null);
			Debug.Assert(safStream != null);
			this.SafStream = safStream;
			this.PersistenceStream = persistenceStream;
			this.CountryCode = countryCode;
		}

		public CountryCode CountryCode { get; private set; }

		public Stream GetPersistenceStream(bool writable = false)
		{
			return PersistenceStream;
		}

		public Stream GetSafStream()
		{
			return SafStream;
		}

		public bool PersistenceExists()
		{
			return true;
		}
	}
}
