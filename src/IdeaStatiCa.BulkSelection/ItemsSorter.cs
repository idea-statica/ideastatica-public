using CI;
using CI.Geometry3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using WM = System.Windows.Media.Media3D;

namespace IdeaStatiCa.BIM.Common
{
	public abstract class Item
	{
		protected Item(object parent)
		{
			Parent = parent ?? throw new ArgumentNullException(nameof(parent));
		}

		public static IEqualityComparer<Item> CustomComparer { get; set; } = new ParentEqualityComparer();

		public object Parent { get; set; }

		public override bool Equals(object obj)
		{
			return CustomComparer != null ? CustomComparer.Equals(this, obj as Item) : base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return CustomComparer != null ? CustomComparer.GetHashCode(this) : base.GetHashCode();
		}

		private class ParentEqualityComparer : IEqualityComparer<Item>
		{
			public bool Equals(Item x, Item y)
			{
				return x.Parent.Equals(y.Parent);
			}

			public int GetHashCode(Item obj)
			{
				return obj.Parent.GetHashCode();
			}
		}

#if DEBUG
		/// <summary>
		/// Internal unique id, that is for testing purpose.
		/// </summary>
		internal int Iid { get; set; }
#endif
	}

	public class Member : Item
	{
		protected Member(object parent, IMatrix44 lcs) : base(parent)
		{
			LCS = lcs ?? throw new ArgumentNullException(nameof(lcs));
		}

		public Member(object parent, IMatrix44 lcs, IPoint3D begin, IPoint3D end, Rect cssBounds) : base(parent)
		{
			LCS = lcs ?? throw new ArgumentNullException(nameof(lcs));
			Begin = begin;
			End = end;
			CrossSectionBounds = cssBounds;
		}

		public IMatrix44 LCS { get; protected set; }

		public IPoint3D Begin { get; protected set; }

		public IPoint3D End { get; protected set; }

		public Rect CrossSectionBounds { get; protected set; }
	}

	public class Plate : Item
	{
		public Plate(object parent, IMatrix44 lcs, List<IPoint3D> points, double thickness) : base(parent)
		{
			LCS = lcs ?? throw new ArgumentNullException(nameof(lcs));
			Contour = points;
			Thickness = thickness;
		}

		public IMatrix44 LCS { get; protected set; }

		public List<IPoint3D> Contour { get; protected set; }

		public double Thickness { get; protected set; }
	}

	public class Weld : Item
	{
		public Weld(object parent, Item firstWeldedItem, Item secondWeldedItem) : base(parent)
		{
			FirstItem = firstWeldedItem;
			SecondItem = secondWeldedItem;
		}

		public Item FirstItem { get; }
		public Item SecondItem { get; }
	}

	public class FastenerGrid : Item
	{
		public FastenerGrid(object parent, IMatrix44 lcs, List<Point3D> gridPoints) : base(parent)
		{
			LCS = lcs ?? throw new ArgumentNullException(nameof(lcs));
			GridPoints = gridPoints;
		}

		public IMatrix44 LCS { get; protected set; }

		public List<Point3D> GridPoints { get; protected set; }
	}

	public sealed class Joint
	{
		public Joint(IPoint3D location)
		{
			Location = location;
		}

		public IPoint3D Location { get; }
		public IReadOnlyList<Member> Members { get; internal set; }
		public IReadOnlyList<Member> StiffeningMembers { get; internal set; }
		public IReadOnlyList<Plate> Plates { get; internal set; }
		public IReadOnlyList<Weld> Welds { get; internal set; }
		public IReadOnlyList<FastenerGrid> Fasteners { get; internal set; }
	}

	public sealed class SorterResult
	{
		public SorterResult(List<Joint> joints)
		{
			Joints = joints.AsReadOnly();
		}

		public IReadOnlyList<Joint> Joints { get; }
	}

	public sealed class SorterData
	{
		public IEnumerable<Member> Members { get; set; }
		public IEnumerable<Plate> Plates { get; set; }
		public IEnumerable<Weld> Welds { get; set; }
		public IEnumerable<FastenerGrid> Fasteners { get; set; }
	}

