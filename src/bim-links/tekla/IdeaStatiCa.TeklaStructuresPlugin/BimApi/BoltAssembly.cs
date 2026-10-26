using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;

namespace IdeaStatiCa.TeklaStructuresPlugin.BimApi
{
	internal class BoltAssembly : IdeaBoltAssemblyByParameters
	{
		public BoltAssembly(Identifier<IIdeaBoltAssemblyByParameters> identifier) : base(identifier)
		{ }

		public BoltAssembly(string id) : base(id)
		{ }

		public override IIdeaMaterial BoltGrade
		{
			get
			{
				var boltGrade = Get<IIdeaMaterial>(BoltGradeNo);
				(boltGrade as IdeaMaterialByName).MaterialType = MaterialType.BoltGrade;
				return boltGrade;
			}
		}

		public string BoltGradeNo { get; set; }
	}
}
