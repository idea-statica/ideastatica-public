//using RAMDATAACCESSLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using IdeaRS.OpenModel.Geometry3D;

//namespace IdeaStatiCa.RamToIdea
//{
//	//This is a Wrapper Class for the 4 element type objects in Ram
//	public class IMember
//	{
//		public enum MemberType { Beam, Column, HorizontalBrace, VerticalBrace }

//		private readonly IBeam Beam;
//		private readonly IColumn Column;
//		private readonly IHorizBrace HorizontalBrace;
//		private readonly IVerticalBrace VerticalBrace;

//		//TODO How to do public Nodes for Cantilevers
//		private readonly INode StartNode;
//		private readonly INode EndNode;

//		public MemberType Type;

//		public int CrossSectionId;

//		public int StartNodeUID { get { return StartNode.lUniqueID; } }
//		public int EndNodeUID { get { return EndNode.lUniqueID; } }

//		public IMember(IBeam beam, INodes modelNodes)
//		{
//			Beam = beam;
//			Type = MemberType.Beam;
//			CrossSectionId = beam.lSectionID;
			
//			SCoordinate[] coords = getStartEndCoordinates();
//			StartNode = findNode(modelNodes, coords[0]);
//			EndNode = findNode(modelNodes, coords[1]);
//		}

//		public IMember(IColumn column, INodes modelNodes)
//		{
//			Column = column;
//			Type = MemberType.Column;
//			CrossSectionId = column.lSectionID;

//			SCoordinate[] coords = getStartEndCoordinates();
//			StartNode = findNode(modelNodes, coords[0]);
//			EndNode = findNode(modelNodes, coords[1]);
//		}

//		public IMember(IHorizBrace horzbrace, INodes modelNodes)
//		{
//			HorizontalBrace = horzbrace;
//			Type = MemberType.HorizontalBrace;
//			CrossSectionId = horzbrace.lSectionID;

//			SCoordinate[] coords = getStartEndCoordinates();
//			StartNode = findNode(modelNodes, coords[0]);
//			EndNode = findNode(modelNodes, coords[1]);
//		}

//		public IMember(IVerticalBrace verticalBrace, INodes modelNodes)
//		{
//			VerticalBrace = verticalBrace;
//			Type = MemberType.VerticalBrace;
//			CrossSectionId = verticalBrace.lSectionID;

//			SCoordinate[] coords = getStartEndCoordinates();
//			StartNode = findNode(modelNodes, coords[0]);
//			EndNode = findNode(modelNodes, coords[1]);
//		}

//		private INode findNode(INodes modelNodes, SCoordinate coordinate)
//		{
//			return modelNodes.GetClosestNode(coordinate.dXLoc, coordinate.dYLoc, coordinate.dZLoc);
//		}

//		public IColumn GetColumn()
//		{
//			return Column;
//		}

//		public IBeam GetBeam()
//		{
//			return Beam;
//		}

//		public IHorizBrace GetHorizBrace()
//		{
//			return HorizontalBrace;
//		}

//		public IVerticalBrace GetVerticalBrace()
//		{
//			return VerticalBrace;
//		}

//		public CoordSystem GetCoordSystem()
//		{
//			//TODO Need to confirm if there is a better way to retrieve coordinate systems from RAM

//			SCoordinate[] sCoordinates = getStartEndCoordinates();
//			Vector3D xAxis = new Vector3D() { X = sCoordinates[1].dXLoc - sCoordinates[0].dXLoc, Y = sCoordinates[1].dYLoc - sCoordinates[0].dYLoc, Z = sCoordinates[1].dZLoc - sCoordinates[0].dZLoc };

//			if (Type == MemberType.Column)
//			{
//				//TODO Need to Set Coordinate system based on the ration angle of the member.
//				double orientationAngle = Column.dOrientation;
//				//return new CoordSystemByPoint();

//				return new CoordSystemByZup() {};
//			}
//			else
//				return new CoordSystemByZup();
//		}

//		private Vector3D Point2Vector(SCoordinate point)
//		{
//			return new Vector3D
//			{
//				X = point.dXLoc,
//				Y = point.dYLoc,
//				Z = point.dZLoc,
//			};
//		}


//		public int MaterialID
//		{
//			get 
//			{
//				if (Type == MemberType.Beam)
//					return Beam.lMaterialID;
//				else if (Type == MemberType.Column)
//					return Column.lMaterialID;
//				else if (Type == MemberType.HorizontalBrace)
//					return HorizontalBrace.lMaterialID;
//				else if (Type == MemberType.VerticalBrace)
//					return VerticalBrace.lMaterialID;
//				else
//					return -1;
//			}
//		}

//		public int IUID
//		{
//			get
//			{
//				if (Type == MemberType.Beam)
//					return Beam.lUID;
//				else if (Type == MemberType.Column)
//					return Column.lUID;
//				else if (Type == MemberType.HorizontalBrace)
//					return HorizontalBrace.lUID;
//				else if (Type == MemberType.VerticalBrace)
//					return VerticalBrace.lUID;
//				else
//					return -1;
//			}
//		}

//	}


//	public static class IMemberUtils
//	{
//		public static IMember GetMember(IModel model, int UID)
//		{
//			INodes analysisNodes = model.GetFrameAnalysisNodes();

//			IBeam beam = model.GetBeam(UID);
//			if (beam != null)
//				return new IMember(beam, analysisNodes);
			
//			IColumn column = model.GetColumn(UID);
//			if (column != null)
//				return new IMember(column, analysisNodes);
			
//			IHorizBrace horxbrace = model.GetHorizBrace(UID);
//			if (horxbrace != null)
//				return new IMember(horxbrace, analysisNodes);
			
//			IVerticalBrace vertbrace = model.GetVerticalBrace(UID);
//			if (vertbrace != null)
//				return new IMember(vertbrace, analysisNodes);

//			return null;
//		}
//	}
//}
