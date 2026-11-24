using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry2D;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Material;
using IdeaRS.OpenModel.Model;
using System;
using System.Linq;

namespace IOM.SteelFrameModel
{
	/// <summary>
	/// Short ways to add the objects both frames are made of.
	/// </summary>
	internal static class IomHelpers
	{
		/// <summary>
		/// Adds the object to the model. <see cref="OpenModel.AddObject(OpenElementId)"/> refuses an object with an Id
		/// the model already has, and an object of a type it has no list for, without an exception. This throws instead.
		/// </summary>
		public static void Add(OpenModel model, OpenElementId obj)
		{
			int result = model.AddObject(obj);
			if (result != 0)
			{
				throw new InvalidOperationException($"The model did not add the {obj.GetType().Name} with Id {obj.Id}: AddObject returned {result}.");
			}
		}

		/// <summary>
		/// Adds a node. Coordinates are in metres.
		/// </summary>
		public static void AddNode(OpenModel model, int id, string name, double x, double y, double z)
		{
			Add(model, new Point3D { Id = id, Name = name, X = x, Y = y, Z = z });
		}

		/// <summary>
		/// A rolled I cross-section, which IDEA StatiCa finds in its library by the name.
		/// </summary>
		public static CrossSectionParameter CreateRolledI(int id, string name, MatSteel material)
		{
			var crossSection = new CrossSectionParameter
			{
				Id = id,
				Name = name,
				CrossSectionRotation = 0,
				CrossSectionType = CrossSectionType.RolledI,
				Material = new ReferenceElement(material),
			};
			crossSection.Parameters.Add(new ParameterString { Name = "UniqueName", Value = name });
			return crossSection;
		}

		/// <summary>
		/// Adds a member that runs through the given nodes, in this order. Between each two nodes it has a line segment
		/// and an element on it. All its segments make one polyline.
		/// </summary>
		public static Member1D AddMember(OpenModel model, int id, Member1DType type, CrossSection crossSection, params string[] nodes)
		{
			var polyline = new PolyLine3D();
			var member = new Member1D
			{
				Id = id,
				Name = "M" + id,
				Member1DType = type,
				CrossSection = new ReferenceElement(crossSection),
			};

			for (int i = 1; i < nodes.Length; i++)
			{
				var segment = new LineSegment3D
				{
					StartPoint = new ReferenceElement(model.Point3D.First(p => p.Name == nodes[i - 1])),
					EndPoint = new ReferenceElement(model.Point3D.First(p => p.Name == nodes[i])),
				};
				segment.Id = model.GetMaxId(segment) + 1;
				if (type == Member1DType.Column)
				{
					// The local z axis of a column lies in the plane of its axis and a point far along global X.
					segment.LocalCoordinateSystem = new CoordSystemByPoint
					{
						Point = new Point3D { X = 100000, Y = 0, Z = 0 },
						InPlane = Plane.ZX,
					};
				}
				Add(model, segment);
				polyline.Segments.Add(new ReferenceElement(segment));

				var element = new Element1D { Segment = new ReferenceElement(segment) };
				element.Id = model.GetMaxId(element) + 1;
				element.Name = "E" + element.Id;
				Add(model, element);
				member.Elements1D.Add(new ReferenceElement(element));
			}

			polyline.Id = model.GetMaxId(polyline) + 1;
			Add(model, polyline);
			Add(model, member);
			return member;
		}

		/// <summary>
		/// The outline of a plate through the given corners, in the plane of the plate: x1, y1, x2, y2 and so on, in
		/// metres. The outline goes back from the last corner to the first.
		/// </summary>
		public static Region2D CreateOutline(params double[] corners)
		{
			var outline = new PolyLine2D { StartPoint = new Point2D { X = corners[0], Y = corners[1] } };
			for (int i = 2; i < corners.Length; i += 2)
			{
				outline.Segments.Add(new LineSegment2D { EndPoint = new Point2D { X = corners[i], Y = corners[i + 1] } });
			}
			outline.Segments.Add(new LineSegment2D { EndPoint = new Point2D { X = corners[0], Y = corners[1] } });
			return new Region2D { Outline = outline };
		}

		/// <summary>
		/// Adds a connection point at the node, named after it, joining the given members.
		/// </summary>
		public static ConnectionPoint AddConnectionPoint(OpenModel model, string node, params ConnectedMember[] members)
		{
			var connectionPoint = new ConnectionPoint
			{
				Name = node,
				Node = new ReferenceElement(model.Point3D.First(p => p.Name == node)),
			};
			connectionPoint.Id = model.GetMaxId(connectionPoint) + 1;
			connectionPoint.ConnectedMembers.AddRange(members);
			Add(model, connectionPoint);
			return connectionPoint;
		}

		/// <summary>
		/// A member that ends in the connection.
		/// </summary>
		public static ConnectedMember Ended(Member1D member)
		{
			return new ConnectedMember { Id = member.Id, MemberId = new ReferenceElement(member), IsContinuous = false };
		}

		/// <summary>
		/// A member that runs through the connection.
		/// </summary>
		public static ConnectedMember Continuous(Member1D member)
		{
			return new ConnectedMember { Id = member.Id, MemberId = new ReferenceElement(member), IsContinuous = true };
		}

		/// <summary>
		/// The member as a beam of a connection, with the same id and name.
		/// </summary>
		public static BeamData CreateBeamData(Member1D member)
		{
			return new BeamData
			{
				Id = member.Id,
				Name = member.Name,
				OriginalModelId = member.Id.ToString(),
				IsAdded = false,
				MirrorY = false,
				RefLineInCenterOfGravity = false,
			};
		}
	}
}
