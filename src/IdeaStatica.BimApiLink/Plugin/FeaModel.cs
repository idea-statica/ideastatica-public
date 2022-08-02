using IdeaRS.OpenModel;
using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatica.BimApiLink.Importers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Plugin
{
	public class FeaUserSelection
	{
		public ICollection<Identifier<IIdeaNode>> Nodes { get; init; }
			= Array.Empty<Identifier<IIdeaNode>>();

		public ICollection<Identifier<IIdeaMember1D>> Members { get; init; }
			= Array.Empty<Identifier<IIdeaMember1D>>();

		public ICollection<Identifier<IIdeaLoading>> Loads { get; init; }
			= Array.Empty<Identifier<IIdeaLoading>>();
	}

	public interface IFeaModel
	{
		FeaUserSelection GetUserSelection();

		IEnumerable<Identifier<IIdeaMember1D>> GetAllMembers();

		OriginSettings GetOriginSettings();
	}

	internal class FeaModelAdapter : IIdeaModel
	{
		private FeaUserSelection? _lastSelection;

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
				throw new InvalidOperationException();
			}

			return _lastSelection.Loads
				.Select(x => _bimApiImporter.Get(x))
				.ToHashSet();
		}

		public ISet<IIdeaMember1D> GetMembers()
		{
			return _feaModel.GetAllMembers()
				.Select(x => _bimApiImporter.Get(x))
				.ToHashSet();
		}

		public void GetSelection(out ISet<IIdeaNode> nodes, out ISet<IIdeaMember1D> members)
		{
			FeaUserSelection selection = _feaModel.GetUserSelection();
			_lastSelection = selection;

			nodes = selection.Nodes
				.Select(x => _bimApiImporter.Get(x))
				.ToHashSet();

			members = selection.Members
				.Select(x => _bimApiImporter.Get(x))
				.ToHashSet();
		}

		public OriginSettings GetOriginSettings()
			=> _feaModel.GetOriginSettings();
	}
}