	public class SorterSettings
	{
		public bool ChainNodes { get; set; } = true;

		public double EnlargeNodeXout { get; set; } = 1.0;
		public double EnlargeNodeXin { get; set; } = 1.0;
		public double EnlargeNodeY { get; set; } = 1.1;
		public double EnlargeNodeZ { get; set; } = 1.1;

		public double LengthTolerance { get; set; } = 0.005;
	}

	public class ItemsSorter
	{
		private const double InclinationToleranceCos0 = 0.087156; // cos(85°)
		private const double degrees5 = 5 * Math.PI / 180; // 5°

		public SorterResult Sort(SorterData data, SorterSettings settings)
		{
#if DEBUG
			AssignIds(data);
#endif
			// Distinct all items
			data.Members = data.Members?.Distinct(Item.CustomComparer).Cast<Member>();
			data.Plates = data.Plates?.Distinct(Item.CustomComparer).Cast<Plate>();
			data.Fasteners = data.Fasteners?.Distinct(Item.CustomComparer).Cast<FastenerGrid>();
			data.Welds = data.Welds?.Distinct(Item.CustomComparer).Cast<Weld>();

			var orderMembers = data.Members.OrderByDescending(GetBiggestMemberSelector);

			var nodes = orderMembers.SelectMany((b, i) =>
			{
				var css = b.CrossSectionBounds;
				var maxX = Math.Max(css.Width, css.Height);
				var surrb = new CI.Common.BoundingBox3D
				{
					MaxX = maxX * settings.EnlargeNodeXin,
					MaxY = css.Right * settings.EnlargeNodeY,
					MaxZ = css.Bottom * settings.EnlargeNodeZ,
					MinX = -maxX * settings.EnlargeNodeXout,
					MinY = css.Left * settings.EnlargeNodeY,
					MinZ = css.Top * settings.EnlargeNodeZ,
				};
				var surre = new CI.Common.BoundingBox3D
				{
					MaxX = maxX * settings.EnlargeNodeXout,
					MaxY = css.Right * settings.EnlargeNodeY,
					MaxZ = css.Bottom * settings.EnlargeNodeZ,
					MinX = -maxX * settings.EnlargeNodeXin,
					MinY = css.Left * settings.EnlargeNodeY,
					MinZ = css.Top * settings.EnlargeNodeZ,
				};
				return new Node[]
				{
					new Node(i * 2 + 1, b.Begin, surrb, b),
					new Node(i * 2 + 2, b.End, surre, b),
				};
			}).ToArray();

			foreach (var node in nodes)
			{
				//try to find and get together similar (exists in surrounding) nodes and continuous members
				var nodecopy = nodes.ToList();
				while (AddMembers(nodecopy, node, settings)) ;
			}

			var joints = new List<Joint>();
			var excluded = new List<Node>();

			var sourcePlates = data.Plates?.ToList() ?? new List<Plate>();
			foreach (var node in nodes.Where(n => n.ConnectedMembers.Count > 0).OrderByDescending(n => n.ConnectedMembers.Count))
			{
				if (excluded.Contains(node))
				{
					continue;
				}

				excluded.Add(node);
				var members = new List<(Member m, bool isended)> { (node.Master, true) };
				AddConnectedMembers(node, node.ConnectedMembers, settings, excluded, members);

				var plates = new List<Plate>();
				while (AddPlates(sourcePlates, plates, node)) { }

				var fasteners = data.Fasteners?.Where(f => node.Contains(f.LCS.Origin)).ToArray() ?? new FastenerGrid[0];

				var parts = members.Select(m => m.m).OfType<Item>().Concat(plates).ToArray();
				var welds = data.Welds?.Where(w => parts.Contains(w.FirstItem) && parts.Contains(w.SecondItem)).ToArray() ?? new Weld[0];

				var stiffeningMembers = members.Where(m => node.Contains(m.m.Begin) && node.Contains(m.m.End)).ToList();
				members = members.Except(stiffeningMembers).GroupBy(m => m.m).Select(g => (g.Key, g.Any(m => m.isended))).ToList();

				// try to find another stiffening members
				stiffeningMembers.AddRange(data.Members
					.Where(m => node.Contains(m.Begin, settings.LengthTolerance) && node.Contains(m.End, settings.LengthTolerance))
					.Select(m => (m, true)));
				excluded.AddRange(stiffeningMembers.SelectMany(m => nodes.Where(n => n.Location == m.m.Begin || n.Location == m.m.End)));

				if (members.Count > 0)
				{
					var bearing = SelectBearingMember(members, node);
					if (bearing != default)
					{
						members.Remove(bearing);
						members.Insert(0, bearing);
					}

					var nodeLocation = node.Location;
					//check if node location is on reference line of bearing member
					if (!bearing.m.IsPointOn(node.Location))
					{
						var distanceFromOrigin = double.PositiveInfinity;
						var canditades = nodes.Except(new Node[] { node }).Where(n => node.Contains(n.Location)).OrderByDescending(n => GetBiggestMemberSelector(n.Master));

						foreach (var cn in canditades)
						{
							if (bearing.m.IsPointOn(cn.Location))
							{
								var newDistatance = GeomOperation.Distance(cn.Location, node.Location);
								//If exist more candidates take it the closest one
								if (newDistatance < distanceFromOrigin)
								{
									distanceFromOrigin = newDistatance;
									nodeLocation = cn.Location;
								}
							}
						}
					}


					var joint = new Joint(nodeLocation)
					{
						Members = members.Select(m => m.m).Distinct().ToArray(),
						StiffeningMembers = stiffeningMembers.Select(m => m.m).Distinct().ToArray(),
						Plates = plates,
						Fasteners = fasteners,
						Welds = welds,
					};

					joints.Add(joint);
				}
			}

#if DEBUG
			TestCaseHelper.CreateTestCaseData(data, new SorterResult(joints));
#endif

			return new SorterResult(joints);
		}

