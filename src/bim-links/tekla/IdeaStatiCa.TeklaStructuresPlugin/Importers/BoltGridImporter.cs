using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using System.Collections;
using System.Collections.Generic;
using TS = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using TSV = Tekla.Structures.TeklaStructuresSettings;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class BoltGridImporter : BaseImporter<IIdeaBoltGrid>
	{
		private static readonly string BoltGradeKey = "GRADE";
		private static readonly string BoltDiameterKey = "DIAMETER";
		private static readonly string BoltAssemblyNameKey = "TYPE";

		public BoltGridImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaBoltGrid Create(string id)
		{
			var item = Model.GetItemByHandler(id);
			if (item is TS.BoltGroup boltGroup)
			{
				var boltgrid = new BoltGrid(boltGroup.Identifier.GUID.ToString())
				{
					BoltAssembly = GetAssembly(boltGroup),
					BoltShearType = IdeaRS.OpenModel.Parameters.BoltShearType.Interaction,
					ConnectedParts = new List<IIdeaObjectConnectable>(),
					Positions = new List<IIdeaNode>(),
					Length = ((double)boltGroup.Length).MilimetersToMeters(),
				};

				//This test due to plate as member and we are not sure if its imported as plate or member
				CheckAndAddConnectedObject<IIdeaPlate>(boltGroup.PartToBoltTo, boltgrid);
				CheckAndAddConnectedObject<IIdeaMember1D>(boltGroup.PartToBoltTo, boltgrid);

				CheckAndAddConnectedObject<IIdeaPlate>(boltGroup.PartToBeBolted, boltgrid);
				CheckAndAddConnectedObject<IIdeaMember1D>(boltGroup.PartToBeBolted, boltgrid);

				if (boltGroup.OtherPartsToBolt != null)
				{
					foreach (var obj in boltGroup.OtherPartsToBolt)
					{
						if (!(obj is TS.Part otherPart))
						{
							continue;
						}
						CheckAndAddConnectedObject<IIdeaPlate>(otherPart, boltgrid);
						CheckAndAddConnectedObject<IIdeaMember1D>(otherPart, boltgrid);
					}
				}
				var boltCs = boltGroup.GetCoordinateSystem();
				var boltAxisZ = TSG.Vector.Cross(boltCs.AxisX, boltCs.AxisY);

				boltgrid.OriginNo = Model.GetPointId(boltCs.Origin);
				boltgrid.LocalCoordinateSystem = new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
				{
					VecX = new IdeaRS.OpenModel.Geometry3D.Vector3D
					{
						X = boltCs.AxisX.X,
						Y = boltCs.AxisX.Y,
						Z = boltCs.AxisX.Z
					},
					VecY = new IdeaRS.OpenModel.Geometry3D.Vector3D
					{
						X = boltCs.AxisY.X,
						Y = boltCs.AxisY.Y,
						Z = boltCs.AxisY.Z
					},
					VecZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
					{
						X = boltAxisZ.X,
						Y = boltAxisZ.Y,
						Z = boltAxisZ.Z

					}
				};

				var midPoints = boltGroup.BoltPositions;
				foreach (var p in midPoints)
				{
					var point = p as TSG.Point;
					var pointId = Model.GetPointId(point);
					(boltgrid.Positions as List<IIdeaNode>).Add(Get<IIdeaNode>(pointId));
				}

				return boltgrid;
			}
			else
			{
				return null;
			}
		}

		private void CheckAndAddConnectedObject<T>(TS.Part part, BoltGrid boltgrid)
			where T : IIdeaObjectConnectable
		{
			IIdeaObject ideaObject = CheckMaybe<T>(part.Identifier.GUID.ToString());
			if (ideaObject != null)
			{
				IIdeaObjectConnectable mainObject = GetMaybe<T>(part.Identifier.GUID.ToString());
				if (mainObject != null)
				{
					(boltgrid.ConnectedParts as List<IIdeaObjectConnectable>).Add(mainObject);
				}
			}
		}

		private IIdeaBoltAssembly GetAssembly(TS.BoltGroup boltGroup)
		{
			bool isImperialUnitPresented = false;
			TSV.GetAdvancedOption("XS_IMPERIAL", ref isImperialUnitPresented);

			var stringPropTable = new Hashtable();
			boltGroup.GetStringReportProperties(new ArrayList
				{
					BoltGradeKey,
					BoltAssemblyNameKey
				}, ref stringPropTable);

			string boltAssemblyName = (string)stringPropTable[BoltAssemblyNameKey];
			string boltGrade = (string)stringPropTable[BoltGradeKey] ?? boltAssemblyName;

			var doublePropTable = new Hashtable();
			boltGroup.GetDoubleReportProperties(new ArrayList
			{
				BoltDiameterKey
			}, ref doublePropTable);

			double boltDiameter = ((double)doublePropTable[BoltDiameterKey]).MilimetersToMeters();

			return new BoltAssembly(boltGroup.Identifier.GUID.ToString())
			{
				BoreHole = boltDiameter + 0.001,
				DiagonalHeadDiameter = boltDiameter * 1.7,
				Diameter = boltDiameter,
				NutThickness = 0.6 * boltDiameter,
				HeadDiameter = boltDiameter * 1.7,
				HeadHeight = 0.6 * boltDiameter,
				HoleDiameter = boltDiameter + 0.001,
				Standard = string.Empty,
				TensileStressArea = 0.0,
				BoltGradeNo = boltGrade,
				Name = $"{(isImperialUnitPresented ? boltDiameter.MetersToInchesFormated() : doublePropTable[BoltDiameterKey])} {boltAssemblyName}"
			};
		}
	}
}