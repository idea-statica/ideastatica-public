using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using NorsokChecker.Controls;
using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// The member-name labels over the joint view.
	///
	/// Reported from the running app (2026-08-27): on CON8 only ONE of six labels was visible. So
	/// this measures the projected screen position of every label against the control's bounds,
	/// which is the property "is it visible" actually depends on — eyeballing a screenshot can say
	/// a label is missing but not why.
	///
	/// Two candidate causes, and the tests separate them: labels drawn before Load() has any
	/// geometry, and a projection that ignores the model's own rotation transform (which is
	/// identity on the §6.4 tab but not on the Check tab, where the joint is turned by dragging).
	///
	/// STA: builds WPF controls.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class MemberLabelTests
	{
		[OneTimeSetUp]
		public void EnsureApplication()
		{
			if (System.Windows.Application.Current == null)
			{
				var app = new NorsokChecker.App();
				app.InitializeComponent();
			}
		}

		/// <summary>
		/// A joint of six members radiating from the node, sized like a real one (metres, node at
		/// the origin) so the projection arithmetic runs on realistic numbers.
		///
		/// Crucially it reproduces the payload's real SHAPE: one vertex array shared by every
		/// member, with each member's own triangles indexing into its slice of it. An earlier
		/// version gave each member its own short array, and that fixture could not fail — it
		/// missed the actual defect, where scanning the shared array gave all six members the same
		/// farthest vertex and stacked all six labels on one pixel.
		/// </summary>
		private static List<MemberMesh> SixMembers()
		{
			// a chord along X and five braces at assorted angles
			var dirs = new (string Name, double X, double Y, double Z)[]
			{
				("M2", 1.0, 0.0, 0.0),        // chord
				("M1", 0.5, 0.0, 0.87),
				("M3", -0.77, 0.0, 0.64),
				("M4", 0.57, 0.0, -0.82),
				("M5", -0.5, 0.0, -0.87),
				("M6", 0.0, 0.0, -1.0),
			};

			// ONE array for the whole joint, eight vertices per member laid end to end
			const int perMember = 8;
			var shared = new List<double>();
			foreach (var (_, x, y, z) in dirs)
			{
				for (int k = 0; k < perMember; k++)
				{
					double t = 0.2 + 0.4 * (k / (double)(perMember - 1));
					double off = (k % 2 == 0) ? 0.02 : -0.02;
					shared.Add(x * t + off);
					shared.Add(y * t + off);
					shared.Add(z * t);
				}
			}
			double[] positions = shared.ToArray();

			var meshes = new List<MemberMesh>();
			for (int m = 0; m < dirs.Length; m++)
			{
				// this member's triangles point only at ITS slice of the shared array
				int b = m * perMember;
				meshes.Add(new MemberMesh
				{
					MemberId = m + 1,
					Name = dirs[m].Name,
					Positions = positions,
					Normals = positions,
					Indices = new[] { b, b + 1, b + 2, b + 3, b + 4, b + 5, b + 6, b + 7 },
				});
			}
			return meshes;
		}

		/// <summary>
		/// Give the control a real size, as the layout would. Without this ActualWidth is 0 and the
		/// projection cannot run at all — which is itself one of the candidate causes.
		/// </summary>
		private static Joint3DView Sized(double w = 380, double h = 300)
		{
			var view = new Joint3DView();
			view.Width = w;
			view.Height = h;
			view.Measure(new Size(w, h));
			view.Arrange(new Rect(0, 0, w, h));
			view.UpdateLayout();
			return view;
		}

		private static Canvas LabelLayerOf(Joint3DView view) =>
			(Canvas)view.FindName("LabelLayer")!;

		[Test]
		public void EveryMemberGetsALabel()
		{
			var view = Sized();
			view.ShowMemberLabels = true;
			view.Load(SixMembers());

			var layer = LabelLayerOf(view);
			var names = layer.Children.OfType<TextBlock>().Select(t => t.Text).OrderBy(t => t).ToList();

			Assert.That(names, Is.EqualTo(new[] { "M1", "M2", "M3", "M4", "M5", "M6" }),
				"one label per member, and the reported defect was that five of six were missing");
		}

		/// <summary>
		/// THE test for the report: every label must land INSIDE the control. A label positioned
		/// off-canvas is present in the tree and invisible on screen, which is exactly what "only M6
		/// is visible" looks like.
		/// </summary>
		[Test]
		public void EveryLabelLandsInsideTheView()
		{
			var view = Sized();
			view.ShowMemberLabels = true;
			view.Load(SixMembers());
			view.LookAtPlane(new Vector3D(0, 1, 0), new Vector3D(1, 0, 0));   // the §6.4 framing

			var layer = LabelLayerOf(view);
			var offscreen = new List<string>();
			foreach (var label in layer.Children.OfType<TextBlock>())
			{
				double x = Canvas.GetLeft(label), y = Canvas.GetTop(label);
				label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				// a label is readable only if it is wholly within the control
				if (double.IsNaN(x) || double.IsNaN(y)
					|| x < 0 || y < 0
					|| x + label.DesiredSize.Width > view.Width
					|| y + label.DesiredSize.Height > view.Height)
					offscreen.Add($"{label.Text} at ({x:F0},{y:F0})");
			}

			Assert.That(offscreen, Is.Empty,
				$"these labels fall outside the {view.Width}x{view.Height} view: "
				+ string.Join(", ", offscreen));
		}

		/// <summary>
		/// THE defect, named directly: the labels must not sit on top of each other.
		///
		/// Measured on CON8 in the running app — all six landed at (20.5, 176.2), so five were
		/// hidden under the sixth and the report was "only M6 is labelled". The cause was scanning
		/// the SHARED vertex array for the farthest point, which gives every member the same answer;
		/// the anchor has to come from the vertices this member's own triangles use.
		///
		/// "Inside the view" could not catch this — every one of those six positions was inside.
		/// </summary>
		[Test]
		public void LabelsDoNotStackOnOneSpot()
		{
			var view = Sized();
			view.ShowMemberLabels = true;
			view.Load(SixMembers());
			view.LookAtPlane(new Vector3D(0, 1, 0), new Vector3D(1, 0, 0));

			var spots = LabelLayerOf(view).Children.OfType<TextBlock>()
				.Select(t => (t.Text, X: Canvas.GetLeft(t), Y: Canvas.GetTop(t)))
				.ToList();

			// count distinct positions, rounded to the pixel — two labels within a pixel of each
			// other are one label as far as the reader is concerned
			int distinct = spots.Select(s => (Math.Round(s.X), Math.Round(s.Y))).Distinct().Count();

			Assert.That(distinct, Is.EqualTo(spots.Count),
				"every member's label must have its own position; got "
				+ string.Join(", ", spots.Select(s => $"{s.Text}({s.X:F0},{s.Y:F0})")));
		}

		/// <summary>
		/// Labels off means none drawn — the Check tab used to have no labels at all, so the flag has
		/// to actually gate them rather than being decorative.
		/// </summary>
		[Test]
		public void NoLabelsWhenTheFlagIsOff()
		{
			var view = Sized();
			view.ShowMemberLabels = false;
			view.Load(SixMembers());

			Assert.That(LabelLayerOf(view).Children, Is.Empty);
		}

		/// <summary>
		/// Turning the view must move the labels with it. If the projection ignores the camera or
		/// the model transform, the labels stay where they were and end up beside their members —
		/// the second candidate cause of the report.
		/// </summary>
		[Test]
		public void TurningTheViewMovesTheLabels()
		{
			var view = Sized();
			view.ShowMemberLabels = true;
			view.Load(SixMembers());
			view.LookAtPlane(new Vector3D(0, 1, 0), new Vector3D(1, 0, 0));

			var before = LabelLayerOf(view).Children.OfType<TextBlock>()
				.ToDictionary(t => t.Text, t => (Canvas.GetLeft(t), Canvas.GetTop(t)));

			view.TurnInPlane(90);

			var after = LabelLayerOf(view).Children.OfType<TextBlock>()
				.ToDictionary(t => t.Text, t => (Canvas.GetLeft(t), Canvas.GetTop(t)));

			Assert.That(after.Keys, Is.EquivalentTo(before.Keys), "the same members are labelled");
			int moved = before.Count(kv => after.TryGetValue(kv.Key, out var p)
				&& (Math.Abs(p.Item1 - kv.Value.Item1) > 1 || Math.Abs(p.Item2 - kv.Value.Item2) > 1));
			Assert.That(moved, Is.GreaterThan(0),
				"a quarter turn must reposition the labels; none moved, so the projection is not "
				+ "following the camera");
		}
	}
}
