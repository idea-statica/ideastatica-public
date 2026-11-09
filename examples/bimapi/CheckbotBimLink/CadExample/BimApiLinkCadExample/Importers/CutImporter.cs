using CadExampleApi;
using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using System;

namespace BimApiLinkCadExample.Importers
{
	/// <summary>
	/// Imports a cut: which part is cut, by which part, and how.
	/// </summary>
	internal class CutImporter : BaseImporter<IIdeaCut>
	{
		public CutImporter(ICadGeometryApi cad)
			: base(cad)
		{
		}

		/// <summary>
		/// Converts the cut with the given CAD id.
		/// </summary>
		public override IIdeaCut Create(int id)
		{
			CadCut cut = Cad.GetCut(id);

			return new IdeaCut(id)
			{
				ModifiedObject = GetPart(cut.CutPart),
				CuttingObject = GetPart(cut.CuttingPart),
				CutMethod = cut.IsSurfaceCut ? CutMethod.Surface : CutMethod.BoundingBox,
				CutOrientation = CutOrientation.Parallel,
				DistanceComparison = DistanceComparison.Closer,
			};
		}

		/// <summary>
		/// A member takes part in a connection as a connected member, so a cut refers to that.
		/// </summary>
		private IIdeaObject GetPart(ICadPart part)
		{
			switch (part)
			{
				case CadMember member:
					return Get(new ConnectedMemberIdentifier<IIdeaConnectedMember>(member.Id.ToString()));
				case CadPlate plate when plate.IsNegative:
					return Get<IIdeaNegativePlate>(plate.Id);
				case CadPlate plate:
					return Get<IIdeaPlate>(plate.Id);
				default:
					throw new NotSupportedException($"A {part.GetType().Name} cannot take part in a cut.");
			}
		}
	}
}
