using IdeaStatiCa.IntermediateModel.Extensions;
using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Downgrade.DownGradeSteps
{
	internal class StepTo202 : BaseStep
	{
		public static Version Version => new(2, 0, 2);


		public override void DoDownStep(SModel _model)
		{
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


			//move RefLineInCenterOfGravity from member 1D in to connection data
			var beamDataList = openModel.GetElements("Connections;ConnectionData;Beams;BeamData");
			var member1DList = openModel.GetElements("Member1D;Member1D");

			foreach (var member1DItem in member1DList)
			{
				var member1dId = member1DItem.GetElementValue("Id");

				var beamData = beamDataList.ToList().Find(bd =>
				{
					var beamDataId = bd.GetElementValue("Id");
					return member1dId == beamDataId;
				});

				//remove from beam data
				var refLineInCenterOfGravity = member1DItem.TakeElementProperty("RefLineInCenterOfGravity");

				//add property in to new object
				beamData.TryAddElementProperty(refLineInCenterOfGravity);

			}
		}

		public override Version GetVersion()
		{
			return StepTo202.Version;
		}
	}
}