		public static (Member m, bool isended) SelectBearingMember(IEnumerable<(Member m, bool isended)> members, Node node)
		{
			(Member m, bool isended) member;

			// Continuous member exists?
			var allCont = members.Where(m => !m.isended);
			if (allCont.Any())
			{
				// Continuous member exists: true (select bearing from continuous)
				var allEnded = members.Except(allCont);

				// Exactly one continuous member exists?
				if (!allCont.Skip(1).Any())
				{
					// Exactly one continuous member exists: true
					var cont = allCont.First();

					// Continuous Purlin?
					var largestEnded = GetBiggest(allEnded.Where(m => m.m.CrossSectionBounds.IsLargerThan(cont.m.CrossSectionBounds)));
					var isPurlin = HasRotatedCss(cont.m) && largestEnded != default;
					if (isPurlin)
					{
						member = largestEnded;
					}
					else
					{
						member = cont;
					}
				}
				else
				{
					// Exactly one continuous member exists: false

					// Largest continuous member exists?
					var largestEnded = GetBiggest(allEnded);
					var largestCont = largestEnded != default
						? GetBiggest(allCont.Where(m => m.m.CrossSectionBounds.IsLargerThan(largestEnded.m.CrossSectionBounds)))
						: default;
					if (largestCont != default)
					{
						// Largest continuous member exists: true
						member = largestCont;
					}
					else
					{
						// Largest continuous member exists: false

						// Vertical continuous member exists?
						var largestVertical = GetBiggestVertical(allCont);
						if (largestVertical != default)
						{
							// Vertical continuous member exists: true
							member = largestVertical;
						}
						else
						{
							// Vertical continuous member exists: false

							// Horizontal continuous member exists?
							var largestHorizontal = GetBiggestHorizontal(allCont);
							if (largestHorizontal != default)
							{
								// Horizontal continuous member exists: true
								member = largestHorizontal;
							}
							else
							{
								// Horizontal continuous member exists: false
								member = GetBiggest(allCont);
							}
						}
					}
				}
			}
			else
			{
				// Continuous member exists: false (ended members only)

				// Vertical member exists?
				var allvert = GetAllVerticalMembers(members);
				if (allvert.Any())
				{
					// Vertical member exists: true

					// Upward pointing vertical member exists?
					var membersUnderJoint = allvert.Where(m => (m.m.Begin.Z < node.Location.Z && !node.Contains(m.m.Begin)) || (m.m.End.Z < node.Location.Z && !node.Contains(m.m.End)));
					if (membersUnderJoint.Any())
					{
						// Upward pointing vertical member exists: true (under node)
						member = GetBiggest(membersUnderJoint);
					}
					else
					{
						// Upward pointing vertical member exists: false
						member = GetBiggest(allvert);
					}
				}
				else
				{
					// Vertical member exists: false

					// Horizontal member exists?
					var allhoriz = GetAllHorizontalMembers(members);
					if (allhoriz.Any())
					{
						// Horizontal member exists: true

						// Ended Purlin?
						var biggest = GetBiggest(members);
						var purlin = GetBiggest(allhoriz.Where(p => HasRotatedCss(p.m)));
						var isPurlin = purlin != default && biggest.m.CrossSectionBounds.IsLargerThan(purlin.m.CrossSectionBounds);
						if (isPurlin)
						{
							member = biggest;
						}
						else
						{
							member = GetBiggest(allhoriz);
						}
					}
					else
					{
						// Horizontal member exists: false
						member = GetBiggest(members);
					}
				}
			}

			return member;
		}

