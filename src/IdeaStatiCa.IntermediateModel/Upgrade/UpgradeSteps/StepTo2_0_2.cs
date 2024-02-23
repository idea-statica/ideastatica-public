using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Upgrade.UpgradeSteps
{
	internal class StepTo202 : BaseStep
	{
		public static Version Version => new(2, 0, 2);


		public override void DoUpStep(SModel _model)
		{
			base.DoUpStep(_model);

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


			//move RefLineInCenterOfGravity from connection data in to member 1D
			var beamDataList = openModel.GetElements("Connections;ConnectionData;Beams;BeamData");
			var member1DList = openModel.GetElements("Member1D;Member1D");

			foreach (var beamData in beamDataList)
			{
				var beamDataId = beamData.GetElementValue("Id");

				var member1DItem = member1DList.ToList().Find(m1d =>
				{
					var member1dId = m1d.GetElementValue("Id");
					return member1dId == beamDataId;
				});

				//remove from beam data
				var refLineInCenterOfGravity = beamData.TakeElementProperty("RefLineInCenterOfGravity");

				//add property in to new object
				member1DItem.TryAddElementProperty(refLineInCenterOfGravity);

			}

		}

		public override Version GetVersion()
		{
			return StepTo202.Version;
		}
	}
}
