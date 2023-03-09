using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatiCa.BimApi;
using Nito.Disposables.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatica.BimApiLink.Plugin
{
	internal class FeaModelAdapter : IIdeaModel
	{
		private FeaUserSelection _lastSelection;

		private readonly IBimApiImporter _bimApiImporter;
		private readonly IFeaModel _feaModel;

		public FeaModelAdapter(IBimApiImporter bimApiImporter, IFeaModel feaModel)
		{
			_bimApiImporter = bimApiImporter;
			_feaModel = feaModel;
		}

		public ISet<IIdeaLoading> GetLoads()
		{
			if (_lastSelection is null)
			{
				return new HashSet<IIdeaLoading>();
			}

			return _lastSelection.Combinations
				.Select(x => _bimApiImporter.Get(x))
				.Cast<IIdeaLoading>()
				.ToHashSet();
		}

		public ISet<IIdeaMember1D> GetMembers()
		{
			return _feaModel.GetAllMembers()
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();
		}

		public OriginSettings GetOriginSettings()
			=> _feaModel.GetOriginSettings();

		public BulkSelection GetBulkSelection()
		{
			FeaUserSelection selection = _feaModel.GetUserSelection();
			_lastSelection = selection;

			var nodes = selection.Nodes
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			var members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			var connectionPoints = new HashSet<IIdeaConnectionPoint>();

			return new BulkSelection(nodes, members, connectionPoints);
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