		private static (Member m, bool isended) GetBiggest(IEnumerable<(Member m, bool isended)> members)
		{
			return members.MaxByOrDefault(GetBiggestMemberSelector);
		}

		private static IEnumerable<(Member m, bool isended)> GetAllHorizontalMembers(IEnumerable<(Member m, bool isended)> members)
		{
			const double HorizontalTolerance = InclinationToleranceCos0;
			return members.Where(m => m.m.IsHorizontal(HorizontalTolerance));
		}

		private static IEnumerable<(Member m, bool isended)> GetAllVerticalMembers(IEnumerable<(Member m, bool isended)> members)
		{
			const double VerticalTolerance = InclinationToleranceCos0;
			return members.Where(m => m.m.IsVertical(VerticalTolerance));
		}

		private static (Member m, bool isended) GetBiggestHorizontal(IEnumerable<(Member m, bool isended)> members)
		{
			return GetAllHorizontalMembers(members).MaxByOrDefault(GetBiggestMemberSelector);
		}

		private static (Member m, bool isended) GetBiggestVertical(IEnumerable<(Member m, bool isended)> members)
		{
			return GetAllVerticalMembers(members).MaxByOrDefault(GetBiggestMemberSelector);
		}

		private static double GetBiggestMemberSelector((Member m, bool isended) member)
		{
			return GetBiggestMemberSelector(member.m);
		}

		private static double GetBiggestMemberSelector(Member member)
		{
			var cssBounds = member.CrossSectionBounds;
			return Math.Max(cssBounds.Width, cssBounds.Height);
		}

		private static bool HasRotatedCss(Member m)
		{
			if (m.IsHorizontal())
			{
				var d = m.LCS.AxisZ.Normalize | new Vector3D(0, 0, 1);
				return Check5Degrees(d);

			}
			else if (m.IsVertical())
			{
				return false;
			}
			else
			{
				var axisXprojection = new Vector3D(m.LCS.AxisX) { DirectionZ = 0 };
				var verticalPlane = (m.LCS.AxisX * axisXprojection).Normalize;
				var d = verticalPlane | m.LCS.AxisZ;
				return Check5Degrees(d);
			}

			bool Check5Degrees(double d)
			{
				var dx = Math.Abs(d);
				var alfaRad = Math.Acos(dx); // mohu si dovolit protoze mam oba vektory vel. 1
				var alfaDeg = alfaRad * 180 / Math.PI; // jen kontrola
				var perpendicular = (Math.PI * 0.5) - alfaRad;

				if (alfaRad.IsLesserOrEqual(degrees5) || perpendicular.IsLesserOrEqual(degrees5))
				{
					return false;
				}
				return true;
			}
		}

