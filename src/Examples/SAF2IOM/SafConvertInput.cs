using IdeaRS.OpenModel;
using IdeaStatiCa.SAF2IOM;
using System.Diagnostics;
using System.IO;

namespace SAF2IOM
{
	/// <summary>
	/// Holds input data for SAF to IOM converter.
	/// </summary>
	public class SafConvertInput : ISafConverterInput
	{
		private Stream SafStream;
		private Stream PersistenceStream;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="countryCode">Contry code for generated IOM</param>
		/// <param name="safStream">SAF data</param>
		/// <param name="persistenceStream">id mapping persistence (used for updating)</param>
		public SafConvertInput(CountryCode countryCode, Stream safStream, Stream persistenceStream)
		{
			Debug.Assert(persistenceStream != null);
			Debug.Assert(safStream != null);
			this.SafStream = safStream;
			this.PersistenceStream = persistenceStream;
			this.CountryCode = countryCode;
		}

		/// <summary>
		/// Country code for materials
		/// </summary>
		public CountryCode CountryCode { get; private set; }

		/// <summary>
		/// Returns stream of id mapping persistence. The stream must be closed by the caller.
		/// Must never return null.
		/// </summary>
		/// <param name="writable">Whether the stream should be writtable.</param>
		/// <returns>Id mapping persistence stream.</returns>
		public Stream GetPersistenceStream(bool writable = false)
		{
			return PersistenceStream;
		}

		/// <summary>
		/// Returns stream of SAF. The stream must be closed by the caller.
		/// </summary>
		/// <returns>SAF stream</returns>
		public Stream GetSafStream()
		{
			return SafStream;
		}

		/// <summary>
		/// Returns whether any store id mapping exists. Generally, it should return true if GetPersistenceStream(writable=true)
		/// was called before.
		/// </summary>
		/// <returns>True if stored id mapping exist, otherwise false.</returns>
		public bool PersistenceExists()
		{
			return false;
		}
	}
}
