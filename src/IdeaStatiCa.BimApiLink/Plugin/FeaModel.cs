using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	internal class FeaModelAdapter : IIdeaModel
	{
		private readonly IBimApiImporter _bimApiImporter;
		private readonly IFeaModel _feaModel;

		public FeaModelAdapter(IBimApiImporter bimApiImporter, IFeaModel feaModel)
		{
			_bimApiImporter = bimApiImporter;
			_feaModel = feaModel;
		}

		public ISet<IIdeaLoading> GetLoads()
		{

			return _feaModel.GetAllCombinations()
				.Select(x => _bimApiImporter.Get(x))
				.Cast<IIdeaLoading>()
				.ToHashSet();
		}

		public ISet<IIdeaMember1D> GetMembers()
		{
			return _feaModel.GetAllMembers()
				.Select(x => _bimApiImporter.Get(x))
				.Where(x => x != null)
				.ToHashSet();
		}

		public OriginSettings GetOriginSettings()
			=> _feaModel.GetOriginSettings();

		public BulkSelection GetBulkSelection()
		{
			FeaUserSelection selection = _feaModel.GetUserSelection();

			var nodes = selection.Nodes
				.Select(x => _bimApiImporter.Get(x))
				.Where(x => x != null)
				.ToHashSet();

			var members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.Where(x => x != null)
				.ToHashSet();

			var members2D = selection.Members2D
				.Select(x => _bimApiImporter.Get(x))
				.Where(x => x != null)
				.ToHashSet();

			var connectionPoints = new HashSet<IIdeaConnectionPoint>();

			return new BulkSelection(nodes, members, connectionPoints, members2D);
		}

		public SingleSelection GetSingleSelection()
		{
			throw new NotImplementedException();
		}

		public BulkSelection GetWholeModel()
		{
			throw new NotImplementedException();
		}
	}
}