		private static void AddConnectedMembers(Node node, List<ConnectedMember> membersToAdd, SorterSettings settings, List<Node> excluded, List<(Member, bool)> members)
		{
			foreach (var cm in membersToAdd)
			{
				if (excluded.Contains(cm.Node))
				{
					continue;
				}

				members.Add((cm.Member, cm.IsEnded));

				if (cm.Node != null)
				{
					node.Inflate(cm.Node);
				}
				else
				{
					// this member looks like continuous - create bounding box in the relative position
					var css = cm.Member.CrossSectionBounds;
					var maxX = Math.Max(css.Width, css.Height);
					var surrb = new CI.Common.BoundingBox3D
					{
						MaxX = maxX * settings.EnlargeNodeXin,
						MaxY = css.Right * settings.EnlargeNodeY,
						MaxZ = css.Bottom * settings.EnlargeNodeZ,
						MinX = -maxX * settings.EnlargeNodeXout,
						MinY = css.Left * settings.EnlargeNodeY,
						MinZ = css.Top * settings.EnlargeNodeZ,
					};
					var point = GeomOperation.Add(cm.Member.Begin, GeomOperation.Subtract(cm.Member.End, cm.Member.Begin) * cm.RelativePosition);
					var tempNode = new Node(-1, point, surrb, cm.Member);
					node.Inflate(tempNode);
				}

				if (cm.Node != null)
				{
					excluded.Add(cm.Node);
				}

				if (settings.ChainNodes && cm.Node != null)
				{
					AddConnectedMembers(node, cm.Node.ConnectedMembers, settings, excluded, members);
				}
			}
		}

		private static bool AddMembers(List<Node> source, Node node, SorterSettings settings)
		{
			var found = false;
			for (var i = source.Count - 1; i >= 0; --i)
			{
				var b = source[i];
				if (b.Master == node.Master)
				{
					continue;
				}

				if (node.Contains(b.Location))
				{
					node.ConnectedMembers.Add(new ConnectedMember(b, b.RelativePosition));
					node.Inflate(b);
					found = true;
					source.Remove(b);
					continue;
				}

				if (node.ConnectedMembers.Find(cm => cm.Member == b.Master) == null)
				{
					var relpos = b.Master.GetPositionOnMember(node.Location, settings);
					if (relpos >= 0 && relpos <= 1)
					{
						node.ConnectedMembers.Add(new ConnectedMember(b.Master, relpos));
						// hledat mimobezne pruty
						found = true;
						node.Inflate(new Node(0, b.Master.GetPointOnRelativePosition(relpos), b.Surroundings, b.Master));
						source.Remove(b);
					}
				}
			}

			return found;
		}

		private static bool AddPlates(List<Plate> source, List<Plate> target, Node node)
		{
			// plate thickness multiplicator for tolerance of Contains method...see below
			const double PlateThicknessMult4Tolerance = 2;
			var found = false;
			for (var i = source.Count - 1; i >= 0; --i)
			{
				var plate = source[i];
				var vertices = plate.Contour.Select(p => p.ToMediaPoint()).ToArray();
				var cov = CentreOfVertices(vertices);
				var coe = CentreOfEdges(vertices);
				var cog = GetCentreOfGravity(vertices);
				var tolerance = plate.Thickness * PlateThicknessMult4Tolerance;
				if (node.Contains(cov, tolerance) || node.Contains(coe, tolerance) || node.Contains(cog, tolerance) || vertices.Any(p => node.Contains(p, tolerance)))
				{
					found = true;
					node.Inflate(vertices);
					source.RemoveAt(i);
					target.Add(plate);
				}
			}

			return found;
		}

		private static WM.Point3D CentreOfVertices(IList<WM.Point3D> verticies)
		{
			double sx = 0, sy = 0, sz = 0;
			var n = verticies.Count;
			for (var i = 0; i < n; i++)
			{
				sx += verticies[i].X;
				sy += verticies[i].Y;
				sz += verticies[i].Z;
			}

			return new WM.Point3D
			{
				X = sx / n,
				Y = sy / n,
				Z = sz / n,
			};
		}

