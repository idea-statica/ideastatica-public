using IdeaRS.OpenModel.Connection;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel
{
	public static class Tools
	{
		/// <summary>
		/// Serialize OpenModelContainer to Xml
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static string OpenModelContainerToXml(OpenModelContainer model)
		{
			return SerializeModel(model);
		}

		/// <summary>
		/// Deserialize OpenModelContainer from Xml
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static OpenModelContainer OpenModelContainerFromXml(string xml)
		{
			OpenModelContainer iomTuple = DeserializeModel<OpenModelContainer>(xml);

			if (iomTuple?.OpenModel != null)
			{
				iomTuple.OpenModel.ReferenceElementsReconstruction();
			}

			return iomTuple;
		}

		/// <summary>
		/// Serialize OpenModelContainer to xml 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filePath"></param>
		public static void OpenModelContainerToFile(OpenModelContainer model, string filePath)
		{
			SerializeModelToFile(model, filePath);
		}

		/// <summary>
		/// Deserialize OpenModelContainer from file
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static OpenModelContainer OpenModelContainerFromFile(string filePath)
		{
			OpenModelContainer iomTuple = DeserializeModelFromFile<OpenModelContainer>(filePath);

			if (iomTuple?.OpenModel != null)
			{
				iomTuple.OpenModel.ReferenceElementsReconstruction();
			}

			return iomTuple;
		}

		/// <summary>
		/// Serialize ConnectionData to xml
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static string ConnectionDataToXml(ConnectionData model)
		{
			return SerializeModel(model);
		}

		/// <summary>
		/// Deserialize ConnectionData from xml
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static ConnectionData ConnectionDataFromXml(string xml)
		{
			return DeserializeModel<ConnectionData>(xml);
		}

		/// <summary>
		/// Serialize Model
		/// </summary>
		/// <typeparam name="T">Type of model</typeparam>
		/// <param name="model">instance of model</param>
		/// <param name="filePath">File path</param>
		/// <returns>Serialize model in string</returns>
		public static void SerializeModelToFile<T>(T model, string filePath)
		{
			using (XmlWriter writer = XmlWriter.Create(filePath, GetWriterSettings()))
			{
				SerializeModel(model, writer);
			}
		}

		/// <summary>
		/// Serialize Model
		/// </summary>
		/// <typeparam name="T">Type of model</typeparam>
		/// <param name="model">instance of model</param>
		/// <returns>Serialize model in string</returns>
		public static string SerializeModel<T>(T model)
		{
			StringBuilder builder = new StringBuilder();
			XmlWriter writer = XmlWriter.Create(builder, GetWriterSettings());
			SerializeModel(model, writer);

			return builder.ToString();
		}

		/// <summary>
		/// Deserialize Model
		/// </summary>
		/// <typeparam name="T">Type of model</typeparam>
		/// <param name="xml">Serialize model in string</param>
		/// <returns>Deserialize instance model</returns>
		public static T DeserializeModel<T>(string xml)
		{
			using (StringReader stringReader = new StringReader(xml))
			{
				// Try skipping utf-16 BOM for backwards compatibility
				int ch = stringReader.Peek();
				if (ch == 0xfeff || ch == 0xffef)
				{
					stringReader.Read();
				}

				XmlReader reader = XmlReader.Create(stringReader, GetReaderSettings());

				return DeserializeModel<T>(reader);
			}
		}

		/// <summary>
		/// Deserialize a model from XML
		/// </summary>
		/// <typeparam name="T">Type of model</typeparam>
		/// <param name="stream">XML stream</param>
		/// <returns>Deserialize instance model</returns>
		public static T DeserializeModel<T>(Stream stream)
			=> DeserializeModel<T>(new StreamReader(stream));

		/// <summary>
		/// Deserialize a model from XML
		/// </summary>
		/// <typeparam name="T">Type of model</typeparam>
		/// <param name="textReader">XML stream reader</param>
		/// <returns>Deserialize instance model</returns>
		public static T DeserializeModel<T>(TextReader textReader)
		{
			XmlReader reader = XmlReader.Create(textReader, GetReaderSettings());
			return DeserializeModel<T>(reader);
		}

		/// <summary>
		/// Creates the instance from the XML file
		/// </summary>
		/// <param name="xmlFileName">XML file name</param>
		/// <returns>Deserialize instance model </returns>
		public static T DeserializeModelFromFile<T>(string xmlFileName)
		{
			var text = File.ReadAllText(xmlFileName);

			return DeserializeModel<T>(text);
		}

		/// <summary>
		/// Deserialize Model from stream
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream"></param>
		/// <returns>>Deserialize instance model</returns>
		public static T DeserializeModelFromStream<T>(Stream stream)
			=> DeserializeModel<T>(stream);

		private static void SerializeModel<T>(T model, XmlWriter writer)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			serializer.Serialize(writer, model);
		}

		private static T DeserializeModel<T>(XmlReader reader)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(reader);
		}

		private static XmlWriterSettings GetWriterSettings()
		{
			return new XmlWriterSettings()
			{
				Indent = true,
				CheckCharacters = false,
				Encoding = Encoding.Unicode
			};
		}

		private static XmlReaderSettings GetReaderSettings()
		{
			return new XmlReaderSettings()
			{
				IgnoreWhitespace = true,
				CheckCharacters = false
			};
		}
	}
}