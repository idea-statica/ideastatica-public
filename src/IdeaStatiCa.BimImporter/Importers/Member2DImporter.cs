using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class Member2DImporter : AbstractImporter<IIdeaMember2D>
	{
		public Member2DImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaMember2D member2D)
		{
			if (member2D.Elements2D.Count == 0)
			{
				throw new ConstraintException("A member2D has to specify at least one element2D.");
			}

			return new Member2D()
			{
				Name = member2D.Name,
				Elements2D = ImportElements2D(ctx, member2D.Elements2D),
			};
		}

		private List<ReferenceElement> ImportElements2D(IImportContext ctx, List<IIdeaElement2D> elements2D)
		{
			var refElements = new List<ReferenceElement>(elements2D.Count);

			for (int i = 0; i < elements2D.Count; i++)
			{
				var element = elements2D[i];
				var refElement = ctx.Import(element);

				// check that elements don't form a cycle
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
	}
}
