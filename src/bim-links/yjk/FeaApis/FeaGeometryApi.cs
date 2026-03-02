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

		IEnumerable<int> GetMembersIdentifiers();

		IFeaNode GetNode(int id);

		IEnumerable<int> GetNodesIdentifiers();

		Dictionary<int, List<int>> GetSelectedIds();
		void GetSelected(Dictionary<int, List<int>> selectedIds, IFeaResultsApi results);
	}

	internal class FeaGeometryApi : IFeaGeometryApi
	{
		/*		private List<IFeaMember> _members = InitializeMembers();
				private List<IFeaNode> _nodes = InitializeNodes();*/

		private List<IFeaMember> _members;
		private List<IFeaNode> _nodes;
		private IFeaResultsApi _results;

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

		public void GetSelected(Dictionary<int, List<int>> selectedIds, IFeaResultsApi results)
		{
			_members = new List<IFeaMember>();
			_nodes = new List<IFeaNode>();
			_results = results;

			GetColumns(selectedIds);
			GetBeams(selectedIds);
			GetBraces(selectedIds);

			//For each nodes, see what members are connected to it
/*			GetConnectedColumns(_nodes);
			GetConnectedBeams(_nodes);
			GetConnectedBraces(_nodes);*/
		}

		private void GetConnectedColumns(List<IFeaNode> nodes)
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

					foreach(IFeaNode node in _nodes)
					{
						if (node.Id == j1 ||  node.Id == j2)
						{
							AddMember(idFlrColumns[j], j1, j2);

							//Record result (force)
							_results.SetResultForColumn(iFlr, idFlrColumns[j]);
						}
					}						
				}
			}
		}

		private void GetConnectedBeams(List<IFeaNode> nodes)
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
					int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);

					int j1 = 0;
					int j2 = 0;
					_cToSDesign.BeamJD(idFlrBeams[j], ref j1, ref j2);

					foreach (IFeaNode node in _nodes)
					{
						if (node.Id == j1 || node.Id == j2)
						{
							AddMember(idFlrBeams[j], j1, j2);

							//Record result (force)
							_results.SetResultForBeam(iFlr, idFlrBeams[j]);
						}
					}
										
				}
				
			}
		}

		private void GetConnectedBraces(List<IFeaNode> nodes)
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
					int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);

					int j1 = 0;
					int j2 = 0;
					_cToSDesign.BraceJD(idFlrBraces[j], ref j1, ref j2);

					foreach (IFeaNode node in _nodes)
					{
						if (node.Id == j1 || node.Id == j2)
						{
							AddMember(idFlrBraces[j], j1, j2);

							//Record result (force)
							_results.SetResultForBeam(iFlr, idFlrBraces[j]);
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

							AddMember(idFlrColumns[j], j1, j2);

							GetNodeDetailAndAdd(j1);
							GetNodeDetailAndAdd(j2);

							//Record result (force)
							_results.SetResultForColumn(iFlr, idFlrColumns[j]);
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
						int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);

						if (selectedIds[12].Contains(modellingId))
						{
							int j1 = 0;
							int j2 = 0;
							_cToSDesign.BeamJD(idFlrBeams[j], ref j1, ref j2);

							AddMember(idFlrBeams[j], j1, j2);

							GetNodeDetailAndAdd(j1);
							GetNodeDetailAndAdd(j2);

							//Record result (force)
							_results.SetResultForBeam(iFlr, idFlrBeams[j]);
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

							AddMember(idFlrBraces[j], j1, j2);

							GetNodeDetailAndAdd(j1);
							GetNodeDetailAndAdd(j2);

							//Record result (force)
							_results.SetResultForBrace(iFlr, idFlrBraces[j]);
						}
					}
				}
			}
		}

		private void AddMember(int memberId, int j1, int j2)
		{
			FeaMember member = new FeaMember(memberId, j1, j2, 1);

			_members.Add(member);
		}

		private void GetNodeDetailAndAdd(int nodeId)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();

			float x = 0;
			float y = 0;
			float z = 0;
			_hi_CToSDesign.XYZ(nodeId, ref x, ref y, ref z);

			FeaNode node = new FeaNode(nodeId, x, y, z);

			_nodes.Add(node);

		}



		public void GetSelectedMembers(Dictionary<int, List<int>> selectedIds)
		{
			List<IFeaMember> members = new List<IFeaMember>();

			/*            foreach (YjkObjectIdentifier columnSelectedIdent in columnSelectedIdents)
                        {
                            IFeaMember member = GetMemberDetail(nodeSelectedIdents, columnSelectedIdent, YjkModel.MemberTypeEnum.Column);
                            members.Add(member);
                        }

                        foreach (YjkObjectIdentifier beamSelectedIdent in beamSelectedIdents)
                        {
                            IFeaMember member = GetMemberDetail(nodeSelectedIdents, beamSelectedIdent, YjkModel.MemberTypeEnum.Beam);
                            members.Add(member);
                        }

                        foreach (YjkObjectIdentifier braceSelectedIdent in braceSelectedIdents)
                        {
                            IFeaMember member = GetMemberDetail(nodeSelectedIdents, braceSelectedIdent, YjkModel.MemberTypeEnum.Brace);
                            members.Add(member);
                        }*/

			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;
				int nBeam = _hi_CToSDesign.NBeam(iFlr);
				int nColumn = _hi_CToSDesign.NColumn(iFlr);
				int nBrace = _hi_CToSDesign.NBrace(iFlr);
				var idFlrBeams = _hi_CToSDesign.FlrBeams(iFlr, nBeam);
				var idFlrColumns = _hi_CToSDesign.FlrColumns(iFlr, nColumn);
				var idFlrBraces = _hi_CToSDesign.FlrBraces(iFlr, nBrace);

				for (int j = 0; j < nColumn; j++)
				{
					int no = _hi_CToSDesign.ColumnONO(idFlrColumns[j]);
					int flrNo = _hi_CToSDesign.ColumnOFlr(idFlrColumns[j]);
					int modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);

					//Check if column is selected
					if (selectedIds.ContainsKey(12))
					{
						if (selectedIds[12].Contains(modellingId))
						{
							int j1 = 0;
							int j2 = 0;
							_cToSDesign.ColumnJD(idFlrBeams[j], ref j1, ref j2);

							FeaMember member = new FeaMember(idFlrColumns[j], j1, j2, 1);
						}
					}
				}
			}
		}

		public IEnumerable<int> GetMembersIdentifiers() => _members.Select(m => m.Id);

		public IFeaNode GetNode(int id) => _nodes.FirstOrDefault(n => n.Id == id);

		public IEnumerable<int> GetNodesIdentifiers() => _nodes.Select(n => n.Id);

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