using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class MemberImporter : AbstractImporter<IIdeaMember1D>
	{
		protected override OpenElementId ImportInternal(ImportContext ctx, IIdeaMember1D member)
		{
			if (member.Elements.Count == 0)
			{
				throw new ConstraintException("A member has to specify at least one element.");
			}

			CheckElementListConstraints(member.Elements);

			Member1D iomMember = new Member1D
			{
				Name = member.Name,
				Member1DType = member.Type,
				Elements1D = member.Elements
					.Select(x => ctx.Import(x))
					.ToList()
			};

			return iomMember;
		}

		private void CheckElementListConstraints(List<IIdeaElement1D> elements)
		{
			IIdeaNode prevNode = elements[0].Segment.StartNode;

			for (int i = 0; i < elements.Count; i++)
			{
				IIdeaElement1D element = elements[i];

				if (element.Segment.StartNode != prevNode)
				{
					throw new ConstraintException();
				}

				prevNode = element.Segment.EndNode;

				for (int j = 0; j < i; j++)
				{
					if (element == elements[j])
					{
						throw new ConstraintException();
					}
				}
			}
		}
	}
}