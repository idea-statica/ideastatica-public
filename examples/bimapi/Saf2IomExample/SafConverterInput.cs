using IdeaRS.OpenModel;
using IdeaStatiCa.SAF2IOM;
using System.IO;

namespace Saf2IomExample
{
	/// <summary>
	/// What <see cref="SAF2IOMConverter"/> reads: the SAF file, and the design code the IOM is made for.
	/// </summary>
	internal class SafConverterInput : ISafConverterInput
	{
		private readonly string _safFilePath;

		public SafConverterInput(string safFilePath, CountryCode countryCode)
		{
			_safFilePath = safFilePath;
			CountryCode = countryCode;
		}

		/// <summary>
		/// The design code the IOM is made for.
		/// </summary>
		public CountryCode CountryCode { get; }

		/// <summary>
		/// The SAF file. The converter closes the stream.
		/// </summary>
		public Stream GetSafStream()
		{
			return File.OpenRead(_safFilePath);
		}

		/// <summary>
		/// The converter keeps here which IOM id it gave to which SAF object, so that a later conversion of the same
		/// model gives the objects the same ids. This example converts each file anew, so it keeps nothing.
		/// </summary>
		public Stream GetPersistenceStream(bool writable = false)
		{
			return Stream.Null;
		}

		/// <summary>
		/// False: there are no ids kept from an earlier conversion.
		/// </summary>
		public bool PersistenceExists()
		{
			return false;
		}
	}
}
