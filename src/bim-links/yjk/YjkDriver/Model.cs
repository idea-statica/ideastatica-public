using CsToYjk;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.CrossSection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using yjk.FeaApis;
using yjk.Helpers;
using yjk.ViewModels;

namespace yjk
{
	public class Model : IFeaModel
	{
		private readonly IFeaGeometryApi geometry;
		private readonly IFeaLoadsApi load;
		private readonly IFeaResultsApi result;
		private readonly IFeaCrossSectionApi crossSection;
		private readonly IFeaMaterialApi materialApi;
		private readonly IProgressMessaging messagingService;
		private readonly IPluginLogger _logger = AppLogger.Instance;

		public Model(IFeaGeometryApi geometry, IFeaLoadsApi load, IFeaResultsApi result, IFeaCrossSectionApi crossSection, IFeaMaterialApi materialApi, IProgressMessaging messagingService)
		{
			this.geometry = geometry;
			this.load = load;
			this.result = result;
			this.crossSection = crossSection;
			this.materialApi = materialApi;
			this.messagingService = messagingService;
		}

		public OriginSettings GetOriginSettings()
		{
			return new OriginSettings()
			{
				ProjectName = "TestProject",
			};
		}

		/// <summary>
		/// Returns all members in the model to be able find all related (connected) members, when select node only.
		/// </summary>
		/// <returns>All members in the model to be able find all related (connected) members, when select node only.</returns>
		public IEnumerable<Identifier<IIdeaMember1D>> GetAllMembers()
		{
			return geometry.GetMembersSelectedIdentifiers()
				.Select(x => new IntIdentifier<IIdeaMember1D>(x));
		}

		/// <summary>
		/// Returns user selection...nodes, members, ...
		/// When nodes and members are selected, no more members are added from <see cref="GetAllMembers"/>.
		/// From all selected nodes (that containes at least one member) are created connections.
		/// When only members are selected, connection nodes are recognized automatically. From this nodes are created connections
		/// </summary>
		/// <returns></returns>
		public FeaUserSelection GetUserSelection()
		{
			_logger.LogInformation("Model.GetUserSelection");

			//Get selected IDs in YJK
			Dictionary<int, List<int>> selectedIds = geometry.GetSelectedIds();

			ReadModel();

			geometry.GetSelected(selectedIds, load, result, crossSection, materialApi);

			List<Identifier<IIdeaNode>> nodes = geometry.GetNodesSelectedIdentifiers()
					.Select(x => new IntIdentifier<IIdeaNode>(x))
					.Cast<Identifier<IIdeaNode>>()
					.ToList();

			
			List<Identifier<IIdeaMember1D>> members = geometry.GetMembersSelectedIdentifiers()
				.Select(x => new IntIdentifier<IIdeaMember1D>(x))
				.Cast<Identifier<IIdeaMember1D>>()
				.ToList();

			return new FeaUserSelection()
			{
				Members = members,
				Nodes = nodes			
			};
		}

		public void Refresh()
		{
			_logger.LogInformation("Model.Refresh");
			ReadModel();
			geometry.GetAll(load, result, crossSection, materialApi);
		}

		public void ReadModel()
		{
			//Read model DB
			_logger.LogInformation("Read model DB");

			Hi_AddToAndReadYjk hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			ClrYJKSCommand yjkscmd = new ClrYJKSCommand();
			yjkscmd.CsRunCommand("yjk_save");
			APIData.Hi_DbModelData model = hi_AddToAndReadYjk.ReadFromYJK();

			//Get cross sections
			_logger.LogInformation("From model DB, get cross sections information");
			geometry.ReadFromModelDB(model);

			//Marshal back to YJK thread
			YjkDispatcher.Invoke(() =>
			{
				_logger.LogInformation("Move YJK to design window");
				var _YJKSUI = new ClrYJKSUI();
				_YJKSUI.CsQSetCurrentRibbonLabel("IDDSN_DSP");
			});

			//Get load cases and combinations
			load.GetLoadCasesAndCombos();

			//Reset result
			_logger.LogInformation("Clear result");
			result.ClearResults();
		}

		public IEnumerable<Identifier<IIdeaCombiInput>> GetAllCombinations()
		{
			return load.GetLoadCombinationsIds()
				.Select(x => new IntIdentifier<IIdeaCombiInput>(x))
				.Cast<Identifier<IIdeaCombiInput>>()
				.ToList();
		}

		/// <inheritdoc cref="SelectUserSelection"/>
		public void SelectUserSelection(IEnumerable<Identifier<IIdeaNode>> nodes, IEnumerable<Identifier<IIdeaMember1D>> members) 
		{
			//feaApi.SelectNodes(nodes, members);
		}
	}
}