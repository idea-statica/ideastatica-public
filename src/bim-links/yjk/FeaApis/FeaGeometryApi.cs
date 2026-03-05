using APIData;
using CsToYjk;
using MathNet.Spatial.Euclidean;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using System.Windows;

namespace yjk.FeaApis
{
	public interface IFeaGeometryApi
	{
		IFeaMember GetMember(int id);

		(UnitVector3D X, UnitVector3D Y, UnitVector3D Z) GetMemberLcs(int id);

		IEnumerable<int> GetMembersSelectedIdentifiers();

		IFeaNode GetNode(int id);

		IEnumerable<int> GetNodesSelectedIdentifiers();

		Dictionary<int, List<int>> GetSelectedIds();
		void GetSelected(Dictionary<int, List<int>> selectedIds, IFeaLoadsApi loads, IFeaResultsApi results);
	}

	internal class FeaGeometryApi : IFeaGeometryApi
	{
		/*		private List<IFeaMember> _members = InitializeMembers();
				private List<IFeaNode> _nodes = InitializeNodes();*/

		private List<IFeaMember> _members;
		private List<IFeaNode> _nodesSelected;
		private List<IFeaNode> _nodes;
		private IFeaResultsApi _results;
		private IFeaLoadsApi _loads;

		public IFeaMember GetMember(int id) => _members.FirstOrDefault(m => m.Id == id);

		public (UnitVector3D X, UnitVector3D Y, UnitVector3D Z) GetMemberLcs(int id)
		{
			var member = GetMember(id);
			IFeaNode beg = GetNode(member.BeginNodeId);
			IFeaNode end = GetNode(member.EndNodeId);
			return CalculateMemberLcs(beg, end);
		}

		public Dictionary<int, List<int>> GetSelectedIds()
		{
			//YJK API
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();

			//Get selected IDs
			Dictionary<int, List<int>> selectedIds = new Dictionary<int, List<int>>();
			selectedIds = _hi_AddToAndReadYjk.GetSelectSetIDs();

			return selectedIds;
		}

		public void GetSelected(Dictionary<int, List<int>> selectedIds, IFeaLoadsApi loads, IFeaResultsApi results)
		{
			_members = new List<IFeaMember>();
			_nodesSelected = new List<IFeaNode>();
			_nodes = new List<IFeaNode>();
			_results = results;
			_loads = loads;

			GetColumns(selectedIds);
			GetBeams(selectedIds);
			GetBraces(selectedIds);

			//For each nodes, see what members are connected to it
			//Fixed the list of nodes to look at
			List<IFeaNode> nodesCopy = new List<IFeaNode>(_nodes);

			GetConnectedColumns(nodesCopy);
			GetConnectedBeams(nodesCopy);
			GetConnectedBraces(nodesCopy);
		}

		private void GetConnectedColumns(List<IFeaNode> nodesCopy)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;
				int nColumn = _hi_CToSDesign.NColumn(iFlr);
				var idFlrColumns = _hi_CToSDesign.FlrColumns(iFlr, nColumn);

				for (int j = 0; j < nColumn; j++)
				{
					int no = _hi_CToSDesign.ColumnONO(idFlrColumns[j]);
					int flrNo = _hi_CToSDesign.ColumnOFlr(idFlrColumns[j]);
					int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);

					int j1 = 0;
					int j2 = 0;
					_cToSDesign.ColumnJD(idFlrColumns[j], ref j1, ref j2);

