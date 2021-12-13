using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.Providers
{
	internal interface IModelDataProvider
	{
		INode GetNode(int no);

		//Member GetMember(int no);

		IMember GetMember(int no);

		//IColumn GetColumn(int no);

		//IHorizBrace GetHorizBrace(int no);

		//IVerticalBrace GetVerticalBrace(int no);

		//IEnumerable<IBeam> GetBeams();

		//IEnumerable<IColumn> GetColumns();

		//IEnumerable<IHorizBrace> GetHorizantalBraces();

		//IEnumerable<IVerticalBrace> GetVerticalBraces();

		//Line GetLine(int no);
	}
}
