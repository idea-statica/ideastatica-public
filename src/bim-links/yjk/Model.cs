using IdeaRS.OpenModel;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Plugin;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System.Collections.Generic;
using System.Linq;
using yjk.FeaApis;
using yjk.Helpers;
using System.Threading;
using CsToYjk;
using System.Windows.Controls.Primitives;
using IdeaRS.OpenModel.CrossSection;

namespace yjk
{
	internal class Model : IFeaModel
	{
		private readonly IFeaGeometryApi geometry;
		private readonly IFeaLoadsApi load;
		private readonly IFeaResultsApi result;
		private readonly IFeaCrossSectionApi crossSection;
		private readonly IFeaMaterialApi materialApi;
		private readonly IProgressMessaging messagingService;

		private static int _entered = 0;

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
			//Get selected IDs in YJK
			Dictionary<int, List<int>> selectedIds = geometry.GetSelectedIds();

			//Read model
			Hi_AddToAndReadYjk hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			ClrYJKSCommand yjkscmd = new ClrYJKSCommand();
			yjkscmd.CsRunCommand("yjk_save");
			APIData.Hi_DbModelData model = hi_AddToAndReadYjk.ReadFromYJK();

			//Get cross sections
			crossSection.ReadFromModel(model);

			//Marshal back to YJK thread
			YjkDispatcher.Invoke(() =>
			{
			if (Interlocked.Exchange(ref _entered, 1) == 1)
				return;

			try
			{
				var _YJKSUI = new ClrYJKSUI();
				_YJKSUI.CsQSetCurrentRibbonLabel("IDDSN_DSP");

				//Get load cases and combinations
				load.GetLoadCasesAndCombos();

				//Reset result
				result.ClearResults();

				geometry.GetSelected(selectedIds, load, result, crossSection, materialApi);

				}
				finally
				{
					_entered = 0;
				}


			});

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