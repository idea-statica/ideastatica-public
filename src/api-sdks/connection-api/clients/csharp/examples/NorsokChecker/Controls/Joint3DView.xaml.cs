using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using NorsokChecker.Services;

namespace NorsokChecker.Controls
{
	/// <summary>
	/// A 3D view of one connection's members, built from the API's own presentation payload — see
	/// <see cref="JointPresentationReader"/>. Selecting or hovering a member highlights its body.
	///
	/// Plain WPF: Viewport3D and MeshGeometry3D ship with the framework, and the payload already
	/// carries positions, normals and triangle indices, so nothing has to be computed or added as a
	/// dependency.
	/// </summary>
	public partial class Joint3DView : UserControl
	{
		private static readonly Brush MemberBrush = new SolidColorBrush(Color.FromRgb(0x90, 0xB4, 0xD8));
		private static readonly Brush HighlightBrush = new SolidColorBrush(Color.FromRgb(0xF5, 0x7C, 0x00));

		private readonly Dictionary<int, GeometryModel3D> _byMember = new();
		private readonly Dictionary<int, string> _names = new();

		/// <summary>
		/// Each member's colour when it is NOT highlighted — the result colour once
		/// <see cref="ColourByUtilisation"/> has run, absent before that. Held separately from the
		/// models because un-highlighting has to know what to go back to.
		/// </summary>
		private readonly Dictionary<int, Brush> _baseBrush = new();
		private int _highlighted = -1;

		/// <summary>Camera width that frames the model — the zoom is relative to this.</summary>
		private double _fitWidth = 1.5;
		private Point? _dragFrom;

		/// <summary>Where the press landed, to tell a click from the end of a drag.</summary>
		private Point? _pressAt;

		/// <summary>
		/// The orientation a double-click returns to. Zero until <see cref="LookAtPlane"/> sets it,
		/// so on the Check tab "reset" still means the oblique load view, while on the §6.4 tab it
		/// means looking square at the joint plane.
		/// </summary>
		private double _homeZ, _homeTilt;

		/// <summary>
		/// The camera frame a double-click returns to, set by <see cref="LookAtPlane"/>. Null until
		/// then, so on the Check tab "reset" keeps the oblique view declared in XAML.
		/// </summary>
		private (Point3D Position, Vector3D Look, Vector3D Up)? _homeCamera;

		/// <summary>
		/// The camera frame <see cref="LookAtPlane"/> produced, so a test can check the plane really
		/// does end up perpendicular to the line of sight and the chord across the screen.
		/// </summary>
		public (Point3D Position, Vector3D Look, Vector3D Up)? HomeCameraForTest => _homeCamera;

		/// <summary>
		/// Raised with the member id when a body is clicked, and with -1 when the click misses every
		/// body. Lets the host select the matching table row — the reverse of
		/// <see cref="HighlightMember"/>.
		/// </summary>
		public event EventHandler<int>? MemberClicked;

		public Joint3DView()
		{
			InitializeComponent();

			// Drag turns the model, the wheel zooms, a double-click puts both back. Handled on the
			// control rather than the viewport so the gestures work over the labels too.
			MouseLeftButtonDown += (_, e) =>
			{
				_dragFrom = e.GetPosition(this);
				_pressAt = _dragFrom;
				CaptureMouse();
			};
			MouseLeftButtonUp += (_, e) =>
			{
				// A click and a rotate share the left button, so tell them apart by distance: a
				// release within a few pixels of the press was a click, anything further was a drag
				// and must not also select a member.
				if (_pressAt is { } down)
				{
					var up = e.GetPosition(this);
					if (Math.Abs(up.X - down.X) <= 3 && Math.Abs(up.Y - down.Y) <= 3)
						MemberClicked?.Invoke(this, HitTestMember(up));
				}
				_pressAt = null;
				_dragFrom = null;
				ReleaseMouseCapture();
			};
			MouseMove += (_, e) =>
			{
				if (!Interactive) return;
				if (_dragFrom is not { } from || e.LeftButton != MouseButtonState.Pressed) return;
				var now = e.GetPosition(this);
				RotateZ.Angle += (now.X - from.X) * 0.5;
				// clamped so the model cannot be turned upside down, which loses the sense of up
				RotateTilt.Angle = Math.Clamp(RotateTilt.Angle + (now.Y - from.Y) * 0.5, -89.0, 89.0);
				_dragFrom = now;
			};
			MouseWheel += (_, e) =>
			{
				if (!Interactive) return;
				// a narrower camera is a closer look; 1.15 per notch is about 12 notches end to end
				double f = e.Delta > 0 ? 1.0 / 1.15 : 1.15;
				Camera.Width = Math.Clamp(Camera.Width * f, _fitWidth * 0.1, _fitWidth * 6.0);
			};
			MouseDoubleClick += (_, _) => { if (Interactive) ResetView(); };
		}

