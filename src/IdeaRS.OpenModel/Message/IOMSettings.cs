using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IdeaRS.OpenModel;

namespace IdeaRS.OpenModel.Message
{
 
	/// <summary>
	/// interface IIOMSettings
	/// </summary>
	public interface IIOMSettings
	{
		/// <summary>
		/// File name
		/// </summary>
		string FileName { get; set; }

		/// <summary>
		/// IOM was generated Successfully
		/// </summary>
		bool CreatedSuccessfully { get; set; }

		/// <summary>
		///  CountryCode
		/// </summary>
		CountryCode CountryCode { get; set; }
	}

	/// <summary>
	/// IOM Settings
	/// </summary>
	public class IOMSettings : IIOMSettings
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public IOMSettings()
		{
			CreatedSuccessfully = false;
			CountryCode = CountryCode.ECEN;
		}
		/// <summary>
		/// File name
		/// </summary>
		public string FileName { get; set; }
	
		/// <summary>
		/// Create OK
		/// </summary>
		public bool CreatedSuccessfully { get; set; }

		/// <summary>
		///  CountryCode
		/// </summary>
		public CountryCode CountryCode { get; set; }
	}

}
