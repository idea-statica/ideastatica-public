using yjk.FeaApis;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using IdeaStatiCa.Plugin;
using yjk.ViewModels;

namespace yjk.Importers
{
	internal class LoadGroupImporter : IntIdentifierImporter<IIdeaLoadGroup>
	{
		private readonly IFeaLoadsApi loadsApi;
		private readonly IPluginLogger _logger = AppLogger.Instance;

		public LoadGroupImporter(IFeaLoadsApi loadsApi)
		{
			this.loadsApi = loadsApi;
		}

		public override IIdeaLoadGroup Create(int id)
		{
			IFeaLoadGroup loadGroup = loadsApi.GetLoadGroup(id);
			var groupType = GetLoadGroupType(loadGroup.LoadGroupCategory, id);
			_logger.LogInformation($"LoadGroup created: id={id}, name={loadGroup.Name}, type={groupType}");
			return new IdeaLoadGroup(id)
			{
				Name = loadGroup.Name,
				GroupType = groupType
			};
		}

		private IdeaRS.OpenModel.Loading.LoadGroupType GetLoadGroupType(LoadGroupCategory loadGroupCategory, int id)
		{
			switch(loadGroupCategory)
			{
				case LoadGroupCategory.Permanent:
					return IdeaRS.OpenModel.Loading.LoadGroupType.Permanent;
				case LoadGroupCategory.Variable:
					return IdeaRS.OpenModel.Loading.LoadGroupType.Variable;
				default:
					_logger.LogWarning($"LoadGroup id={id}: unrecognised category {loadGroupCategory}, defaulting to Permanent");
					return IdeaRS.OpenModel.Loading.LoadGroupType.Permanent;
			}
		}
	}
}
