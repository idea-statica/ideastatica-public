using IdeaStatiCa.BimApi;

namespace BimApiLinkCadExample.BimApi
{
	internal class NegativePlate : Plate, IIdeaNegativePlate
	{
		public NegativePlate(int no)
			: base(no)
		{
			Name = $"NP{No}";
		}
	}
}
