using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamLoadGroup : IIdeaLoadGroup
	{
		public RamLoadGroup(string loadGroupName, LoadGroupType type)
		{
			Id = loadGroupName;
			GroupType = type;
		}

		public Relation Relation { get; set; } = Relation.Standard;

		public LoadGroupType GroupType { get; set; }

		public double GammaQ => 1;

		public double Dzeta => 1;

		public double GammaGInf => 1;

		public double GammaGSup => 1;

		public double Psi0 => 1;

		public double Psi1 => 1;

		public double Psi2 => 1;

		public string Id { get; }

		public string Name => "LG " + Id;
	}
}
