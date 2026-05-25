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
using IdeaStatiCa.Plugin;
using yjk.ViewModels;

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

		void GetAll(IFeaLoadsApi load, IFeaResultsApi result,
			IFeaCrossSectionApi crossSection, IFeaMaterialApi materialApi);

		void ReadFromModelDB(APIData.Hi_DbModelData model);
	}

	internal class FeaGeometryApi : IFeaGeometryApi
	{
		/*		private List<IFeaMember> _members = InitializeMembers();
				private List<IFeaNode> _nodes = InitializeNodes();*/

		private List<IFeaMember> _members;
		private List<IFeaNode> _nodesSelected;
		private List<IFeaNode> _nodes;
		private IFeaResultsApi _resultsApi;
		private IFeaLoadsApi _loadsApi;
		private IFeaCrossSectionApi _crossSectionApi;
		private IFeaMaterialApi _materialApi;
		private APIData.Hi_DbModelData _model;
		private IPluginLogger _logger = AppLogger.Instance;

		public void ReadFromModelDB(APIData.Hi_DbModelData model) { _model = model; }

		public IFeaMember GetMember(int id) => _members.FirstOrDefault(m => m.Id == id);

		public (UnitVector3D X, UnitVector3D Y, UnitVector3D Z) GetMemberLcs(int id)
		{
			_logger.LogInformation($"FeaGeometryApi.GetMemberLcs: id={id}");
			var member = GetMember(id);
			IFeaNode beg = GetNode(member.BeginNodeId);
			IFeaNode end = GetNode(member.EndNodeId);
			return CalculateMemberLcs(beg, end);
		}

		public Dictionary<int, List<int>> GetSelectedIds()
		{
			_logger.LogInformation("FeaGeometryApi.GetSelectedIds");
			Hi_AddToAndReadYjk hi_AddToAndReadYjk = new Hi_AddToAndReadYjk();
			Dictionary<int, List<int>> selectedIds = hi_AddToAndReadYjk.GetSelectSetIDs();
			_logger.LogInformation($"FeaGeometryApi.GetSelectedIds: {selectedIds.Count} key(s) returned");
			return selectedIds;
		}

		public void GetSelected(Dictionary<int, List<int>> selectedIds, IFeaLoadsApi loadsApi, IFeaResultsApi resultsApi,
			IFeaCrossSectionApi crossSectionApi, IFeaMaterialApi materialApi)
		{
			_logger.LogInformation("FeaGeometryApi.GetSelected");

			_members = new List<IFeaMember>();
			_nodesSelected = new List<IFeaNode>();
			_nodes = new List<IFeaNode>();
			_resultsApi = resultsApi;
			_loadsApi = loadsApi;
			_crossSectionApi = crossSectionApi;
			_materialApi = materialApi;

			var hiDesign = new Hi_CToSDesign();
			var hiReader = new Hi_AddToAndReadYjk();
			GetSelectedMembers(hiDesign, hiReader, selectedIds, MemberType.Column);
			GetSelectedMembers(hiDesign, hiReader, selectedIds, MemberType.Beam);
			GetSelectedMembers(hiDesign, hiReader, selectedIds, MemberType.Brace);

			//For each nodes, see what members are connected to it
			//Fixed the list of nodes to look at
			List<IFeaNode> nodesCopy = new List<IFeaNode>(_nodes);

			GetConnectedMembers(hiDesign, hiReader, nodesCopy, MemberType.Column);
			GetConnectedMembers(hiDesign, hiReader, nodesCopy, MemberType.Beam);
			GetConnectedMembers(hiDesign, hiReader, nodesCopy, MemberType.Brace);
			_logger.LogInformation($"FeaGeometryApi.GetSelected done: {_members.Count} members, {_nodes.Count} nodes");
		}

		public void GetAll(IFeaLoadsApi loadsApi, IFeaResultsApi resultsApi,
			IFeaCrossSectionApi crossSectionApi, IFeaMaterialApi materialApi)
		{
			_logger.LogInformation("FeaGeometryApi.GetAll");

			_members = new List<IFeaMember>();
			_nodesSelected = new List<IFeaNode>();
			_nodes = new List<IFeaNode>();
			_resultsApi = resultsApi;
			_loadsApi = loadsApi;
			_crossSectionApi = crossSectionApi;
			_materialApi = materialApi;

			var hiDesign = new Hi_CToSDesign();
			var hiReader = new Hi_AddToAndReadYjk();
			GetMembers(hiDesign, hiReader, MemberType.Column);
			GetMembers(hiDesign, hiReader, MemberType.Beam);
			GetMembers(hiDesign, hiReader, MemberType.Brace);
			_logger.LogInformation($"FeaGeometryApi.GetAll done: {_members.Count} members, {_nodes.Count} nodes");
		}

		private void GetConnectedMembers(Hi_CToSDesign hiDesign, Hi_AddToAndReadYjk hiReader, List<IFeaNode> nodesCopy, MemberType memberType)
		{
			int membersBefore = _members.Count;
			_logger.LogInformation($"FeaGeometryApi.GetConnectedMembers: memberType={memberType}, nodeCount={nodesCopy.Count}");

			int numFloor = hiDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;

				(int nMember, List<int> idFlrMembers) = GetFloorMembers(hiDesign, iFlr, memberType);

				for (int j = 0; j < nMember; j++)
				{
					(int j1, int j2) = GetMemberNodeIds(hiDesign, idFlrMembers[j], memberType);

					foreach (IFeaNode node in nodesCopy)
					{
						if (node.Id == j1 || node.Id == j2)
						{
							bool exists = _members.Any(p => p.Id == idFlrMembers[j]);

							if (!exists)
							{
								int modellingId = GetModellingId(hiDesign, hiReader, idFlrMembers[j], memberType);
								double rotationAngle = GetRotationAngle(modellingId, memberType);

								FeaMember member = AddMember(hiDesign, idFlrMembers[j], j1, j2, false, memberType, rotationAngle);

								//Record result (force)
								_resultsApi.SetResult(iFlr, member, _loadsApi, memberType, _crossSectionApi);
							}
						}
					}
				}
			}
			_logger.LogInformation($"FeaGeometryApi.GetConnectedMembers done: {_members.Count - membersBefore} members added");
		}

		private void GetSelectedMembers(Hi_CToSDesign hiDesign, Hi_AddToAndReadYjk hiReader, Dictionary<int, List<int>> selectedIds, MemberType memberType)
		{
			_logger.LogInformation($"FeaGeometryApi.GetSelectedMembers: memberType={memberType}");

			int keyToCheck = GetSelectionKey(memberType);
			int numFloor = hiDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;

				(int nMember, List<int> idFlrMembers) = GetFloorMembers(hiDesign, iFlr, memberType);

				//Check if column is selected
				if (selectedIds.ContainsKey(keyToCheck))
				{
					for (int j = 0; j < nMember; j++)
					{
						int modellingId = GetModellingId(hiDesign, hiReader, idFlrMembers[j], memberType);

						if (selectedIds[keyToCheck].Contains(modellingId))
						{
							double rotationAngle = GetRotationAngle(modellingId, memberType);

							(int j1, int j2) = GetMemberNodeIds(hiDesign, idFlrMembers[j], memberType);
							FeaMember member = AddMember(hiDesign, idFlrMembers[j], j1, j2, true, memberType, rotationAngle);

							//Record result (force)
							_resultsApi.SetResult(iFlr, member, _loadsApi, memberType, _crossSectionApi);
						}
					}
				}
			}
		}

		private void GetMembers(Hi_CToSDesign hiDesign, Hi_AddToAndReadYjk hiReader, MemberType memberType)
		{
			_logger.LogInformation($"FeaGeometryApi.GetMembers: memberType={memberType}");

			int numFloor = hiDesign.NZRC();
			for (int i = 1; i < numFloor + 1; i++)
			{
				int iFlr = i;

				(int nMember, List<int> idFlrMembers) = GetFloorMembers(hiDesign, iFlr, memberType);

				for (int j = 0; j < nMember; j++)
				{
					int modellingId = GetModellingId(hiDesign, hiReader, idFlrMembers[j], memberType);
					double rotationAngle = GetRotationAngle(modellingId, memberType);

					(int j1, int j2) = GetMemberNodeIds(hiDesign, idFlrMembers[j], memberType);
					FeaMember member = AddMember(hiDesign, idFlrMembers[j], j1, j2, true, memberType, rotationAngle);

					//Record result (force)
					_resultsApi.SetResult(iFlr, member, _loadsApi, memberType, _crossSectionApi);
				}
			}
		}

		private static (int nMember, List<int> idFlrMembers) GetFloorMembers(Hi_CToSDesign hi, int iFlr, MemberType memberType)
		{
			int n;
			switch (memberType)
			{
				case MemberType.Column:
					n = hi.NColumn(iFlr);
					return n > 0 ? (n, hi.FlrColumns(iFlr, n)) : (0, new List<int>());
				case MemberType.Beam:
					n = hi.NBeam(iFlr);
					return n > 0 ? (n, hi.FlrBeams(iFlr, n)) : (0, new List<int>());
				case MemberType.Brace:
					n = hi.NBrace(iFlr);
					return n > 0 ? (n, hi.FlrBraces(iFlr, n)) : (0, new List<int>());
				default:
					return (0, new List<int>());
			}
		}

		private static (int j1, int j2) GetMemberNodeIds(Hi_CToSDesign hi, int memberId, MemberType memberType)
		{
			int j1 = 0, j2 = 0;
			switch (memberType)
			{
				case MemberType.Column: hi.ColumnJD(memberId, ref j1, ref j2); break;
				case MemberType.Beam:   hi.BeamJD(memberId, ref j1, ref j2);   break;
				case MemberType.Brace:  hi.BraceJD(memberId, ref j1, ref j2);  break;
			}
			return (j1, j2);
		}

		private static int GetSelectionKey(MemberType memberType) =>
			memberType == MemberType.Column ? 11 : 12;

		private static int GetModellingId(Hi_CToSDesign hiDesign, Hi_AddToAndReadYjk hiReader, int memberId, MemberType memberType)
		{

			int no = 0;
			int flrNo = 0;
			int modellingId = 0;

			switch (memberType)
			{
				case MemberType.Column:
					no = hiDesign.ColumnONO(memberId);
					flrNo = hiDesign.ColumnOFlr(memberId);
					modellingId = hiReader.ReadIdByNO(GjKind.IDK_COLM, no, flrNo);
					return modellingId;
				case MemberType.Beam:
					no = hiDesign.BeamONO(memberId);
					flrNo = hiDesign.BeamOFlr(memberId);
					modellingId = hiReader.ReadIdByNO(GjKind.IDK_BEAM, no, flrNo);
					return modellingId;
				case MemberType.Brace:
					no = hiDesign.BraceONO(memberId);
					flrNo = hiDesign.BraceOFlr(memberId);
					modellingId = hiReader.ReadIdByNO(GjKind.IDK_CL3D, no, flrNo);
					return modellingId;
			}
			return -1;
		}

		private double GetRotationAngle(int modellingId, MemberType memberType)
		{
			double raw = 0;
			double corrected = -1;

			switch (memberType)
			{
				case MemberType.Column:
					{
						Mdl_ColSeg segment = _model.m_ColSeg.FirstOrDefault(m => m.ID == modellingId);
						raw = segment.Rotation;
						corrected = raw * -1;
						break;
					}
				case MemberType.Beam:
					{
						Mdl_BeamSeg segment = _model.m_BeamSeg.FirstOrDefault(m => m.ID == modellingId);
						raw = segment.Rotation;
						corrected = raw * -1;
						break;
					}
				case MemberType.Brace:
					{
						Mdl_BraceSeg segment = _model.m_BraceSeg.FirstOrDefault(m => m.ID == modellingId);
						raw = segment.Rotation;
						corrected = raw * -1;
						break;
					}
			}

			_logger.LogInformation($"FeaGeometryApi.GetRotationAngle: modellingId={modellingId}, type={memberType}, raw={raw:F4}, corrected={corrected:F4}");
			return corrected;
		}

		private FeaMember AddMember(Hi_CToSDesign hiDesign, int memberId, int j1, int j2, bool addNodeSelected, MemberType memberType, double rotationAngle)
		{

			float x1 = 0;
			float y1 = 0;
			float z1 = 0;
			(x1, y1, z1) = GetNodeDetail(hiDesign, j1);

			float x2 = 0;
			float y2 = 0;
			float z2 = 0;
			(x2, y2, z2) = GetNodeDetail(hiDesign, j2);

			//Get cross section and material id
			int yjkCrossSectionId = 0;
			int matType = 0;
			float matGrade = 0;
			float matGrade2 = 0;
			float matGrade3 = 0;
			switch (memberType)
			{
				case MemberType.Column:
					yjkCrossSectionId = hiDesign.ColumnOLX(memberId);
					hiDesign.ColumnMat(memberId, ref matType, ref matGrade, ref matGrade2, ref matGrade3);
					break;
				case MemberType.Beam:
					yjkCrossSectionId = hiDesign.BeamOLX(memberId);
					hiDesign.BeamMat(memberId, ref matType, ref matGrade, ref matGrade2);
					break;
				case MemberType.Brace:
					yjkCrossSectionId = hiDesign.BraceOLX(memberId);
					hiDesign.BraceMat(memberId, ref matType, ref matGrade, ref matGrade2, ref matGrade3);
					break;
			}

			string crossSectionId = _crossSectionApi.GetCrossSectionId(memberId, memberType, yjkCrossSectionId, matType, matGrade,
				matGrade2, matGrade3, _materialApi, _model);

			FeaMember member = new FeaMember(memberId, new FeaNode(j1, x1, y1, z1), new FeaNode(j2, x2, y2, z2),
				crossSectionId, memberType, rotationAngle);
			_members.Add(member);
			_logger.LogInformation($"Member added: id={memberId}, type={memberType}, nodeStart={j1}, nodeEnd={j2}, crossSection={crossSectionId}");

			AddNode(j1, x1, y1, z1, addNodeSelected);
			AddNode(j2, x2, y2, z2, addNodeSelected);

			return member;
		}

		private static (float x, float y, float z) GetNodeDetail(Hi_CToSDesign hiDesign, int nodeId)
		{
			float x = 0;
			float y = 0;
			float z = 0;
			hiDesign.XYZ(nodeId, ref x, ref y, ref z);

			return (x, y, z);
		}

		private void AddNode(int nodeId, float x, float y, float z, bool addNodeSelected)
		{
			bool exists = _nodes.Any(p => p.Id == nodeId);

			if (!exists)
			{
				FeaNode node = new FeaNode(nodeId, x, y, z);

				_nodes.Add(node);
				_logger.LogInformation($"Node added: id={nodeId}, x={x}, y={y}, z={z}");

				if (addNodeSelected)
				{
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
			UnitVector3D memberZ;

			if (memberX.IsParallelTo(globalZ))
			{
				// column
				memberY = UnitVector3D.XAxis.Negate();
				memberZ = memberX.CrossProduct(memberY).Normalize();
			}
			else
			{
				// beam
				memberY = memberX.CrossProduct(globalZ).Normalize().Negate();
				memberZ = memberX.CrossProduct(memberY).Normalize();
			}

			return (memberX.Normalize(), memberY, memberZ);
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