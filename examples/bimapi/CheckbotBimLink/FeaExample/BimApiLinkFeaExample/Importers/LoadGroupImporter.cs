using FeaExampleApi;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using System;

namespace BimApiLinkFeaExample.Importers
{
	/// <summary>
	/// Imports a load group by its FEA id.
	/// </summary>
	internal class LoadGroupImporter : IntIdentifierImporter<IIdeaLoadGroup>
	{
		private readonly IFeaLoadsApi _loads;

		public LoadGroupImporter(IFeaLoadsApi loads)
		{
			_loads = loads;
		}

		/// <summary>
		/// Converts the load group with the given FEA id.
		/// </summary>
		public override IIdeaLoadGroup Create(int id)
		{
			FeaLoadGroup loadGroup = _loads.GetLoadGroup(id);

			return new IdeaLoadGroup(id)
			{
				Name = loadGroup.Name,
				GroupType = ToIdea(loadGroup.Type),
			};
		}

		/// <summary>
		/// The IDEA StatiCa load group type for an FEA load group type.
		/// </summary>
		private static LoadGroupType ToIdea(FeaLoadGroupType type)
		{
			switch (type)
			{
				case FeaLoadGroupType.Permanent:
					return LoadGroupType.Permanent;
				case FeaLoadGroupType.Variable:
					return LoadGroupType.Variable;
				default:
					throw new NotSupportedException($"Load group type {type} is not supported.");
			}
		}
	}
}
