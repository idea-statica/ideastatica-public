using System.IO;
using System.Text;
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
		public static string OpenModelContainerToXml(IdeaRS.OpenModel.OpenModelContainer model)
		{
			return SerializeModel<IdeaRS.OpenModel.OpenModelContainer>(model);
		}

		/// <summary>
		/// Deserialize OpenModelContainer from Xml
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static IdeaRS.OpenModel.OpenModelContainer OpenModelContainerFromXml(string xml)
		{
			IdeaRS.OpenModel.OpenModelContainer iomTuple = DeserializeModel<IdeaRS.OpenModel.OpenModelContainer>(xml);

			if (iomTuple?.OpenModel != null)
			{
				iomTuple.OpenModel.ReferenceElementsReconstruction();
			}

			return iomTuple;
		}

		/// <summary>
		/// Serialize OpenModelContainer to File 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static void OpenModelContainerToFile(IdeaRS.OpenModel.OpenModelContainer model, string filePath)
		{
			SerializeModelToFile<IdeaRS.OpenModel.OpenModelContainer>(model, filePath);
		}

		/// <summary>
		/// Deserialize OpenModelContainer from file
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static IdeaRS.OpenModel.OpenModelContainer OpenModelContainerFromFile(string filePath)
		{
			IdeaRS.OpenModel.OpenModelContainer iomTuple = DeserializeModelFromFile<IdeaRS.OpenModel.OpenModelContainer>(filePath);

			if (iomTuple?.OpenModel != null)
			{
				iomTuple.OpenModel.ReferenceElementsReconstruction();
			}

			return iomTuple;
		}


		private static string SerializeModel(object model, XmlSerializer xs)
		{
			string res;
			using (MemoryStream ms = new MemoryStream())
			{
				FillStream(model, ms, xs);
				ms.Position = 0;
				res = Encoding.Unicode.GetString(ms.ToArray());
			}

			return res;
		}

		private static void SerializeModelToFile(object model, string filePath, XmlSerializer xs)
		{
			using (FileStream fs = new FileStream(filePath, FileMode.Create))
			{
				FillStream(model, fs, xs);
			}
		}

		private static void FillStream(object model, Stream stream, XmlSerializer xs)
		{
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Unicode);
			// Serialize using the XmlTextWriter.
			writer.Formatting = Formatting.Indented;
			xs.Serialize(writer, model);
			writer.Flush();
		}

		/// <summary>
		/// Serialize ConnectionData to xml
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static string ConnectionDataToXml(IdeaRS.OpenModel.Connection.ConnectionData model)
		{
			return SerializeModel<IdeaRS.OpenModel.Connection.ConnectionData>(model);
		}

		/// <summary>
		/// Deserialize ConnectionData from xml
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static IdeaRS.OpenModel.Connection.ConnectionData ConnectionDataFromXml(string xml)
		{
			return DeserializeModel<IdeaRS.OpenModel.Connection.ConnectionData>(xml);
		}


		/// <summary>
		/// Serialize Model
		/// </summary>
		/// <typeparam name="T">Type of model</typeparam>
		/// <param name="model">instance of model</param>
		/// <param name="filePath">File path</param>
		/// <returns>Serialize model in string</returns>
		public static void SerializeModelToFile<T>(object model, string filePath)
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			SerializeModelToFile(model, filePath, xs);
		}

		/// <summary>
		/// Serialize Model
		/// </summary>
		/// <typeparam name="T">Type of model</typeparam>
		/// <param name="model">instance of model</param>
		/// <returns>Serialize model in string</returns>
		public static string SerializeModel<T>(object model)
		{
			XmlSerializer xs = new XmlSerializer(typeof(T));
			return SerializeModel(model, xs);
		}

		/// <summary>
		/// Deserialize Model
		/// </summary>
		/// <typeparam name="T">Type of model</typeparam>
		/// <param name="xml">Serialize model in string</param>
		/// <returns>Deserialize instance model</returns>
		public static T DeserializeModel<T>(string xml)
		{
			using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
			{
				return DeserializeModelFromStream<T>(ms);
			}
		}

		/// <summary>
		/// Creates the instance from the XML file
		/// </summary>
		/// <param name="xmlFileName">XML file name</param>
		/// <returns>Deserialize instance model </returns>
		public static T DeserializeModelFromFile<T>(string xmlFileName)
		{
			using (FileStream fs = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read))
			{
				return DeserializeModelFromStream<T>(fs);
			}
		}

		/// <summary>
		/// Deserialize Model from stream
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="stream"></param>
		/// <returns>>Deserialize instance model</returns>
		public static T DeserializeModelFromStream<T>(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(stream);
		}
	}
}