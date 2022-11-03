using IdeaRS.OpenModel.Result;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// OpenModelContainer is used to keep structural data and results of a finite element analysis in one place.
	/// The main reason is easier moving (passing) pass the instance of OpenModel and corresponding instace of OpenModelResults.
	/// </summary>
	public class OpenModelContainer
	{
		/// <summary>
		/// Open Model - structural data
		/// </summary>
		public OpenModel OpenModel { get; set; }

		/// <summary>
		/// Open Model Result - results of a finite element analysis
		/// </summary>
		public OpenModelResult OpenModelResult { get; set; }

		/// <summary>
		/// Saves content into file as a XML format
		/// </summary>
		/// <param name="xmlFileName">XML file name</param>
		/// <returns>True if succeeded</returns>
		public bool SaveToXmlFile(string xmlFileName)
		{
			// Storing to standard xml file
			XmlSerializer xs = new XmlSerializer(typeof(OpenModelContainer));

			Stream fs = new FileStream(xmlFileName, FileMode.Create);
			XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode)
			{
				Formatting = Formatting.Indented
			};
			// Serialize using the XmlTextWriter.
			xs.Serialize(writer, this);
			writer.Close();
			fs.Close();

			return true;
		}

		/// <summary>
		/// Creates the instance of Open Model container from the XML file
		/// </summary>
		/// <param name="xmlFileName">XML file name</param>
		/// <returns>Open Model Container or null</returns>
		public static OpenModelContainer LoadFromXmlFile(string xmlFileName)
		{
			OpenModelContainer openModelContainer = null;
			using (FileStream fs = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read))
			{
				openModelContainer = LoadFromStream(fs);
			}

			return openModelContainer;
		}

		/// <summary>
		/// Creates the instance of Open Model container from the stream
		/// </summary>
		/// <param name="xmlFileStream">The input stream</param>
		/// <returns>The new instance of Open Model Container</returns>
		public static OpenModelContainer LoadFromStream(Stream xmlFileStream)
		{
			XmlReaderSettings xmlSettings = new XmlReaderSettings
			{
				CloseInput = false
			};

			XmlReader reader = XmlReader.Create(xmlFileStream, xmlSettings);
			XmlSerializer xs = new XmlSerializer(typeof(OpenModelContainer));
			OpenModelContainer openModelContainer = (xs.Deserialize(reader) as OpenModelContainer);
			openModelContainer.OpenModel.ReferenceElementsReconstruction();

			return openModelContainer;
		}

		/// <summary>
		/// Creates the instance of Open Model container from xml string
		/// </summary>
		/// <param name="xmlString">The input string</param>
		/// <returns>The new instance of Open Model Container</returns>
		public static OpenModelContainer LoadFromString(string xmlString)
		{
			StringReader stringReader = new System.IO.StringReader(xmlString);
			XmlSerializer serializer = new XmlSerializer(typeof(OpenModelContainer));
			OpenModelContainer openModelContainer = serializer.Deserialize(stringReader) as OpenModelContainer;
			openModelContainer.OpenModel.ReferenceElementsReconstruction();

			return openModelContainer;
		}
	}
}