		private static WM.Point3D CentreOfEdges(IList<WM.Point3D> verticies)
		{
			double sx = 0, sy = 0, sz = 0, slen = 0;
			var n = verticies.Count;
			var x1 = verticies[n - 1].X;
			var y1 = verticies[n - 1].Y;
			var z1 = verticies[n - 1].Z;

			for (var i = 0; i < n; i++)
			{
				var x2 = verticies[i].X;
				var y2 = verticies[i].Y;
				var z2 = verticies[i].Z;
				var dx = x2 - x1;
				var dy = y2 - y1;
				var dz = z2 - z1;
				var len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
				sx += (x1 + x2) / 2 * len;
				sy += (y1 + y2) / 2 * len;
				sz += (z1 + z2) / 2 * len;
				slen += len;
				x1 = x2;
				y1 = y2;
				z1 = z2;
			}

			return new WM.Point3D
			{
				X = sx / slen,
				Y = sy / slen,
				Z = sz / slen,
			};
		}

		private static WM.Point3D GetCentreOfGravity(IList<WM.Point3D> verticies)
		{
			double sx = 0, sy = 0, sz = 0;
			double a = 0;

			var p1 = verticies[0];
			var p2 = verticies[1];

			var count = verticies.Count;
			for (var i = 2; i < count; i++)
			{
				var p3 = verticies[i];
				var edge1 = p3 - p1;
				var edge2 = p3 - p2;

				var crossProduct = WM.Vector3D.CrossProduct(edge1, edge2); //edge1 * edge2;
				var area = crossProduct.Length / 2;//crossProduct.Magnitude / 2;

				sx += area * (p1.X + p2.X + p3.X) / 3;
				sy += area * (p1.Y + p2.Y + p3.Y) / 3;
				sz += area * (p1.Z + p2.Z + p3.Z) / 3;

				a += area;
				p2 = p3;
			}

			return new WM.Point3D
			{
				X = sx / a,
				Y = sy / a,
				Z = sz / a,
			};
		}

		[DebuggerDisplay("{Id}: {Location}")]
		public sealed class Node
		{
			private const double DefaultTolerance = 1e-10;

			public Node(int id, IPoint3D location, CI.Common.BoundingBox3D surroundings, Member master)
			{
				Id = id;
				Location = location;
				Surroundings = surroundings;
				OriginalSurroundings = new CI.Common.BoundingBox3D(surroundings);
				Master = master;
				LocationInLCS = master.LCS.TransformToLCS(location);
			}

			internal int Id { get; }

			public IPoint3D Location { get; }

			public CI.Common.BoundingBox3D Surroundings { get; }

			public CI.Common.BoundingBox3D OriginalSurroundings { get; }

			public Member Master { get; }

			public List<ConnectedMember> ConnectedMembers { get; } = new List<ConnectedMember>();

			public double RelativePosition => Location == Master.Begin ? 0 : 1;

			internal IPoint3D LocationInLCS { get; }

			public bool Contains(IPoint3D point, double tolerance = DefaultTolerance)
			{
				return Contains(point.ToMediaPoint(), tolerance);
			}

			public bool Contains(WM.Point3D point, double tolerance = DefaultTolerance)
			{
				var pointInLCS = Master.LCS.TransformToLCS(point);
				var pointInSurroundings = GeomOperation.Subtract(pointInLCS, LocationInLCS).ToMediaPoint();
				return Surroundings.IsPointInside(pointInSurroundings, tolerance);
			}

			public void Inflate(Node n)
			{
				var nloc = (WM.Vector3D)n.LocationInLCS.ToMediaPoint();
				var loc = LocationInLCS.ToMediaPoint();

				var npoints = n.OriginalSurroundings.GetPoints();
				for (var i = npoints.Count - 1; i >= 0; --i)
				{
					var np = npoints[i] + nloc;
					np = n.Master.LCS.TransformToGCS(np);

					var p = loc - (WM.Vector3D)Master.LCS.TransformToLCS(np);
					Surroundings.Inflate(ref p);
				}
			}

			public void Inflate(IList<WM.Point3D> points)
			{
				var loc = LocationInLCS.ToMediaPoint();
				for (var i = points.Count - 1; i >= 0; --i)
				{
					var p = (WM.Vector3D)Master.LCS.TransformToLCS(points[i]) - loc;
					Surroundings.Inflate(ref p);
				}
			}
		}

