using APIData;
using CsToYjk;
using MathNet.Spatial.Euclidean;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using System.Windows;
using System;
using IdeaRS.OpenModel.CrossSection;

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
		void GetSelected(Dictionary<int, List<int>> selectedIds, IFeaLoadsApi load, IFeaResultsApi result, 
			IFeaCrossSectionApi crossSection, IFeaMaterialApi materialApi);
	}

	internal class FeaGeometryApi : IFeaGeometryApi
	{
		/*		private List<IFeaMember> _members = InitializeMembers();
				private List<IFeaNode> _nodes = InitializeNodes();*/

		private List<IFeaMember> _members;
		private List<IFeaNode> _nodesSelected;
		private List<IFeaNode> _nodes;
		private IFeaResultsApi _result;
		private IFeaLoadsApi _load;
		private IFeaCrossSectionApi _crossSection;
		private IFeaMaterialApi _materialApi;

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
			Hi_AddToAndReadYjk hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();

			//Get selected IDs
			Dictionary<int, List<int>> selectedIds = new Dictionary<int, List<int>>();
			selectedIds = hi_AddToAndReadYjk.GetSelectSetIDs();

			return selectedIds;
		}

		public void GetSelected(Dictionary<int, List<int>> selectedIds, IFeaLoadsApi load, IFeaResultsApi result, 
			IFeaCrossSectionApi crossSection, IFeaMaterialApi materialApi)
		{
			_members = new List<IFeaMember>();
			_nodesSelected = new List<IFeaNode>();
			_nodes = new List<IFeaNode>();
			_result = result;
			_load = load;
			_crossSection = crossSection;
			_materialApi = materialApi;

			GetMembers(selectedIds, MemberType.Column);
			GetMembers(selectedIds, MemberType.Beam);
			GetMembers(selectedIds, MemberType.Brace);

			//For each nodes, see what members are connected to it
			//Fixed the list of nodes to look at
			List<IFeaNode> nodesCopy = new List<IFeaNode>(_nodes);

			GetConnectedMembers(nodesCopy, MemberType.Column);
			GetConnectedMembers(nodesCopy, MemberType.Beam);
			GetConnectedMembers(nodesCopy, MemberType.Brace);
		}

		private void GetConnectedMembers(List<IFeaNode> nodesCopy, MemberType type)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;

				int nMember = 0;
				List<int> idFlrMembers = new List<int>();
				switch (type)
				{
					case MemberType.Column:
						nMember = _hi_CToSDesign.NColumn(iFlr);
						idFlrMembers = _hi_CToSDesign.FlrColumns(iFlr, nMember);
						break;
					case MemberType.Beam:
						nMember = _hi_CToSDesign.NBeam(iFlr);
						idFlrMembers = _hi_CToSDesign.FlrBeams(iFlr, nMember);
						break;
					case MemberType.Brace:
						nMember = _hi_CToSDesign.NBrace(iFlr);
						idFlrMembers = _hi_CToSDesign.FlrBraces(iFlr, nMember);
						break;
				}

				for (int j = 0; j < nMember; j++)
				{
					int j1 = 0;
					int j2 = 0;
					FeaMember member = null;

					switch (type)
					{
						case MemberType.Column:
							_cToSDesign.ColumnJD(idFlrMembers[j], ref j1, ref j2);
							break;
						case MemberType.Beam:
							_cToSDesign.BeamJD(idFlrMembers[j], ref j1, ref j2);
							break;
						case MemberType.Brace:
							_cToSDesign.BraceJD(idFlrMembers[j], ref j1, ref j2);
							break;
					}

					foreach (IFeaNode node in nodesCopy)
					{
						if (node.Id == j1 || node.Id == j2)
						{
							bool exists = _members.Any(p => p.Id == idFlrMembers[j]);

							if (!exists)
							{
								member = AddMember(idFlrMembers[j], j1, j2, false, type);

								switch (type)
								{
									case MemberType.Column:
										//Record result (force)
										_result.SetResultForColumn(iFlr, member, _load);
										break;
									case MemberType.Beam:
										//Record result (force)
										_result.SetResultForBeam(iFlr, member, _load);
										break;
									case MemberType.Brace:
										//Record result (force)
										_result.SetResultForBrace(iFlr, member, _load);
										break;
								}
							}
						}
					}
				}
			}
		}

		private void GetMembers(Dictionary<int, List<int>> selectedIds, MemberType type)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();
			var _hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			var _cToSDesign = new Hi_CToSDesign();

			int numFloor = _hi_CToSDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;

				int nMember = 0;
				List<int> idFlrMembers = new List<int>();
				int keyToCheck = 0;
				switch (type)
				{
					case MemberType.Column:
						nMember = _hi_CToSDesign.NColumn(iFlr);
						idFlrMembers = _hi_CToSDesign.FlrColumns(iFlr, nMember);
						keyToCheck = 11;
						break;
					case MemberType.Beam:
						nMember = _hi_CToSDesign.NBeam(iFlr);
						idFlrMembers = _hi_CToSDesign.FlrBeams(iFlr, nMember);
						keyToCheck = 12;
						break;
					case MemberType.Brace:
						nMember = _hi_CToSDesign.NBrace(iFlr);
						idFlrMembers = _hi_CToSDesign.FlrBraces(iFlr, nMember);
						keyToCheck = 12;
						break;
				}

				//Check if column is selected
				if (selectedIds.ContainsKey(keyToCheck))
				{

					for (int j = 0; j < nMember; j++)
					{
						int no = 0;
						int flrNo = 0;
						int modellingId = 0;

						switch (type)
						{
							case MemberType.Column:
								no = _hi_CToSDesign.ColumnONO(idFlrMembers[j]);
								flrNo = _hi_CToSDesign.ColumnOFlr(idFlrMembers[j]);
								modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);
								break;
							case MemberType.Beam:
								no = _hi_CToSDesign.BeamONO(idFlrMembers[j]);
								flrNo = _hi_CToSDesign.BeamOFlr(idFlrMembers[j]);
								modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_BEAM, no, flrNo);
								break;
							case MemberType.Brace:
								no = _hi_CToSDesign.BraceONO(idFlrMembers[j]);
								flrNo = _hi_CToSDesign.BraceOFlr(idFlrMembers[j]);
								modellingId = _hi_AddToAndReadYjk.ReadIdByNO(GjKind.IDK_BEAM, no, flrNo);
								break;
						}

						if (selectedIds[keyToCheck].Contains(modellingId))
						{
							int j1 = 0;
							int j2 = 0;
							FeaMember member = null;

							switch (type)
							{
								case MemberType.Column:
									_cToSDesign.ColumnJD(idFlrMembers[j], ref j1, ref j2);
									member = AddMember(idFlrMembers[j], j1, j2, true, type);
									//Record result (force)
									_result.SetResultForColumn(iFlr, member, _load);
									break;
								case MemberType.Beam:
									_cToSDesign.BeamJD(idFlrMembers[j], ref j1, ref j2);
									member = AddMember(idFlrMembers[j], j1, j2, true, type);
									//Record result (force)
									_result.SetResultForBeam(iFlr, member, _load);
									break;
								case MemberType.Brace:
									_cToSDesign.BraceJD(idFlrMembers[j], ref j1, ref j2);
									member = AddMember(idFlrMembers[j], j1, j2, true, type);
									//Record result (force)
									_result.SetResultForBrace(iFlr, member, _load);
									break;
							}


						}
					}
				}
			}
		}

		private FeaMember AddMember(int memberId, int j1, int j2, bool addNodeSelected, MemberType memberType)
		{
			var _hi_CToSDesign = new Hi_CToSDesign();

			float x1 = 0;
			float y1 = 0;
			float z1 = 0;
			(x1, y1, z1) = GetNodeDetail(j1);

			float x2 = 0;
			float y2 = 0;
			float z2 = 0;
			(x2, y2, z2) = GetNodeDetail(j2);

			//Get cross section and material id
			int yjkCrossSectionId = 0;
			int matType = 0;
			float matGrade = 0;
			float matGrade2 = 0;
			float matGrade3 = 0;
			switch (memberType)
			{
				case MemberType.Column:
					yjkCrossSectionId = _hi_CToSDesign.ColumnOLX(memberId);
					_hi_CToSDesign.ColumnMat(memberId, ref matType, ref matGrade, ref matGrade2, ref matGrade3);
					break;
				case MemberType.Beam:
					yjkCrossSectionId = _hi_CToSDesign.BeamOLX(memberId);
					_hi_CToSDesign.BeamMat(memberId, ref matType, ref matGrade, ref matGrade2);
					break;
				case MemberType.Brace:
					yjkCrossSectionId = _hi_CToSDesign.BraceOLX(memberId);
					_hi_CToSDesign.BraceMat(memberId, ref matType, ref matGrade, ref matGrade2, ref matGrade3);
					break;
			}

			int crossSectionId = _crossSection.GetCrossSectionId(memberId, memberType, yjkCrossSectionId, matType, matGrade, 
				matGrade2, matGrade3, _materialApi);

			FeaMember member = new FeaMember(memberId, new FeaNode(j1, x1, y1, z1), new FeaNode(j2, x2, y2, z2), crossSectionId, memberType);
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
				memberY = UnitVector3D.XAxis.Negate();
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