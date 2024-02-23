using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Upgrade.UpgradeSteps
{
	internal class StepTo203 : BaseStep
	{
		public static Version Version => new(2, 0, 3);


		public override void DoUpStep(SModel _model)
		{
			base.DoUpStep(_model);

			//move plate data from connection data in to new collection PlatesInIOMModel. Rename PlateData to Plate. Remove Plates from connection data
			var openModelColection = _model.GetElements("OpenModel");

			//check if its BimModel model
			if (!openModelColection.Any())
			{
				openModelColection = _model.GetElements("ModelBIM;Model");
			}

			if (openModelColection.Count() > 1)
			{
				throw new InvalidOperationException($"Model is not valid OpenModel defined {openModelColection.Count()} times");
			}

			if (openModelColection.Count() != 1 || openModelColection.First() is not SObject)
			{
				throw new InvalidOperationException("OpenModel is not valid");
			}

			var openModel = (openModelColection.First() as SObject);


			var platesInIOMModel = openModel.CreateElementProperty("PlatesInIOMModel");
			var platesInIOMModelList = platesInIOMModel.CreateListProperty("Plate");

			var connectionData = openModel.GetElements("Connections;ConnectionData");

			foreach (var connectionDataElement in connectionData)
			{
				var platesDataList = connectionDataElement.GetElements("Plates;PlateData");

				foreach (var plateData in platesDataList)
				{
					//rename
					plateData.ChangeElementPropertyName("PlateData", "Plate");

					//add to new collection
					platesInIOMModelList.Add(plateData);
				}
				//remove Plates from ConnectionData
				connectionDataElement.RemoveElementProperty("Plates");
			}
		}

		public override Version GetVersion()
		{
			return StepTo203.Version;
		}
	}
}