		/// <summary>
		/// Whether the mouse may turn and zoom the view. True on the Check tab, where the point is to
		/// look around the joint.
		///
		/// FALSE on the §6.4 tab: that view is read as a drawing of the joint plane, and a free-turned
		/// camera destroys what the drawing means — "in-plane" and "out-of-plane" stop matching what
		/// the reader sees, so M_ip and M_op become impossible to check against the picture. The
		/// python reference draws a fixed 2D schematic there for exactly this reason; the equivalent
		/// here is the same presentation geometry with the camera pinned, turned only in 90-degree
		/// steps and flipped through the plane. Selecting a member by clicking stays live either way.
		/// </summary>
		public bool Interactive { get; set; } = true;

		/// <summary>
		/// Turn the view a quarter turn within the joint plane, keeping the plane face-on.
		///
		/// Rotates the UP direction about the line of sight, which is what keeps the plane square to
		/// the camera — turning the model instead would tip the plane out of view (see LookAtPlane
		/// for why two model angles cannot hold an arbitrary plane).
		/// </summary>
		public void TurnInPlane(double degrees)
		{
			var look = Camera.LookDirection;
			if (look.Length < 1e-9) return;
			look.Normalize();
			var rot = new RotateTransform3D(new AxisAngleRotation3D(look, degrees));
			var up = rot.Transform(Camera.UpDirection);
			up.Normalize();
			Camera.UpDirection = up;
			_homeCamera = (Camera.Position, Camera.LookDirection, up);
		}

		/// <summary>
		/// Look at the joint plane from the other side: the normal is reversed, so what was the +ey
		/// face becomes the far one. Up is kept, so the view mirrors rather than turning upside down.
		/// </summary>
		public void FlipNormal()
		{
			Camera.Position = new Point3D(-Camera.Position.X, -Camera.Position.Y, -Camera.Position.Z);
			Camera.LookDirection = -Camera.LookDirection;
			_homeCamera = (Camera.Position, Camera.LookDirection, Camera.UpDirection);
		}

		/// <summary>Back to the framing and orientation the model loaded with.</summary>
		public void ResetView()
		{
			Camera.Width = _fitWidth;
			RotateZ.Angle = _homeZ;
			RotateTilt.Angle = _homeTilt;
			if (_homeCamera is { } home)
			{
				Camera.Position = home.Position;
				Camera.LookDirection = home.Look;
				Camera.UpDirection = home.Up;
			}
		}

		/// <summary>
		/// Look square at the joint plane, with the chord running across the view — the view §6.4 is
		/// read in.
		///
		/// This moves the CAMERA, not the model, and that is not a stylistic choice: the model
		/// transform offers only two angles (RotateZ about global Z, then RotateTilt about global X,
		/// clamped to +-89 deg), and two angles cannot bring an arbitrary normal onto this camera's
		/// oblique line of sight. Measured: for a joint in the global XY plane the best any (z, tilt)
		/// pair achieves is |dot| = 0.84 — it never faces the plane at all. The camera has a full
		/// frame to set, so it lands exactly.
		///
		/// The model rotations are zeroed, so a subsequent drag turns the joint from this view rather
		/// than from wherever it was.
		/// </summary>
		public void LookAtPlane(Vector3D planeNormal, Vector3D chordAxis)
		{
			if (planeNormal.Length < 1e-9) return;
			planeNormal.Normalize();

			// Look ALONG the normal, at the origin (the joint node is at (0,0,0) by construction).
			var look = -planeNormal;

			// Up is chosen so the chord lies across the screen: screen-right should be the chord, so
			// up = look x chord. Falls back to any perpendicular when the chord is parallel to the
			// normal, which would mean a chord perpendicular to its own joint plane — not a real
			// joint, but the view must not produce NaNs for it.
			var up = Vector3D.CrossProduct(look, chordAxis);
			if (up.Length < 1e-6)
			{
				up = Vector3D.CrossProduct(look, new Vector3D(0, 0, 1));
				if (up.Length < 1e-6) up = Vector3D.CrossProduct(look, new Vector3D(1, 0, 0));
			}
			up.Normalize();

			// Orthographic: the position only has to be outside the model, the width does the framing.
			Camera.Position = (Point3D)(planeNormal * Math.Max(1.0, _fitWidth * 2.0));
			Camera.LookDirection = look;
			Camera.UpDirection = up;

			_homeZ = 0;
			_homeTilt = 0;
			RotateZ.Angle = 0;
			RotateTilt.Angle = 0;
			_homeCamera = (Camera.Position, look, up);
		}

