using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Extensions;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter.Importers
{
	internal class CutImporter : AbstractImporter<IIdeaCut>
	{
		public CutImporter(IPluginLogger logger) : base(logger)
		{
		}

		protected override object ImportInternal(IImportContext ctx, IIdeaCut cut, ConnectionData connectionData)
		{

			if (cut.CuttingObject is IIdeaWorkPlane workPlane)
			{
				var cutData = new CutData()
				{
					NormalVector = workPlane.Normal.ToIOMVector(),
					Offset = cut.Offset,
					Direction = cut.CutOrientation,
					PlanePoint = ctx.Import(workPlane.Origin).Element as Point3D,

				};
				var beamIOM = FindIOMObjectData(cut, connectionData);

				if (beamIOM is BeamData beam)
				{
					(beam.Cuts ?? (beam.Cuts = new List<CutData>())).Add(cutData);
				}

				return cutData;
			}
			else
			{
				OpenElementId itemIOM = FindIOMObjectData(cut, connectionData);

				if (itemIOM != null)
				{
					var cutIOM = new IdeaRS.OpenModel.Connection.CutBeamByBeamData
					{
						CuttingObject = new ReferenceElement(ctx.ImportConnectionItem(cut.CuttingObject, connectionData) as OpenElementId),
						ModifiedObject = new ReferenceElement(itemIOM),
						IsWeld = cut.Weld != null,
						Method = cut.CutMethod,
						Orientation = cut.CutOrientation,
						PlaneOnCuttingObject = cut.DistanceComparison,
						WeldThickness = (cut.Weld?.Thickness) ?? 0.0,
						WeldType = cut.Weld != null ? cut.Weld.WeldType : WeldType.Fillet,
						ExtendBeforeCut = cut.ExtendBeforeCut,
						Name = cut.Name,
					};


					(connectionData.CutBeamByBeams ?? (connectionData.CutBeamByBeams = new List<CutBeamByBeamData>())).Add(cutIOM);

					return cutIOM;
				}

				return null;
			}
		}

		private static OpenElementId FindIOMObjectData(IIdeaCut cut, ConnectionData connectionData)
		{
			var modifiedObject = cut.ModifiedObject;

			// Find a beam based on the modified object
			var beam = FindElement(connectionData.Beams, modifiedObject, (b, m) =>
			{
				if (m is IIdeaConnectedMember cm)
					return b.OriginalModelId == cm.IdeaMember.Id;
				else
					return b.OriginalModelId == m.Id;
			});

			if (beam != null)
				return beam;

			// Find a plate based on the modified object
			var plate = FindElement(connectionData.Plates, modifiedObject, (p, m) =>
			{
				return m is IIdeaPlate pl && p.OriginalModelId == pl.Id;
			});

			return plate;
		}

		// Generic method to find an element in a list based on a condition
		private static T FindElement<T>(IEnumerable<T> elements, IIdeaObject modifiedObject, Func<T, IIdeaObject, bool> condition)
		{
			return elements.FirstOrDefault(element => condition(element, modifiedObject));
		}

		protected override OpenElementId ImportInternal(IImportContext ctx, IIdeaCut cut)
		{
			throw new System.NotImplementedException();
		}

	}
}
