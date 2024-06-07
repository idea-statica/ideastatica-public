using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Model;
using IdeaStatica.TeklaStructuresPlugin.Utils;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi.Library;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using System;
using System.Linq;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using UnitVectorMN = MathNet.Spatial.Euclidean.UnitVector3D;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class MemberImporter : BaseImporter<IIdeaMember1D>
	{
		public MemberImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaMember1D Create(string id)
		{
			PlugInLogger.LogInformation($"MemberImporter create {id}");
			var member = Model.GetItemByHandler(id);
			if (member == null)
			{
				PlugInLogger.LogInformation($"MemberImporter not found member {id}");
				return null;
			}

			//BooleanPart OperativePart id aways point again on booleanPart so now rewrite item by operative part (real ContourPlate/beam)
			if (member is BooleanPart boolean)
			{
				member = boolean.OperativePart;
			}

			if (member is Part beam)
			{
				var ideaMember = new BimApi.Member1D(id)
				{
					Type = GetMemberType(beam),
					CrossSectionNo = $"{beam.Profile.ProfileString};{beam.Material.MaterialString}",
					Elements = new System.Collections.Generic.List<IIdeaElement1D>() { CreateElements(beam) },
					Length = GetMemberLength(beam),
				};

				MemberHelper.AdjustAngleMember(beam, ideaMember);

				if (ideaMember != null)
				{
					Model.CacheCreatedObject(new StringIdentifier<IIdeaMember1D>(id), ideaMember);
				}

				return ideaMember;
			}
			else
			{
				PlugInLogger.LogInformation($"MemberImporter not found member {id}");
				return null;
			}
		}

		protected (CoordSystemByVector LCS, double Rotation) GetLCSAndRotation(Part member)
		{
			PlugInLogger.LogInformation($"MemberImporter GetLCSAndRotation");
			return (GetMemberLCS(member), GetRotation(member));
		}
		private double GetRotation(Part beam)
		{
			PlugInLogger.LogInformation($"MemberImporter GetRotation");
			return Math.PI / 2;
		}
		private CoordSystemByVector GetMemberLCS(Part member)
		{
			PlugInLogger.LogInformation($"MemberImporter GetMemberLCS");
			var beamCs = member.GetCoordinateSystem();

			Vector beamLcsAxisX = beamCs.AxisX;
			beamLcsAxisX.Normalize();
			Vector beamLcsAxisY = beamCs.AxisY;
			beamLcsAxisY.Normalize();
			Vector beamLcsAxisZ = Vector.Cross(beamLcsAxisX, beamLcsAxisY);

			//adjust vector Y. make sure LCS is properly set
			Vector beamLcsAxisYNormalized = Vector.Cross(beamLcsAxisZ, beamLcsAxisX);
			Vector beamLcsAxisYNormalized2 = Vector.Cross(beamLcsAxisX, beamLcsAxisZ);

			if (beamLcsAxisYNormalized.GetAngleBetween(beamLcsAxisY) < beamLcsAxisYNormalized2.GetAngleBetween(beamLcsAxisY))
			{
				beamLcsAxisY = beamLcsAxisYNormalized;
			}
			else
			{
				beamLcsAxisY = beamLcsAxisYNormalized2;
			}

			UnitVectorMN axisX = UnitVectorMN.Create(
					beamLcsAxisX.X,
					beamLcsAxisX.Y,
					beamLcsAxisX.Z);

			UnitVectorMN axisY = UnitVectorMN.Create(
					beamLcsAxisY.X,
					beamLcsAxisY.Y,
					beamLcsAxisY.Z);

			UnitVectorMN axisZ = UnitVectorMN.Create(
					beamLcsAxisZ.X,
					beamLcsAxisZ.Y,
					beamLcsAxisZ.Z);


			PlugInLogger.LogInformation($"MemberImporter vectorX {axisX} vectorY {axisY} vectorZ{axisZ}");
			return new CoordSystemByVector()
			{
				VecX = axisX.ToIOM(),
				VecY = axisY.ToIOM(),
				VecZ = axisZ.ToIOM(),
			};
		}

		private static Member1DType GetMemberType(Part member)
		{
			return Member1DType.Beam;
		}

		private IIdeaElement1D CreateElements(Part member)
		{
			PlugInLogger.LogInformation($"MemberImporter CreateElements");
			(CoordSystemByVector lcs, double rotation) = GetLCSAndRotation(member);

			return new IdeaElement1D(member.Identifier.GUID.ToString())
			{
				Segment = CreateSegment(member, lcs),
				RotationRx = rotation,
				EccentricityBegin = GetEccentricity(member),
				EccentricityEnd = GetEccentricity(member)
			};
		}

		private IIdeaSegment3D CreateSegment(Part member, CoordSystemByVector lcs)
		{
			PlugInLogger.LogInformation($"MemberImporter CreateSegment");
			var segment = new BimApi.Segment3D(member.Identifier.GUID.ToString());

			var centLine = member.GetCenterLine(false);

			Point begNode = centLine[0] as Point;
			Point endNode = centLine[centLine.Count - 1] as Point;

			segment.StartNodeNo = Model.GetPointId(begNode);
			segment.EndNodeNo = Model.GetPointId(endNode);

			segment.LocalCoordinateSystem = lcs;
			return segment;
		}

		private double GetMemberLength(Part member)
		{
			PlugInLogger.LogInformation($"MemberImporter GetMemberLength");
			var centLine = member.GetCenterLine(false);

			Point begNode = centLine[0] as Point;
			Point endNode = centLine[centLine.Count - 1] as Point;
			var vector = new Vector(endNode - begNode);

			var memberLen = vector.GetLength();
			PlugInLogger.LogInformation($"MemberImporter GetMemberLength {memberLen}");
			return memberLen;
		}

		private IdeaVector3D GetEccentricity(Part beam)
		{
			PlugInLogger.LogInformation($"MemberImporter GetEccentricity");
			var css = Get<IIdeaCrossSection>($"{beam.Profile.ProfileString};{beam.Material.MaterialString}");

			var z = 0.0;
			var y = 0.0;

			//RolledT section has different mapping on centerline
			if (css is CrossSectionByParameters cssParam && cssParam.Type == IdeaRS.OpenModel.CrossSection.CrossSectionType.RolledT && beam is Tekla.Structures.Model.Beam teklaBeam)
			{
				var height = cssParam.Parameters.ToList().Find(x => x.Name == "H") as ParameterDouble;
				z = -(teklaBeam.StartPointOffset.Dz.MilimetersToMeters());
				y = -(teklaBeam.StartPointOffset.Dy.MilimetersToMeters()) / 2;

				if (teklaBeam.StartPointOffset.Dz > 0 || teklaBeam.StartPointOffset.Dz < 0 && height != null)
				{
					z -= height.Value / 2;
				}
			}

			PlugInLogger.LogInformation($"MemberImporter GetEccentricity y {y} z {z}");
			return new IdeaVector3D(0, y, z);
		}
	}
}
