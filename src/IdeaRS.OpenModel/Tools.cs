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

		private static string SerializeModel(object model, XmlSerializer xs)
		{
			string res;
			using (MemoryStream ms = new MemoryStream())
			{
				XmlTextWriter writer = new XmlTextWriter(ms, Encoding.Unicode);
				// Serialize using the XmlTextWriter.
				writer.Formatting = Formatting.Indented;
				xs.Serialize(writer, model);
				writer.Flush();
				ms.Position = 0;
				res = Encoding.Unicode.GetString(ms.ToArray());
			}

			return res;
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
		/// <returns>Deserialize instance model </returns>
		public static T DeserializeModel<T>(string xml)
		{
			var serializer = new XmlSerializer(typeof(T));
			return (T)serializer.Deserialize(new MemoryStream(Encoding.Unicode.GetBytes(xml)));
		}
	}
}