		public sealed class ConnectedMember
		{
			private readonly Member _beam;

			public ConnectedMember(Member beam, double relativePosition)
			{
				_beam = beam;
				RelativePosition = relativePosition;
			}
			public ConnectedMember(Node node, double relativePosition)
			{
				Node = node;
				RelativePosition = relativePosition;
			}

			public Node Node { get; }

			public Member Member => _beam == null && Node != null ? Node.Master : _beam;

			public double RelativePosition { get; }

			public bool IsEnded => RelativePosition == 0 || RelativePosition == 1;
		}

#if DEBUG
		private static void AssignIds(SorterData data)
		{
			int iid = 0;
			if (data.Members != null)
			{
				foreach (var ii in data.Members)
				{
					ii.Iid = ++iid;
				}
			}

			iid = 0;
			if (data.Plates != null)
			{
				foreach (var ii in data.Plates)
				{
					ii.Iid = ++iid;
				}
			}

			iid = 0;
			if (data.Welds != null)
			{
				foreach (var ii in data.Welds)
				{
					ii.Iid = ++iid;
				}
			}

			iid = 0;
			if (data.Fasteners != null)
			{
				foreach (var ii in data.Fasteners)
				{
					ii.Iid = ++iid;
				}
			}
		}

		private static class TestCaseHelper
		{
			internal static string CreateTestCaseData(SorterData data, SorterResult result)
			{
				var sb = new System.Text.StringBuilder();

				sb.AppendLine("private static object[] TestData()");
				sb.AppendLine("{");

				if (data.Members != null)
				{
					var members = data.Members
						.Select(m => $"\tvar m{m.Iid} = new Member({m.Iid}, {CreateLCS(m.LCS)}, {CreatePoint3D(m.Begin)}, {CreatePoint3D(m.End)}, {CreateRect(m.CrossSectionBounds)});");
					sb.AppendLine(string.Join(Environment.NewLine, members));
				}

				if (data.Plates != null)
				{
					var plates = data.Plates
						.Select(p => $"\tvar p{p.Iid} = new Plate({p.Iid}, {CreateLCS(p.LCS)}, {CreateContour(p.Contour)}, {p.Thickness});");
					sb.AppendLine(string.Join(Environment.NewLine, plates));
				}

				var joints = result.Joints
					.Select((j, i) => $"\tvar j{i + 1} = new Joint({CreatePoint3D(j.Location)}) {{ " +
					$"Members = new Member[] {{ {string.Join(",", j.Members.Select(m => $"m{m.Iid}"))} }}, " +
					$"Plates = new Plate[] {{ {string.Join(",", j.Plates.Select(p => $"p{p.Iid}"))} }}, }};");
				sb.AppendLine(string.Join(Environment.NewLine, joints));

				sb.AppendLine("\treturn new object[]");
				sb.AppendLine("\t{");
				sb.AppendLine("\t\tGetCurrentMethod(),");
				sb.AppendLine("\t\tnew SorterData");
				sb.AppendLine("\t\t{");
				if (data.Members != null)
				{
					sb.Append("\t\t\tMembers = new List<Member>() { ").Append(string.Join(",", data.Members.Select(m => $"m{m.Iid}"))).AppendLine(" },");
				}

				if (data.Plates != null)
				{
					sb.Append("\t\t\tPlates = new List<Plate>() { ").Append(string.Join(",", data.Plates.Select(p => $"p{p.Iid}"))).AppendLine(" },");
				}

				sb.AppendLine("\t\t},");
				sb.AppendLine("\t\tnew SorterResult(new List<Joint>()");
				sb.AppendLine("\t\t{");
				sb.Append("\t\t\t").AppendLine(string.Join(",", joints.Select((_, i) => $"j{i + 1}")));
				sb.AppendLine("\t\t}),");
				sb.AppendLine("\t};");

				sb.AppendLine("}");
				var testCaseString = sb.ToString();
				//System.IO.File.WriteAllText("D:\\TestCaseData.txt", testCaseString);
				return testCaseString;
			}