		/// <summary>Replace the view's contents with these member bodies.</summary>
		public void Load(IReadOnlyList<MemberMesh> meshes)
		{
			MembersGroup.Children.Clear();
			_byMember.Clear();
			_names.Clear();
			_baseBrush.Clear();
			_highlighted = -1;
			// a new connection has a different joint plane, so a LookAtPlane home set for the previous
			// one would aim at the wrong plane — back to the oblique default until it is set again
			_homeZ = 0;
			_homeTilt = 0;
			_homeCamera = null;

			if (meshes.Count == 0)
			{
				Placeholder.Text = "No drawable members in this connection";
				Placeholder.Visibility = Visibility.Visible;
				HintLabel.Text = "";
				return;
			}

			double extent = 0;
			foreach (var m in meshes)
			{
				var mesh = new MeshGeometry3D();
				// The payload shares one vertex array across all members and indexes into it, so the
				// positions are added whole and only the indices differ. Copying just this member's
				// vertices would mean renumbering every index for no gain.
				for (int i = 0; i + 2 < m.Positions.Length; i += 3)
				{
					mesh.Positions.Add(new Point3D(m.Positions[i], m.Positions[i + 1], m.Positions[i + 2]));
					extent = Math.Max(extent, Math.Abs(m.Positions[i]));
					extent = Math.Max(extent, Math.Abs(m.Positions[i + 1]));
					extent = Math.Max(extent, Math.Abs(m.Positions[i + 2]));
				}
				for (int i = 0; i + 2 < m.Normals.Length; i += 3)
					mesh.Normals.Add(new Vector3D(m.Normals[i], m.Normals[i + 1], m.Normals[i + 2]));
				foreach (int idx in m.Indices)
					mesh.TriangleIndices.Add(idx);

				var model = new GeometryModel3D(mesh, new DiffuseMaterial(MemberBrush))
				{
					// tubes are closed surfaces, but a cut member can be seen into
					BackMaterial = new DiffuseMaterial(MemberBrush),
				};
				MembersGroup.Children.Add(model);
				_byMember[m.MemberId] = model;
				_names[m.MemberId] = m.Name;
			}

			// The node is at (0,0,0) and lengths are metres, so the model self-centres — only the
			// camera width has to match the joint's size. Kept as the zoom's reference and the
			// double-click reset target.
			_fitWidth = Math.Max(0.2, extent * 2.4);
			ResetView();
			Placeholder.Visibility = Visibility.Collapsed;
			HintLabel.Text = $"{meshes.Count} members";
		}

		/// <summary>
		/// Which member's body is under this point, or -1 for none. The hit test runs against the
		/// Viewport3D, so it accounts for the current rotation and zoom without any maths here.
		/// </summary>
		private int HitTestMember(Point p)
		{
			// The point comes from this control; the viewport may sit inside padding, so re-base it.
			Point inViewport = TranslatePoint(p, Viewport);
			if (inViewport.X < 0 || inViewport.Y < 0
				|| inViewport.X > Viewport.ActualWidth || inViewport.Y > Viewport.ActualHeight)
				return -1;

			int found = -1;
			VisualTreeHelper.HitTest(Viewport, null,
				result =>
				{
					if (result is RayMeshGeometry3DHitTestResult r)
					{
						foreach (var kv in _byMember)
						{
							if (ReferenceEquals(kv.Value, r.ModelHit))
							{
								found = kv.Key;
								return HitTestResultBehavior.Stop;
							}
						}
					}
					// not one of ours (a light, or a model we do not track) — keep looking behind it
					return HitTestResultBehavior.Continue;
				},
				new PointHitTestParameters(inViewport));

			return found;
		}

