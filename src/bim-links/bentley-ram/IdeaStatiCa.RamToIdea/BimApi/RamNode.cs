using RAMDATAACCESSLib;
using IdeaStatiCa.RamToIdea.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	/// <summary>
	/// Implementation of <see cref="IIdeaNode"/> for RSTAB.
	/// </summary>
	internal class RamNode : IIdeaNode
	{
		//TODO
		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

		/// <summary>
		/// Position of the node in cartesian coordinates.
		/// </summary>
		public IdeaVector3D Vector => GetNodePosition();

		/// <summary>
		/// Unique object id.
		/// </summary>
		public string Id => "node-" + Name;

		/// <summary>
		/// Name of the node.
		/// </summary>
		public string Name => No.ToString();

		public IIdeaPersistenceToken Token { get; }

		public int No { get; }

		private readonly IImportSession _importSession;
		private readonly IModelDataProvider _modelDataProvider;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="model">RSTAB model, required to get orientation of Z axis.</param>
		/// <param name="node">RSTAB node to convert.</param>
		/// <exception cref="NotImplementedException">If <see cref="Node.CS"/> is not
		/// Cartesian, XCylindrical, YCylindrical, ZCylindrical, or Polar.</exception>
		public RamNode(IImportSession importSession, IModelDataProvider modelDataProvider, int nodeNo)
		{
			_importSession = importSession;
			_modelDataProvider = modelDataProvider;

			No = nodeNo;
			Token = new PersistenceToken(TokenObjectType.Node, No);

			//TODO
			//_logger.LogDebug($"Created {nameof(RamNode)} with id {Id}");
		}

		private INode GetData()
		{
			return _modelDataProvider.GetNode(No);
		}

		private IdeaVector3D GetNodePosition()
		{
			IdeaVector3D pos = GetCartesianPosition();

			if (!_importSession.IsGCSOrientedUpwards)
			{
				// RSTAB rotates the Y axis to keep the coordinate system right handed.
				return new IdeaVector3D(pos.X, -pos.Y, -pos.Z);
			}

			return pos;
		}

		private IdeaVector3D GetCartesianPosition()
		{
			//double r, theta, phi;

			INode nodeData = GetData();

			return new IdeaVector3D(nodeData.sLocation.dXLoc, nodeData.sLocation.dYLoc, nodeData.sLocation.dZLoc);

			//switch (nodeData.CS)
			//{
			//	case CoordinateSystemType.Cartesian:
			//		return new IdeaVector3D(nodeData.X, nodeData.Y, nodeData.Z);

			//	case CoordinateSystemType.XCylindrical:
			//		r = nodeData.Y;
			//		theta = nodeData.Z;
			//		return new IdeaVector3D(nodeData.X, Math.Cos(theta) * r, Math.Sin(theta) * r);

			//	case CoordinateSystemType.YCylindrical:
			//		r = nodeData.X;
			//		theta = nodeData.Z;
			//		return new IdeaVector3D(Math.Sin(theta) * r, nodeData.Y, Math.Cos(theta) * r);

			//	case CoordinateSystemType.ZCylindrical:
			//		r = nodeData.X;
			//		theta = nodeData.Y;
			//		return new IdeaVector3D(Math.Cos(theta) * r, Math.Sin(theta) * r, nodeData.Z);

			//	case CoordinateSystemType.Polar:
			//		r = nodeData.X;
			//		theta = nodeData.Y;
			//		phi = nodeData.Z;
			//		return new IdeaVector3D(
			//			Math.Sin(phi) * Math.Cos(theta) * r,
			//			Math.Sin(phi) * Math.Sin(theta) * r,
			//			Math.Cos(phi) * r);

			//	default:
			//		throw new NotImplementedException($"Unsupported coordinate system '{nodeData.CS}'.");
			//}
		}
	}
}
