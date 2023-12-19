using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class Cut : IdeaCut
	{
		public override IIdeaObject ModifiedObject => GetModifiedObject();


		private IIdeaObject GetModifiedObject()
		{
			IIdeaObject ideaObject = GetMaybe<IIdeaConnectedMember>(new ConnectedMemberIdentifier<IIdeaConnectedMember>(ModifiedObjectNo));

			if (ideaObject != null)
			{
				return ideaObject;
			}

			ideaObject = GetMaybe<IIdeaPlate>(ModifiedObjectNo);

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

			if (ideaObject != null)
			{
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