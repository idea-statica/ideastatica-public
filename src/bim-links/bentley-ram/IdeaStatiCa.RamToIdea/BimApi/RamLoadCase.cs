using IdeaRS.OpenModel.Loading;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.RamToIdea.Factories;
using IdeaStatiCa.RamToIdea.Providers;
using RAMDATAACCESSLib;
using System.Collections.Generic;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamLoadCase : IIdeaLoadCase
	{
		public RamLoadCase(IObjectFactory objectFactory, ILoadsProvider loadsProvider, int uid)
		{
			var ramLoadCase = loadsProvider.GetLoadCase(uid);
			Id = uid.ToString();
			Name = $"{ramLoadCase.strTypeLabel} ({ramLoadCase.strLoadCaseGroupLabel})";
			var (lt, ls) = GetLoadCaseType(ramLoadCase.eLoadType);
			LoadType = lt;
			Type = ls;
			if (ramLoadCase.eLoadType == ELoadCaseType.SeismicLCa)
			{
				Variable = VariableType.Seismicity;
			}

			LoadGroup = objectFactory.GetLoadGroup(ramLoadCase.strLoadCaseGroupLabel, LoadType == LoadCaseType.Permanent ? LoadGroupType.Permanent : LoadGroupType.Variable);
		}

		public LoadCaseType LoadType { get; }

		public LoadCaseSubType Type { get; }

		public VariableType Variable { get; }

		public IIdeaLoadGroup LoadGroup { get; }

		public string Description { get; }

		public string Id { get; }

		public string Name { get; }

		public IEnumerable<IIdeaLoadOnSurface> LoadsOnSurface { get; set; }

		private static (LoadCaseType lt, LoadCaseSubType ls) GetLoadCaseType(ELoadCaseType ramLoadType)
		{
			switch (ramLoadType)
			{
				case ELoadCaseType.DeadLCa:
				case ELoadCaseType.ConstructionDeadLCa:
				case ELoadCaseType.NotionalDeadLCa:
				case ELoadCaseType.NotionalRoofLCa:
				case ELoadCaseType.MassDeadLCa:
					return (LoadCaseType.Permanent, LoadCaseSubType.PermanentSelfweight);

				case ELoadCaseType.LiveLCa:
				case ELoadCaseType.LiveReducibleLCa:
				case ELoadCaseType.LiveUnReducibleLCa:
				case ELoadCaseType.LiveStorageLCa:
				case ELoadCaseType.LiveRoofLCa:
				case ELoadCaseType.ConstructionLiveLCa:
				case ELoadCaseType.SnowLCa:
				case ELoadCaseType.SeismicLCa:
				case ELoadCaseType.WindLCa:
				case ELoadCaseType.NotionalLiveLCa:
				case ELoadCaseType.DynamicLCa:
				case ELoadCaseType.PartitionLCa:
					return (LoadCaseType.Variable, LoadCaseSubType.VariableStatic);

				case ELoadCaseType.BalancedLCa:
				case ELoadCaseType.VirtualLCa:
				case ELoadCaseType.OtherLCa:
					break;
			}

			return (LoadCaseType.Variable, LoadCaseSubType.VariableStatic);
		}
	}
}