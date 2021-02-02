using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IdeaStatiCa.Plugin
{
	public static class Tools
	{
		public static string ModelToXml(List<ModelBIM> model)
		{
			XmlSerializer xs = new XmlSerializer(typeof(List<ModelBIM>));
			return SerializeModel(model, xs);
		}

		public static string OpenModelContainerToXml(IdeaRS.OpenModel.OpenModelContainer model)
		{
			XmlSerializer xs = new XmlSerializer(typeof(IdeaRS.OpenModel.OpenModelContainer));
			return SerializeModel(model, xs);
		}

		public static IdeaRS.OpenModel.OpenModelContainer OpenModelContainerFromXml(string xml)
		{
			var serializer = new XmlSerializer(typeof(IdeaRS.OpenModel.OpenModelContainer));
			IdeaRS.OpenModel.OpenModelContainer iomTuple = serializer.Deserialize(new MemoryStream(Encoding.Unicode.GetBytes(xml))) as IdeaRS.OpenModel.OpenModelContainer;

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

		public static string ConnectionDataToXml(IdeaRS.OpenModel.Connection.ConnectionData model)
		{
			XmlSerializer xs = new XmlSerializer(typeof(IdeaRS.OpenModel.Connection.ConnectionData));
			return SerializeModel(model, xs);
		}
		public static IdeaRS.OpenModel.Connection.ConnectionData ConnectionDataFromXml(string xml)
		{
			var serializer = new XmlSerializer(typeof(IdeaRS.OpenModel.Connection.ConnectionData));
			return serializer.Deserialize(new MemoryStream(Encoding.Unicode.GetBytes(xml))) as IdeaRS.OpenModel.Connection.ConnectionData;
		}

		public static string ModelToXml(ModelBIM model)
		{
			XmlSerializer xs = new XmlSerializer(typeof(ModelBIM));
			return SerializeModel(model, xs);
		}

		public static string ProjectToXml(BIMProject project)
		{
			XmlSerializer xs = new XmlSerializer(typeof(BIMProject));
			return SerializeModel(project, xs);
		}

		public static BIMProject ProjectFromXml(string xml)
		{
			var serializer = new XmlSerializer(typeof(BIMProject));
			return serializer.Deserialize(new MemoryStream(Encoding.Unicode.GetBytes(xml))) as BIMProject;
		}

		public static ModelBIM ModelFromXml(string xml)
		{
			var serializer = new XmlSerializer(typeof(ModelBIM));
			ModelBIM modelFEA = serializer.Deserialize(new MemoryStream(Encoding.Unicode.GetBytes(xml))) as ModelBIM;
			if (modelFEA != null && modelFEA.Model != null)
			{
				modelFEA.Model.ReferenceElementsReconstruction();
			}
			return modelFEA;
		}

		public static List<ModelBIM> ModelsFromXml(string xml)
		{
			var serializer = new XmlSerializer(typeof(List<ModelBIM>));
			var models = serializer.Deserialize(new MemoryStream(Encoding.Unicode.GetBytes(xml))) as List<ModelBIM>;
			foreach (var model in models)
			{
				if (model != null && model.Model != null)
				{
					model.Model.ReferenceElementsReconstruction();
				}
			}
			return models;
		}
	}
}