					foreach (IFeaNode node in nodesCopy)
					{
						if (node.Id == j1 ||  node.Id == j2)
						{
							bool exists = _members.Any(p => p.Id == idFlrColumns[j]);

							if (!exists)
							{
								FeaMember member = AddMember(idFlrColumns[j], j1, j2, false);

								//Record result (force)
								_results.SetResultForColumn(iFlr, member, _loads);
							}
						}
					}						
				}
			}
		}

		private void GetConnectedBeams(List<IFeaNode> nodesCopy)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;
				int nBeam = _hi_CToSDesign.NBeam(iFlr);
				var idFlrBeams = _hi_CToSDesign.FlrBeams(iFlr, nBeam);

				for (int j = 0; j < nBeam; j++)
				{
					int no = _hi_CToSDesign.BeamONO(idFlrBeams[j]);
					int flrNo = _hi_CToSDesign.BeamOFlr(idFlrBeams[j]);
					int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_BEAM, no, flrNo);

					int j1 = 0;
					int j2 = 0;
					_cToSDesign.BeamJD(idFlrBeams[j], ref j1, ref j2);

					
					foreach (IFeaNode node in nodesCopy)
					{
						if (node.Id == j1 || node.Id == j2)
						{
							bool exists = _members.Any(p => p.Id == idFlrBeams[j]);

							if (!exists)
							{
								FeaMember member = AddMember(idFlrBeams[j], j1, j2, false);

								//Record result (force)
								_results.SetResultForBeam(iFlr, member, _loads);
							}
						}
					}
										
				}
				
			}
		}

		private void GetConnectedBraces(List<IFeaNode> nodesCopy)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;
				int nBrace = _hi_CToSDesign.NBrace(iFlr);
				var idFlrBraces = _hi_CToSDesign.FlrBraces(iFlr, nBrace);

				for (int j = 0; j < nBrace; j++)
				{
					int no = _hi_CToSDesign.BraceONO(idFlrBraces[j]);
					int flrNo = _hi_CToSDesign.BraceOFlr(idFlrBraces[j]);
					int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_BEAM, no, flrNo);

					int j1 = 0;
					int j2 = 0;
					_cToSDesign.BraceJD(idFlrBraces[j], ref j1, ref j2);

					foreach (IFeaNode node in nodesCopy)
					{
						if (node.Id == j1 || node.Id == j2)
						{
							bool exists = _members.Any(p => p.Id == idFlrBraces[j]);

							if (!exists)
							{
								FeaMember member = AddMember(idFlrBraces[j], j1, j2, false);

								//Record result (force)
								_results.SetResultForBeam(iFlr, member, _loads);
							}
						}
					}
					
				}
				
			}
		}

		private void GetColumns(Dictionary<int, List<int>> selectedIds)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;
				int nColumn = _hi_CToSDesign.NColumn(iFlr);
				var idFlrColumns = _hi_CToSDesign.FlrColumns(iFlr, nColumn);

				//Check if column is selected
				if (selectedIds.ContainsKey(11))
				{

					for (int j = 0; j < nColumn; j++)
					{
						int no = _hi_CToSDesign.ColumnONO(idFlrColumns[j]);
						int flrNo = _hi_CToSDesign.ColumnOFlr(idFlrColumns[j]);
						int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);

						if (selectedIds[11].Contains(modellingId))
						{
							int j1 = 0;
							int j2 = 0;
							_cToSDesign.ColumnJD(idFlrColumns[j], ref j1, ref j2);

							FeaMember member = AddMember(idFlrColumns[j], j1, j2, true);

							//Record result (force)
							_results.SetResultForColumn(iFlr, member, _loads);
						}
					}
				}
			}
		}

		private void GetBeams(Dictionary<int, List<int>> selectedIds)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;
				int nBeam = _hi_CToSDesign.NBeam(iFlr);
				var idFlrBeams = _hi_CToSDesign.FlrBeams(iFlr, nBeam);

				//Check if beam/brace is selected
				if (selectedIds.ContainsKey(12))
				{
					for (int j = 0; j < nBeam; j++)
					{
						int no = _hi_CToSDesign.BeamONO(idFlrBeams[j]);
						int flrNo = _hi_CToSDesign.BeamOFlr(idFlrBeams[j]);
						int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_BEAM, no, flrNo);

						if (selectedIds[12].Contains(modellingId))
						{
							int j1 = 0;
							int j2 = 0;
							_cToSDesign.BeamJD(idFlrBeams[j], ref j1, ref j2);

							FeaMember member = AddMember(idFlrBeams[j], j1, j2, true);

							//Record result (force)
							_results.SetResultForBeam(iFlr, member, _loads);
						}
					}
				}
			}
		}

		private void GetBraces(Dictionary<int, List<int>> selectedIds)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;
				int nBrace = _hi_CToSDesign.NBrace(iFlr);
				var idFlrBraces = _hi_CToSDesign.FlrBraces(iFlr, nBrace);

				//Check if beam/brace is selected
				if (selectedIds.ContainsKey(12))
				{
					for (int j = 0; j < nBrace; j++)
					{
						int no = _hi_CToSDesign.BraceONO(idFlrBraces[j]);
						int flrNo = _hi_CToSDesign.BraceOFlr(idFlrBraces[j]);
						int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);

						if (selectedIds[12].Contains(modellingId))
						{
							int j1 = 0;
							int j2 = 0;
							_cToSDesign.BraceJD(idFlrBraces[j], ref j1, ref j2);

							FeaMember member = AddMember(idFlrBraces[j], j1, j2, true);

							//Record result (force)
							_results.SetResultForBrace(iFlr, member, _loads);
						}
					}
				}
			}
		}

		private FeaMember AddMember(int memberId, int j1, int j2, bool addNodeSelected)
		{
			float x1 = 0;
			float y1 = 0;
			float z1 = 0;
			(x1, y1, z1) = GetNodeDetail(j1);

			float x2 = 0;
			float y2 = 0;
			float z2 = 0;
			(x2, y2, z2) = GetNodeDetail(j2);

			FeaMember member = new FeaMember(memberId, new FeaNode(j1, x1, y1, z1), new FeaNode(j2, x2, y2, z2), 1);
			_members.Add(member);

			AddNode(j1, x1, y1, z1, addNodeSelected);
			AddNode(j2, x2, y2, z2, addNodeSelected);

			return member;
		}

		private (float x, float y, float z) GetNodeDetail (int nodeId)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();

			float x = 0;
			float y = 0;
			float z = 0;
			_hi_CToSDesign.XYZ(nodeId, ref x, ref y, ref z);

			return (x, y, z);
		}

		private void AddNode(int nodeId, float x, float y, float z, bool addNodeSelected)
		{
			bool exists = _nodes.Any(p => p.Id == nodeId);

			if (!exists)
			{
				FeaNode node = new FeaNode(nodeId, x, y, z);

				_nodes.Add(node);

				if (addNodeSelected) { 
					_nodesSelected.Add(node);
				}

			}

		}

		public IEnumerable<int> GetMembersSelectedIdentifiers() => _members.Select(m => m.Id);

		public IFeaNode GetNode(int id) => _nodes.FirstOrDefault(n => n.Id == id);

		public IEnumerable<int> GetNodesSelectedIdentifiers() => _nodesSelected.Select(n => n.Id);

		private static (UnitVector3D X, UnitVector3D Y, UnitVector3D Z) CalculateMemberLcs(IFeaNode begin, IFeaNode end)
		{
			Vector3D memberX = end.Point - begin.Point;
			UnitVector3D globalZ = UnitVector3D.ZAxis;

			UnitVector3D memberY;

			if (memberX.IsParallelTo(globalZ))
			{
				// column
				memberY = UnitVector3D.YAxis;
			}
			else
			{
				// beam
				memberY = memberX.CrossProduct(globalZ).Normalize().Negate();
			}

			return (memberX.Normalize(), memberY, memberX.CrossProduct(memberY).Normalize());
		}

/*		private static List<IFeaMember> InitializeMembers()
		{
			return new List<IFeaMember>()
			{
				new FeaMember { Id = 1, BeginNode = 1, EndNode = 2, CrossSectionId = 1, },
				new FeaMember { Id = 2, BeginNode = 2, EndNode = 3, CrossSectionId = 1, },
			};
		}

		private static List<IFeaNode> InitializeNodes()
		{
			return new List<IFeaNode>()
			{
				new FeaNode(1, 0, 0, 0),
				new FeaNode(2, 0, 0, 3),
				new FeaNode(3, 5, 0, 3),
			};
		}*/
	}
}