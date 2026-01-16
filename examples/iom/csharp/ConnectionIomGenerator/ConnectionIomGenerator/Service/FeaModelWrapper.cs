using ConnectionIomGenerator.Fea;
using IdeaRS.OpenModel;
using IdeaStatiCa.BimApi;

namespace ConnectionIomGenerator.Service
{
	/// <summary>
	/// Wrapper for FeaModel that implements <see cref="IIdeaModel"/> interface for BimImporter
	/// </summary>
	internal class FeaModelWrapper : IIdeaModel
	{
		private readonly FeaModel _feaModel;

		public FeaModelWrapper(FeaModel feaModel)
		{
			_feaModel = feaModel ?? throw new ArgumentNullException(nameof(feaModel));
		}

		public ISet<IIdeaMember1D> GetMembers()
		{
			return new HashSet<IIdeaMember1D>(_feaModel.Members1D.Values);
		}

		public ISet<IIdeaLoading> GetLoads()
		{
			// No loads in current implementation
			return new HashSet<IIdeaLoading>(_feaModel.Loading.Values);
		}

		public OriginSettings GetOriginSettings()
		{
			return new OriginSettings()
			{
				CrossSectionConversionTable = CrossSectionConversionTable.NoUsed,
				ProjectName = "Connection",
				ProjectDescription = "Generated connection model",
				Author = "ConnectionIomGenerator"
			};
		}

		/// <summary>
		/// Returns explicitly defined connection points from the FeaModel.
		/// Only returns nodes and members that are part of the connection points to prevent auto-creation of additional connections.
		/// </summary>
		public BulkSelection GetBulkSelection()
		{
			// Get all connection points
			var connectionPoints = new HashSet<IIdeaConnectionPoint>(_feaModel.ConnectionPoints.Values);

			// Only include nodes and members that are part of connection points
			var nodes = new HashSet<IIdeaNode>();
			var members = new HashSet<IIdeaMember1D>();

			foreach (var connectionPoint in connectionPoints)
			{
				// Add the connection node
				nodes.Add(connectionPoint.Node);

				// Add all connected members
				foreach (var connectedMember in connectionPoint.ConnectedMembers)
				{
					if (connectedMember.IdeaMember != null)
					{
						members.Add(connectedMember.IdeaMember);

						// Add all nodes from the member's segments
						foreach (var element in connectedMember.IdeaMember.Elements)
						{
							if (element.Segment != null)
							{
								nodes.Add(element.Segment.StartNode);
								nodes.Add(element.Segment.EndNode);
							}
						}
					}
				}
			}

			return new BulkSelection(nodes, members, connectionPoints);
		}

		/// <summary>
		/// Returns all nodes, members and connection points from the FeaModel.
		/// Same as GetBulkSelection for this implementation.
		/// </summary>
		public BulkSelection GetWholeModel()
		{
			return GetBulkSelection();
		}

		/// <summary>
		/// Not implemented - bulk selection is used instead
		/// </summary>
		public SingleSelection GetSingleSelection()
		{
			throw new System.NotImplementedException("Single selection is not supported. Use GetBulkSelection instead.");
		}
	}
}