		/// <summary>
		/// Colour each member by what its check said, as the python schematic does: green through
		/// amber to red by utilisation, grey for a member nothing was checked on, and a fixed slate
		/// for the chord. Members not named here keep the default.
		///
		/// The colours become each body's BASE colour, so hovering a table row still highlights and
		/// un-highlighting returns to the result colour rather than to plain blue.
		/// </summary>
		public void ColourByUtilisation(IReadOnlyDictionary<int, double?> utilByMember, int chordMemberId = -1)
		{
			foreach (var (id, model) in _byMember)
			{
				Brush brush;
				if (id == chordMemberId) brush = ChordBrush;
				else if (!utilByMember.TryGetValue(id, out var u) || u == null) brush = NoCheckBrush;
				else brush = UtilisationBrush(u.Value);

				_baseBrush[id] = brush;
				if (id == _highlighted) continue;      // leave the hovered one highlighted
				model.Material = new DiffuseMaterial(brush);
				model.BackMaterial = new DiffuseMaterial(brush);
			}
		}

		/// <summary>Clear the result colours; every body goes back to the default.</summary>
		public void ClearUtilisationColours()
		{
			_baseBrush.Clear();
			foreach (var (id, model) in _byMember)
			{
				if (id == _highlighted) continue;
				model.Material = new DiffuseMaterial(MemberBrush);
				model.BackMaterial = new DiffuseMaterial(MemberBrush);
			}
		}

		private static readonly SolidColorBrush ChordBrush = new(Color.FromRgb(0x78, 0x90, 0x9C));
		private static readonly SolidColorBrush NoCheckBrush = new(Color.FromRgb(0xBD, 0xBD, 0xBD));

		/// <summary>
		/// The utilisation ramp, matching the legend beside the view: green to 0.5, yellow-green to
		/// 0.85, amber below 1.0, red at or above it. Banded rather than continuously interpolated
		/// because the four bands are what the legend shows, and a reader compares a body against the
		/// legend, not against a gradient.
		/// </summary>
		private static Brush UtilisationBrush(double util) =>
			util >= 1.0 ? new SolidColorBrush(Color.FromRgb(0xC6, 0x28, 0x28))
			: util >= 0.85 ? new SolidColorBrush(Color.FromRgb(0xF9, 0xA8, 0x25))
			: util >= 0.5 ? new SolidColorBrush(Color.FromRgb(0xC0, 0xCA, 0x33))
			: new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32));

		/// <summary>Paint one member's body in the highlight colour; -1 clears.</summary>
		public void HighlightMember(int memberId)
		{
			if (memberId == _highlighted) return;

			if (_byMember.TryGetValue(_highlighted, out var previous))
			{
				// back to its RESULT colour when one was set — restoring MemberBrush unconditionally
				// wiped the utilisation colouring off whichever body the mouse last touched
				var back = _baseBrush.GetValueOrDefault(_highlighted) ?? MemberBrush;
				previous.Material = new DiffuseMaterial(back);
				previous.BackMaterial = new DiffuseMaterial(back);
			}

			_highlighted = memberId;
			if (_byMember.TryGetValue(memberId, out var current))
			{
				current.Material = new DiffuseMaterial(HighlightBrush);
				current.BackMaterial = new DiffuseMaterial(HighlightBrush);
				HintLabel.Text = string.IsNullOrEmpty(_names.GetValueOrDefault(memberId))
					? $"member {memberId}"
					: _names[memberId];
			}
			else
			{
				HintLabel.Text = $"{_byMember.Count} members";
			}
		}

		public void Clear()
		{
			MembersGroup.Children.Clear();
			_byMember.Clear();
			_names.Clear();
			_baseBrush.Clear();
			_highlighted = -1;
			// a new connection has a different joint plane, so a LookAtPlane home set for the previous
			// one would aim at the wrong plane — back to the oblique default until it is set again
			_homeZ = 0;
			_homeTilt = 0;
			_homeCamera = null;
			Placeholder.Text = "Select a connection to see its members";
			Placeholder.Visibility = Visibility.Visible;
			HintLabel.Text = "";
		}
	}
}
