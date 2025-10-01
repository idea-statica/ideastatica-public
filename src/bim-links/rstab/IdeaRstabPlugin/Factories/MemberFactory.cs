using Dlubal.RSTAB8;
using IdeaRS.OpenModel.Model;
using IdeaRstabPlugin.BimApi;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System.Collections.Generic;

namespace IdeaRstabPlugin.Factories
{
	internal class MemberFactory : IFactory<int, IIdeaMember1D>
	{
		private static readonly IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.factories.memberfactory");

		private readonly IResultsFactory _resultsFactory;
		private readonly IModelDataProvider _modelDataProvider;
		private readonly IElementFactory _elementFactory;
		private readonly IResultsProvider _resultsProvider;

		public MemberFactory(IResultsFactory resultsFactory,
					   IModelDataProvider modelDataProvider,
					   IElementFactory elementFactory,
					   IResultsProvider resultsProvider)
		{
			_resultsFactory = resultsFactory;
			_modelDataProvider = modelDataProvider;
			_elementFactory = elementFactory;
			_resultsProvider = resultsProvider;
		}

		public IIdeaMember1D Create(IObjectFactory objectFactory, IImportSession importSession, int memberNo)
		{
			Member member = _modelDataProvider.GetMember(memberNo);

			if (!CanImport(member))
			{
				return null;
			}

			_resultsProvider.Prefetch(member.No);

			var eccNo = member.EccentricityNo;
			(IdeaVector3D begin, IdeaVector3D end, InsertionPoints insertionPoint, EccentricityReference eccRef) ecc = GetEccentricity(eccNo, importSession);

			return new RstabMember(objectFactory,
						  _modelDataProvider,
						  _resultsFactory,
						  _elementFactory,
						  memberNo,
						  ecc.begin, ecc.end, ecc.insertionPoint, ecc.eccRef);
		}

		private static bool CanImport(Member member)
		{
			if (member.StartCrossSectionNo == 0)
			{
				_logger.LogDebug($"Member has no start cross-section, skipping.");
				return false;
			}

			return CanImportType(member.Type);
		}

		private static bool CanImportType(MemberType type)
		{
			switch (type)
			{
				case MemberType.UnknownMemberType:
				case MemberType.Rigid:
				case MemberType.CouplingRigidRigid:
				case MemberType.CouplingRigidHinge:
				case MemberType.CouplingHingeHinge:
				case MemberType.CouplingHingeRigid:
				case MemberType.NullMember:
					_logger.LogDebug($"Member is of unsupported type '{type}', skipping.");
					return false;
			}

			return true;
		}

		private (IdeaVector3D, IdeaVector3D, InsertionPoints, EccentricityReference) GetEccentricity(int eccNo, IImportSession importSession)
		{
			// member without any ecc 
			if (eccNo <= 0)
			{
				return DefaultEcc();
			}

			Dlubal.RSTAB8.MemberEccentricity dlubalEcc = _modelDataProvider.GetMemberEccentricity(eccNo);

			// ecc set realtively to another object - unsupported case
			if (dlubalEcc.TransverseOffset)
			{
				return DefaultEcc();
			}

			// insertion point
			var insertionPoint = GetInsertionPoint(dlubalEcc);

			// coord system
			var (valid, eccRef) = GetEccCoordSystem(dlubalEcc.ReferenceSystem);
			if (!valid)
			{
				return DefaultEcc();
			}

			var eccVectors = GetAbsoluteEcc(dlubalEcc, eccRef, importSession);

			return (eccVectors.begin, eccVectors.end, insertionPoint, eccRef);
		}

		private (IdeaVector3D begin, IdeaVector3D end) GetAbsoluteEcc(MemberEccentricity ecc, EccentricityReference eccRef, IImportSession importSession)
		{
			if (eccRef == EccentricityReference.GlobalCoordinateSystem && importSession.IsGCSOrientedUpwards == false)
			{
				return (new IdeaVector3D(ecc.Start.X, -ecc.Start.Y, -ecc.Start.Z), new IdeaVector3D(ecc.End.X, -ecc.End.Y, -ecc.End.Z));
			}
			else
			{
				return (new IdeaVector3D(ecc.Start.X, ecc.Start.Y, ecc.Start.Z), new IdeaVector3D(ecc.End.X, ecc.End.Y, ecc.End.Z));
			}
		}

		private static (bool valid, EccentricityReference coordSys) GetEccCoordSystem(ReferenceSystemType eccRef)
		{
			switch (eccRef)
			{
				case ReferenceSystemType.GlobalSystemType:
					return (true, EccentricityReference.GlobalCoordinateSystem);
				case ReferenceSystemType.LocalSystemType:
					return (true, EccentricityReference.LocalCoordinateSystem);
				default:
					return (false, EccentricityReference.GlobalCoordinateSystem);
			}
		}		

		private static InsertionPoints GetInsertionPoint(MemberEccentricity ecc)
		{

			if (_insertionPointMap.TryGetValue((ecc.HorizontalAlignment, ecc.VerticalAlignment), out var result))
			{
				return result;
			}

			// default
			return InsertionPoints.CenterOfGravity;			
		}

		private static readonly Dictionary<(HorizontalAlignmentType, VerticalAlignmentType), InsertionPoints>
			_insertionPointMap = new Dictionary<(HorizontalAlignmentType, VerticalAlignmentType), InsertionPoints>
			{
				{(HorizontalAlignmentType.Left, VerticalAlignmentType.Top), InsertionPoints.BottomLeft},
				{(HorizontalAlignmentType.Left, VerticalAlignmentType.Middle), InsertionPoints.Left},
				{(HorizontalAlignmentType.Left, VerticalAlignmentType.Bottom), InsertionPoints.TopLeft},
				{(HorizontalAlignmentType.Center, VerticalAlignmentType.Top), InsertionPoints.Bottom},
				{(HorizontalAlignmentType.Center, VerticalAlignmentType.Middle), InsertionPoints.CenterOfGravity},
				{(HorizontalAlignmentType.Center, VerticalAlignmentType.Bottom), InsertionPoints.Top},
				{(HorizontalAlignmentType.Right, VerticalAlignmentType.Top), InsertionPoints.BottomRight},
				{(HorizontalAlignmentType.Right, VerticalAlignmentType.Middle), InsertionPoints.Right},
				{(HorizontalAlignmentType.Right,  VerticalAlignmentType.Bottom), InsertionPoints.TopRight},
			};

		private static (IdeaVector3D, IdeaVector3D, InsertionPoints, EccentricityReference) DefaultEcc()
			=> (new IdeaVector3D(0, 0, 0), new IdeaVector3D(0, 0, 0), InsertionPoints.CenterOfGravity, EccentricityReference.GlobalCoordinateSystem);
	}
}