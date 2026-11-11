using FeaExampleApi;
using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.BimApi;
using IdeaStatiCa.BimApiLink.Importers;
using System;

namespace BimApiLinkFeaExample.Importers
{
	/// <summary>
	/// Imports a load case by its FEA id, with its load group.
	/// </summary>
	internal class LoadCaseImporter : IntIdentifierImporter<IIdeaLoadCase>
	{
		private readonly IFeaLoadsApi _loads;

		public LoadCaseImporter(IFeaLoadsApi loads)
		{
			_loads = loads;
		}

		/// <summary>
		/// Converts the load case with the given FEA id. Its load group is imported by its own importer.
		/// </summary>
		public override IIdeaLoadCase Create(int id)
		{
			FeaLoadCase loadCase = _loads.GetLoadCase(id);

			return new IdeaLoadCase(id)
			{
				Name = loadCase.Name,
				LoadType = ToIdeaType(loadCase.Type),
				Type = ToIdeaSubType(loadCase.Type),
				Variable = VariableType.Standard,
				LoadGroup = Get<IIdeaLoadGroup>(loadCase.LoadGroupId),
			};
		}

		/// <summary>
		/// Whether IDEA StatiCa treats the load case as permanent or variable.
		/// </summary>
		private static LoadCaseType ToIdeaType(FeaLoadCaseType type)
		{
			switch (type)
			{
				case FeaLoadCaseType.SelfWeight:
				case FeaLoadCaseType.DeadLoad:
					return LoadCaseType.Permanent;
				case FeaLoadCaseType.Snow:
					return LoadCaseType.Variable;
				default:
					throw new NotSupportedException($"Load case type {type} is not supported.");
			}
		}

		/// <summary>
		/// The more detailed kind of the load case in IDEA StatiCa.
		/// </summary>
		private static LoadCaseSubType ToIdeaSubType(FeaLoadCaseType type)
		{
			switch (type)
			{
				case FeaLoadCaseType.SelfWeight:
					return LoadCaseSubType.PermanentSelfweight;
				case FeaLoadCaseType.DeadLoad:
					return LoadCaseSubType.PermanentStandard;
				case FeaLoadCaseType.Snow:
					return LoadCaseSubType.VariableStatic;
				default:
					throw new NotSupportedException($"Load case type {type} is not supported.");
			}
		}
	}
}
