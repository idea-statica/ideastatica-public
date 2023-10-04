using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace BimApiLinkCadExample.BimApi
{
	internal class Cut : IdeaCut
	{
		public override IIdeaObject ModifiedObject => GetModifiedObject();


		private IIdeaObject GetModifiedObject()
		{
			IIdeaObject ideaObject = GetMaybe<IIdeaConnectedMember>(new ConnectedMemberIdentifier<IIdeaConnectedMember>(ModifiedObjectNo.ToString()));

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

		public int ModifiedObjectNo { get; set; }

		public override IIdeaObject CuttingObject => GetCuttingObject();

		private IIdeaObject GetCuttingObject()
		{
			//IIdeaObject ideaObject = GetMaybe<IIdeaWorkPlane>(CuttingObjectNo);

			//if (ideaObject != null)
			//{
			//	return ideaObject;
			//}

			//ideaObject = GetMaybe<IIdeaNegativePlate>(CuttingObjectNo);

			//if (ideaObject != null)
			//{
			//	return ideaObject;
			//}

			IIdeaObject ideaObject = GetMaybe<IIdeaPlate>(CuttingObjectNo);

			if (ideaObject != null)
			{
				return ideaObject;
			}

			ideaObject = GetMaybe<IIdeaConnectedMember>(new ConnectedMemberIdentifier<IIdeaConnectedMember>(CuttingObjectNo.ToString()));

			if (ideaObject != null)
			{
				return ideaObject;
			}

			return null;
		}
		public int CuttingObjectNo { get; set; }

		public override IIdeaWeld Weld => !string.IsNullOrEmpty(WeldNo) ? Get<IIdeaWeld>(WeldNo) : null;

		public string WeldNo { get; set; }

		public Cut(int no) : base(no)
		{
		}
	}
}
