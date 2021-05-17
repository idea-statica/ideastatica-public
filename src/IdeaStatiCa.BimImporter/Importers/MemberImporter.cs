using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class MemberImporter : AbstractImporter<IIdeaMember1D>
	{
		public MemberImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaMember1D member)
		{
			if (member.Elements.Count == 0)
			{
				throw new ConstraintException("A member has to specify at least one element.");
			}

			List<ReferenceElement> refElements = new List<ReferenceElement>();

			IIdeaNode prevNode = null;
			for (int i = 0; i < member.Elements.Count; i++)
			{
				IIdeaElement1D element = member.Elements[i];
				IIdeaSegment3D segment = element.Segment;

				// check that StartNode of n-th element is equal to EndNode of (n-1)-th element
				if (prevNode != null && segment.StartNode.Id != prevNode.Id)
				{
					throw new ConstraintException("StartNode of an element must be equal to EndNode of a previous element.");
				}

				prevNode = segment.EndNode;

				// check that elements don't form a cycle
				ReferenceElement refElement = ctx.Import(element);
				for (int j = 0; j < i; j++)
				{
					if (refElement == refElements[j])
					{
						throw new ConstraintException("Sequence of elements contains a cycle.");
					}
				}

				refElements.Add(refElement);
			}

			return new Member1D
			{
				Name = member.Name,
				Member1DType = member.Type,
				Elements1D = refElements
			};
		}
	}
}