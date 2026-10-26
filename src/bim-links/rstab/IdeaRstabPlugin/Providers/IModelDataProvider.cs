using Dlubal.RSTAB8;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Providers
{
	internal interface IModelDataProvider
	{
		Node GetNode(int no);

		Member GetMember(int no);

		IMember GetIMember(int no);

		IEnumerable<Member> GetMembers();

		(IdeaVector3D begin, IdeaVector3D end, InsertionPoints insertionPoint, EccentricityReference eccRef) GetMemberEccentricity(int no);

		//Line GetLine(int no);
	}
}