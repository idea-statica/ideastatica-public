using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class Cut : IdeaCut
	{
		public override IIdeaObject ModifiedObject => GetModifiedObject();


		private IIdeaObject GetModifiedObject()
		{
			IIdeaObject ideaObject = GetMaybe<IIdeaPlate>(ModifiedObjectNo);

			if (ideaObject != null)
			{
				return ideaObject;
			}

			ideaObject = GetMaybe<IIdeaConnectedMember>(new ConnectedMemberIdentifier<IIdeaConnectedMember>(ModifiedObjectNo));

			if (ideaObject != null)
			{
				return ideaObject;
			}
			return null;
		}
		public string ModifiedObjectNo { get; set; }

		public override IIdeaObject CuttingObject => GetCuttingObject();

		private IIdeaObject GetCuttingObject()
		{
			IIdeaObject ideaObject = GetMaybe<IIdeaWorkPlane>(CuttingObjectNo);

			if (ideaObject != null)
			{
				return ideaObject;
			}

			ideaObject = GetMaybe<IIdeaNegativePlate>(CuttingObjectNo);

			if (ideaObject != null)
			{
				return ideaObject;
			}

			ideaObject = GetMaybe<IIdeaPlate>(CuttingObjectNo);

			if (ideaObject != null)
			{
				return ideaObject;
			}

			ideaObject = GetMaybe<IIdeaConnectedMember>(new ConnectedMemberIdentifier<IIdeaConnectedMember>(CuttingObjectNo));

			if (ideaObject is IdeaConnectedMember connectedMember)
			{
				connectedMember.ConnectedMemberType = IdeaConnectedMemberType.Negative;
				connectedMember.AutoAddCutByWorkplane = false;


				//we cant cut by rod so rod transfer in to pipe
				if (connectedMember.IdeaMember is IIdeaMember1D mem
					&& mem.CrossSection is IdeaStatiCa.TeklaStructuresPlugin.BimApi.Library.CrossSectionByParameters cssParam
					&& cssParam.Type == IdeaRS.OpenModel.CrossSection.CrossSectionType.O)
				{

					var diam = (cssParam.Parameters.Single() as ParameterDouble).Value;

					CrossSectionParameter cssParameter = new CrossSectionParameter
					{
						Name = cssParam.Name,
					};

					cssParam.Parameters.Clear();
					IdeaRS.OpenModel.CrossSection.CrossSectionFactory.FillRolledCHS(cssParameter, 0.5 * diam, 0.001);
					cssParam.Type = CrossSectionType.RolledCHS;
					cssParam.Parameters = new System.Collections.Generic.HashSet<Parameter>(cssParameter.Parameters);
				}

				return ideaObject;
			}

			return null;
		}
		public string CuttingObjectNo { get; set; }

		public override IIdeaWeld Weld => !string.IsNullOrEmpty(WeldNo) ? Get<IIdeaWeld>(WeldNo) : null;

		public string WeldNo { get; set; }

		public Cut(string no)
			: base(no)
		{
		}
	}
}