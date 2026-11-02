using System.Collections.Generic;

namespace IdeaStatiCa.Plugin
{
	public static class Tools
	{
		public static string ModelToXml(List<ModelBIM> model)
		{
			return IdeaRS.OpenModel.Tools.SerializeModel<List<ModelBIM>>(model);
		}

		public static void ModelToFile(List<ModelBIM> model, string filePath)
		{
			IdeaRS.OpenModel.Tools.SerializeModelToFile<List<ModelBIM>>(model, filePath);
		}

		public static string OpenModelContainerToXml(IdeaRS.OpenModel.OpenModelContainer model)
		{
			return IdeaRS.OpenModel.Tools.OpenModelContainerToXml(model);
		}

		public static void OpenModelContainerToFile(IdeaRS.OpenModel.OpenModelContainer model, string filePath)
		{
			IdeaRS.OpenModel.Tools.OpenModelContainerToFile(model, filePath);
		}

		public static IdeaRS.OpenModel.OpenModelContainer OpenModelContainerFromXml(string xml)
		{
			return IdeaRS.OpenModel.Tools.OpenModelContainerFromXml(xml);
		}
		public static IdeaRS.OpenModel.OpenModelContainer OpenModelContainerFromFile(string filePath)
		{
			return IdeaRS.OpenModel.Tools.OpenModelContainerFromFile(filePath);
		}
		public static string ConnectionDataToXml(IdeaRS.OpenModel.Connection.ConnectionData model)
		{
			return IdeaRS.OpenModel.Tools.ConnectionDataToXml(model);
		}
		public static IdeaRS.OpenModel.Connection.ConnectionData ConnectionDataFromXml(string xml)
		{
			return IdeaRS.OpenModel.Tools.ConnectionDataFromXml(xml);
		}

		public static string ModelToXml(ModelBIM model)
		{
			return IdeaRS.OpenModel.Tools.SerializeModel<ModelBIM>(model);
		}

		public static void ModelToFile(ModelBIM model, string filePath)
		{
			IdeaRS.OpenModel.Tools.SerializeModelToFile<ModelBIM>(model, filePath);
		}

		public static string ProjectToXml(BIMProject project)
		{
			return IdeaRS.OpenModel.Tools.SerializeModel<BIMProject>(project);
		}

		public static void ProjectToFile(BIMProject project, string filePath)
		{
			IdeaRS.OpenModel.Tools.SerializeModelToFile<BIMProject>(project, filePath);
		}

		public static BIMProject ProjectFromXml(string xml)
		{
			return IdeaRS.OpenModel.Tools.DeserializeModel<BIMProject>(xml);
		}

		public static BIMProject ProjectFromFile(string filePath)
		{
			return IdeaRS.OpenModel.Tools.DeserializeModelFromFile<BIMProject>(filePath);
		}

		public static ModelBIM ModelFromXml(string xml)
		{
			ModelBIM modelFEA = IdeaRS.OpenModel.Tools.DeserializeModel<ModelBIM>(xml);

			if (modelFEA?.Model != null)
			{
				modelFEA.Model.ReferenceElementsReconstruction();
			}
			return modelFEA;
		}

		public static ModelBIM ModelFromFile(string filePath)
		{
			ModelBIM modelFEA = IdeaRS.OpenModel.Tools.DeserializeModelFromFile<ModelBIM>(filePath);

			if (modelFEA?.Model != null)
			{
				modelFEA.Model.ReferenceElementsReconstruction();
			}
			return modelFEA;
		}

		public static List<ModelBIM> ModelsFromXml(string xml)
		{
			List<ModelBIM> models = IdeaRS.OpenModel.Tools.DeserializeModel<List<ModelBIM>>(xml);

			foreach (var model in models)
			{
				if (model?.Model != null)
				{
					model.Model.ReferenceElementsReconstruction();
				}
			}
			return models;
		}

		public static List<ModelBIM> ModelsFromFile(string filePath)
		{
			List<ModelBIM> models = IdeaRS.OpenModel.Tools.DeserializeModelFromFile<List<ModelBIM>>(filePath);

			foreach (var model in models)
			{
				if (model?.Model != null)
				{
					model.Model.ReferenceElementsReconstruction();
				}
			}
			return models;
		}
	}
}