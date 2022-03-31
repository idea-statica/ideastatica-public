using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;

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

			List<IIdeaElement1D> elements = member.Elements;

			IIdeaCrossSection css;
#pragma warning disable CS0618 // Type or member is obsolete
			if (!(member.CrossSection is null))
			{
				css = member.CrossSection;
			}
			else if (!(elements[0].StartCrossSection is null))
			{
				Logger.LogInformation("Element cross-section are obsolete, use member cross-section or member spans for haunched members.");
				css = elements[0].StartCrossSection;
			}
			else
			{
				throw new ConstraintException("Cross-section cannot be null.");
			}
#pragma warning restore CS0618 // Type or member is obsolete

			return new Member1D
			{
				Name = member.Name,
				Member1DType = member.Type,
				Elements1D = ImportElements(ctx, elements),
				CrossSection = ctx.Import(css),
				Spans = ImportSpans(ctx, member.Spans)
			};
		}

		private List<ReferenceElement> ImportElements(IImportContext ctx, List<IIdeaElement1D> elements)
		{
			List<ReferenceElement> refElements = new List<ReferenceElement>(elements.Count);
			IIdeaNode prevNode = null;

			for (int i = 0; i < elements.Count; i++)
			{
				IIdeaElement1D element = elements[i];
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

			return refElements;
		}

		private List<ReferenceElement> ImportSpans(IImportContext ctx, IEnumerable<IIdeaSpan> spans)
		{
			return spans
				.Select(x => ctx.Import(x))
				.ToList();
		}
	}
}