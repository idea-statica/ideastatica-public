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

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out ISet<IIdeaConnectionPoint> connectionPoints)
		{
			FeaUserSelection selection = _feaModel.GetUserSelection();
			_lastSelection = selection;

			nodes = selection.Nodes
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.WhereNotNull()
				.ToHashSet();

			connectionPoints = new HashSet<IIdeaConnectionPoint>();
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out IIdeaConnectionPoint connectionPoint)
		{
			throw new NotImplementedException();
		}

		public void GetWholeModel(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members, out ISet<IIdeaConnectionPoint> connectionPoints)
		{
			throw new NotImplementedException();
		}
	}
}