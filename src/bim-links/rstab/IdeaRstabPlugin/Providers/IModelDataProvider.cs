using Dlubal.RSTAB8;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Providers
{
	internal interface IModelDataProvider
	{
		Node GetNode(int no);

		Member GetMember(int no);

		IMember GetIMember(int no);

		IEnumerable<Member> GetMembers();

		//Line GetLine(int no);
	}
}