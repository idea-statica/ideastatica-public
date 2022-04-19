using Dlubal.RSTAB8;
using IdeaRstabPlugin.Factories;
using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System;

namespace IdeaRstabPlugin.BimApi
{
	/// <summary>
	/// Implementation of <see cref="IIdeaNode"/> for RSTAB.
	/// </summary>
	internal class RstabNode : IIdeaNode
	{
		private static readonly IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.bimapi");

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
		private readonly ObjectFactory _objectFactory;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="model">RSTAB model, required to get orientation of Z axis.</param>
		/// <param name="node">RSTAB node to convert.</param>
		/// <exception cref="NotImplementedException">If <see cref="Node.CS"/> is not
		/// Cartesian, XCylindrical, YCylindrical, ZCylindrical, or Polar.</exception>
		public RstabNode(IImportSession importSession, IModelDataProvider modelDataProvider, ObjectFactory objectFactory,
			int nodeNo)
		{
			_importSession = importSession;
			_modelDataProvider = modelDataProvider;
			_objectFactory = objectFactory;

			No = nodeNo;
			Token = new PersistenceToken(TokenObjectType.Node, No);

			_logger.LogDebug($"Created {nameof(RstabNode)} with id {Id}");
		}

		private Node GetData()
		{
			return _modelDataProvider.GetNode(No);
		}

		private IdeaVector3D GetNodePosition()
		{
			IdeaVector3D pos = GetCartesianPosition();

			double x = pos.X, y = pos.Y, z = pos.Z;
			if (!_importSession.IsGCSOrientedUpwards)
			{
				// RSTAB rotates the Y axis to keep the coordinate system right handed.
				y = -y;
				z = -z;
			}

			Node data = GetData();

			int refNode = data.RefObjectNo;
			if (refNode != 0)
			{
				IdeaVector3D refNodePos = _objectFactory.GetNode(refNode).Vector;
				x += refNodePos.X;
				y += refNodePos.Y;
				z += refNodePos.Z;
			}

			return new IdeaVector3D(x, y, z);
		}

		private IdeaVector3D GetCartesianPosition()
		{
			double r, theta, phi;

			Node nodeData = GetData();

			switch (nodeData.CS)
			{
				case CoordinateSystemType.Cartesian:
					return new IdeaVector3D(nodeData.X, nodeData.Y, nodeData.Z);

				case CoordinateSystemType.XCylindrical:
					r = nodeData.Y;
					theta = nodeData.Z;
					return new IdeaVector3D(nodeData.X, Math.Cos(theta) * r, Math.Sin(theta) * r);

				case CoordinateSystemType.YCylindrical:
					r = nodeData.X;
					theta = nodeData.Z;
					return new IdeaVector3D(Math.Sin(theta) * r, nodeData.Y, Math.Cos(theta) * r);

				case CoordinateSystemType.ZCylindrical:
					r = nodeData.X;
					theta = nodeData.Y;
					return new IdeaVector3D(Math.Cos(theta) * r, Math.Sin(theta) * r, nodeData.Z);

				case CoordinateSystemType.Polar:
					r = nodeData.X;
					theta = nodeData.Y;
					phi = nodeData.Z;
					return new IdeaVector3D(
						Math.Sin(phi) * Math.Cos(theta) * r,
						Math.Sin(phi) * Math.Sin(theta) * r,
						Math.Cos(phi) * r);

				default:
					throw new NotImplementedException($"Unsupported coordinate system '{nodeData.CS}'.");
			}
		}
	}
}