			private static string CreateContour(IList<IPoint3D> contour)
			{
				return $"new List<IPoint3D> {{ {string.Join(", ", contour.Select(CreatePoint3D))} }}";
			}

			private static string CreateLCS(IMatrix44 m)
			{
				return $"CreateLCS({CreatePoint3D(m.Origin)}, {CreateVector3D(m.AxisX)}, {CreateVector3D(m.AxisY)}, {CreateVector3D(m.AxisZ)})";
			}

			private static string CreatePoint3D(IPoint3D p)
			{
				return $"Point3D({p.X}, {p.Y}, {p.Z})";
			}

			private static string CreateRect(Rect r)
			{
				return $"Rect({r.Width}, {r.Height})";
			}

			private static string CreateVector3D(Vector3D v)
			{
				return $"new Vector3D({v.DirectionX}, {v.DirectionY}, {v.DirectionZ})";
			}
		}
#endif
	}

	internal static class MemberExtension
	{
		public static double GetPositionOnMember(this Member member, IPoint3D point, SorterSettings settings)
		{
			var pointInLCS = member.LCS.TransformToLCS(point);
			var point2DInLCS = new Point(pointInLCS.Y, pointInLCS.Z);
			var b = member.CrossSectionBounds;
			b.Scale(settings.EnlargeNodeY, settings.EnlargeNodeZ);
			if (b.Contains(point2DInLCS))
			{
				var beginInLCS = member.LCS.TransformToLCS(member.Begin);
				var endInLCS = member.LCS.TransformToLCS(member.End);
				return (pointInLCS.X - beginInLCS.X) / (endInLCS.X - beginInLCS.X);
			}

			return double.NaN;
		}

		public static bool IsPointOn(this Member member, IPoint3D point)
		{
			var position = GetPositionOnMember(member, point, new SorterSettings() { EnlargeNodeY = 0.001, EnlargeNodeZ = 0.001 });

			return !double.IsNaN(position);
		}

		/// <summary>
		/// Get point on member represent relative position
		/// </summary>
		/// <param name="member"></param>
		/// <param name="relativePosition"></param>
		/// <returns>point</returns>
		public static IPoint3D GetPointOnRelativePosition(this Member member, double relativePosition)
		{
			if (relativePosition <= 0)
			{
				return member.Begin;
			}

			if (relativePosition >= 1)
			{
				return member.End;
			}
			var vect = GeomOperation.Subtract(member.End, member.Begin) * relativePosition;
			return GeomOperation.Add(member.Begin, vect);
		}

		public static bool IsHorizontal(this Member member, double tolerance = 1e-9)
		{
			return member.LCS.AxisX.DirectionZ.IsZero(tolerance);
		}

		public static bool IsVertical(this Member member, double tolerance = 1e-9)
		{
			return member.LCS.AxisX.DirectionX.IsZero(tolerance) && member.LCS.AxisX.DirectionY.IsZero(tolerance);
		}
	}

	internal static class BoundingBox3DExtensions
	{
		public static IList<WM.Point3D> GetPoints(this CI.Common.BoundingBox3D box)
		{
			return new WM.Point3D[]
			{
				new WM.Point3D(box.MinX, box.MinY, box.MinZ),
				new WM.Point3D(box.MaxX, box.MinY, box.MinZ),
				new WM.Point3D(box.MaxX, box.MaxY, box.MinZ),
				new WM.Point3D(box.MinX, box.MaxY, box.MinZ),
				new WM.Point3D(box.MinX, box.MinY, box.MaxZ),
				new WM.Point3D(box.MaxX, box.MinY, box.MaxZ),
				new WM.Point3D(box.MaxX, box.MaxY, box.MaxZ),
				new WM.Point3D(box.MinX, box.MaxY, box.MaxZ),
			};
		}
	}

	internal static class RectExtensions
	{
		public static bool IsLargerThan(this Rect rect1, Rect rect2)
		{
			const double margin = 1.2;
			return rect1.Width >= rect2.Width * margin || rect1.Height >= rect2.Height * margin;
		}
	}
}