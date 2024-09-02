using BimApiLinkFeaExample.FeaExampleApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;

namespace BimApiLinkFeaExample.Importers
{
	internal class LoadGroupImporter : IntIdentifierImporter<IIdeaLoadGroup>
	{
		private readonly IFeaLoadsApi loadsApi;

		public LoadGroupImporter(IFeaLoadsApi loadsApi)
		{
			this.loadsApi = loadsApi;
		}

		public override IIdeaLoadGroup Create(int id)
		{
			IFeaLoadGroup loadGroup = loadsApi.GetLoadGroup(id);
			return new IdeaLoadGroup(id)
			{
				Name = loadGroup.Name,
				GroupType = GetLoadGroupType(loadGroup.LoadGroupCategory)
			};
		}

		private static IdeaRS.OpenModel.Loading.LoadGroupType GetLoadGroupType(LoadGroupCategory loadGroupCategory)
		{ 
			switch(loadGroupCategory) 
			{
				case LoadGroupCategory.Permanent:
					return IdeaRS.OpenModel.Loading.LoadGroupType.Permanent;
				case LoadGroupCategory.Variable:
					return IdeaRS.OpenModel.Loading.LoadGroupType.Variable;
				default:
					return IdeaRS.OpenModel.Loading.LoadGroupType.Permanent;
			}
		}
	}
}
