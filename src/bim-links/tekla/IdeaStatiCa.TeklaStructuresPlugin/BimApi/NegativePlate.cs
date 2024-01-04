using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class NegativePlate : Plate, IIdeaNegativePlate
	{
		public NegativePlate(string no)
			: base(no)
		{
			Name = $"NP{No}";
		}
	}
}