using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using System.Collections.Generic;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class FoldedPlate : IdeaFoldedPlate
	{
		public FoldedPlate(string no)
			: base(no)
		{
			Plates = new List<IIdeaPlate>();
			Bends = new List<IIdeaBend>();
		}
	}
}