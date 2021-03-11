using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using System;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class MemberImporter : AbstractImporter<IIdeaMember1D>
	{
		private readonly IImporter<IIdeaElement1D> _elementImporter;

		public MemberImporter(IImporter<IIdeaElement1D> elementImporter)
		{
			_elementImporter = elementImporter;
		}

		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaMember1D member)
		{
			if(member.Elements.Count == 0)
			{
				throw new ConstraintException("A member has to specify at least one element.");
			}

			Member1D iomMember = new Member1D
			{
				Name = member.Name,
				Member1DType = member.Type,
				Elements1D = member.Elements
				.Select(x => _elementImporter.Import(ctx, x))
				.ToList()
			};

			return iomMember;
		}
	}
}