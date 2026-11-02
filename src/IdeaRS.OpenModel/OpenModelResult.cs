using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Results of open model
	/// </summary>
	public class OpenModelResult
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OpenModelResult()
		{
			ResultOnMembers = new List<ResultOnMembers>();
		}

		/// <summary>
		/// Results on members
		/// </summary>
		public List<ResultOnMembers> ResultOnMembers { get; set; }

		/// <summary>
		/// Saves content into file as a XML format
		/// </summary>
		/// <param name="xmlFileName">XML file name</param>
		/// <returns>True if succeeded</returns>
		public bool SaveToXmlFile(string xmlFileName)
		{
			// Storing to standard xml file
			XmlSerializer xs = new XmlSerializer(typeof(OpenModelResult));

			Stream fs = new FileStream(xmlFileName, FileMode.Create);
			XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode);
			// Serialize using the XmlTextWriter.
			writer.Formatting = Formatting.Indented;
			xs.Serialize(writer, this);
			writer.Close();
			fs.Close();

			return true;
		}

		/// <summary>
		/// Creates the instance of Open Model Result from the XML file
		/// </summary>
		/// <param name="xmlFileName">XML file name</param>
		/// <returns>Open Model Result or null</returns>
		public static OpenModelResult LoadFromXmlFile(string xmlFileName)
		{
			using (FileStream fs = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read))
			{
				return LoadFromStream(fs);
			}
		}

		/// <summary>
		/// Creates the instance of Open Model Result  from the stream
		/// </summary>
		/// <param name="xmlFileStream">The input stream</param>
		/// <returns>The new instance of Open Model Result </returns>
		public static OpenModelResult LoadFromStream(Stream xmlFileStream)
		{
			XmlReaderSettings xmlSettings = new XmlReaderSettings();
			xmlSettings.CloseInput = false;

			XmlReader reader = XmlReader.Create(xmlFileStream, xmlSettings);
			XmlSerializer xs = new XmlSerializer(typeof(OpenModelResult));
			OpenModelResult results = (xs.Deserialize(reader) as OpenModelResult);

			return results;
		}
	}
}