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
				RefreshLabels();   // the labels are 2D; a turn leaves them beside their members
			};
			MouseWheel += (_, e) =>
			{
				if (!Interactive) return;
				// a narrower camera is a closer look; 1.15 per notch is about 12 notches end to end
				double f = e.Delta > 0 ? 1.0 / 1.15 : 1.15;
				Camera.Width = Math.Clamp(Camera.Width * f, _fitWidth * 0.1, _fitWidth * 6.0);
				RefreshLabels();   // zooming changes the projection scale
			};
			MouseDoubleClick += (_, _) => { if (Interactive) ResetView(); };

			// The labels are 2D and the fit depends on the aspect ratio, so both have to be redone
			// when the control is resized — dragging the sheet's splitter would otherwise leave the
			// names beside their members and the joint mis-framed.
			SizeChanged += (_, _) =>
			{
				if (_byMember.Count == 0) return;
				if (_homeCamera != null) FitToView();   // a plane view: keep it filling the frame
				else RefreshLabels();
			};
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
		public bool Interactive
		{
			get => _interactive;
			set
			{
				_interactive = value;
				// the hint has to follow, or it promises gestures the reader does not have
				GestureHint.Text = value
					? "drag to rotate · wheel to zoom · double-click to reset"
					: "click a member to select it";
			}
		}
		private bool _interactive = true;

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
			FitToView();
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
			FitToView();
		}

		/// <summary>
		/// Set the camera width so the joint fills the view in its CURRENT orientation.
		///
		/// Load() sizes the camera from the model's largest coordinate — a sphere around the node,
		/// which is safe for any rotation and therefore always too wide for the one actually being
		/// looked at. Seen square onto its own plane a joint is flat, so most of the frame was
		/// empty. This measures the vertices as PROJECTED onto the camera's own right/up axes, so
		/// each quarter turn and each flip re-fits to what is really on screen.
		///
		/// The aspect ratio is accounted for: an orthographic camera's Width covers the horizontal
		/// extent, so a joint that is tall and narrow has to be framed by its height instead.
		/// </summary>
		/// <summary>
		/// Whether member names are drawn over the view. Off by default; the §6.4 sheet turns them
		/// on, as the python schematic labels its members.
		/// </summary>
		public bool ShowMemberLabels
		{
			get => _showLabels;
			set { _showLabels = value; RefreshLabels(); }
		}
		private bool _showLabels;

		/// <summary>
		/// Redraw the name labels at their members' current screen positions.
		///
		/// Called after anything that moves the projection — a turn, a flip, a re-fit, a reload —
		/// because the labels are 2D and know nothing about the camera on their own.
		///
		/// The anchor is the member's FARTHEST vertex from the node, not its centroid: every body
		/// meets at the node, so centroids cluster in the middle of the view and the labels land on
		/// top of each other. The far end is where the member is unambiguously itself, which is also
		/// where the python schematic puts its labels.
		/// </summary>
		private void RefreshLabels()
		{
			LabelLayer.Children.Clear();
			if (!_showLabels || _byMember.Count == 0) return;
			if (ActualWidth <= 0 || ActualHeight <= 0) return;

			var look = Camera.LookDirection;
			var up = Camera.UpDirection;
			if (look.Length < 1e-9 || up.Length < 1e-9) return;
			look.Normalize();
			var right = Vector3D.CrossProduct(look, up);
			if (right.Length < 1e-9) return;
			right.Normalize();
			up = Vector3D.CrossProduct(right, look);
			up.Normalize();

			double halfW = Camera.Width / 2.0;
			double halfH = halfW * ActualHeight / ActualWidth;
			if (halfW <= 0 || halfH <= 0) return;

			// Dragging turns the MODEL, not the camera (see the MouseMove handler), so a projection
			// built from the camera alone would be right only while those angles are zero — true on
			// the §6.4 tab, false on the Check tab the moment the user drags. Fold the model
			// transform into each vertex first.
			Transform3D modelTransform = MembersVisual.Transform ?? Transform3D.Identity;

			foreach (var (id, model) in _byMember)
			{
				string name = _names.GetValueOrDefault(id) ?? "";
				if (string.IsNullOrEmpty(name)) continue;
				if (model.Geometry is not MeshGeometry3D mesh || mesh.Positions.Count == 0) continue;

				// Farthest vertex from the node, measured in the screen plane so the label lands at
				// the visible end rather than at one pointing away from the camera.
				//
				// Only over the vertices this member's TRIANGLES actually use. The payload shares one
				// vertex array across every member (see Load), so mesh.Positions holds the whole
				// joint — scanning it gave all six members the same farthest point, and all six
				// labels stacked on one pixel with only the last one drawn visible. That is the
				// "only M6 is labelled" report: measured on CON8, all six landed at (20.5, 176.2).
				Point3D anchor = modelTransform.Transform(mesh.Positions[0]);
				double best = -1;
				foreach (int idx in mesh.TriangleIndices)
				{
					if (idx < 0 || idx >= mesh.Positions.Count) continue;
					var p = modelTransform.Transform(mesh.Positions[idx]);
					var v = new Vector3D(p.X, p.Y, p.Z);
					double x = Vector3D.DotProduct(v, right), y = Vector3D.DotProduct(v, up);
					double d = x * x + y * y;
					if (d > best) { best = d; anchor = p; }
				}

				var av = new Vector3D(anchor.X, anchor.Y, anchor.Z);
				// pull the label back toward the node by a tenth, so it sits ON the member's end
				// rather than beyond its tip where it can fall outside the frame
				av *= 0.9;
				double sx = (Vector3D.DotProduct(av, right) / halfW + 1) / 2 * ActualWidth;
				double sy = (1 - Vector3D.DotProduct(av, up) / halfH) / 2 * ActualHeight;

				var label = new TextBlock
				{
					Text = name,
					FontSize = 11,
					FontWeight = FontWeights.SemiBold,
					Foreground = new SolidColorBrush(Color.FromRgb(0x37, 0x47, 0x4F)),
					// a pill behind the text, so a name over a dark body stays legible
					Background = new SolidColorBrush(Color.FromArgb(0xD0, 0xFF, 0xFF, 0xFF)),
					Padding = new Thickness(3, 0, 3, 0),
				};
				// centre the pill on the anchor; measured first, since a TextBlock has no size until then
				label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Canvas.SetLeft(label, sx - label.DesiredSize.Width / 2);
				Canvas.SetTop(label, sy - label.DesiredSize.Height / 2);
				LabelLayer.Children.Add(label);
			}
		}

		public void FitToView()
		{
			if (_byMember.Count == 0) return;

			var look = Camera.LookDirection;
			var up = Camera.UpDirection;
			if (look.Length < 1e-9 || up.Length < 1e-9) return;
			look.Normalize();
			var right = Vector3D.CrossProduct(look, up);
			if (right.Length < 1e-9) return;
			right.Normalize();
			up = Vector3D.CrossProduct(right, look);
			up.Normalize();

			double halfW = 0, halfH = 0;
			foreach (var model in _byMember.Values)
			{
				if (model.Geometry is not MeshGeometry3D mesh) continue;
				foreach (var p in mesh.Positions)
				{
					var v = new Vector3D(p.X, p.Y, p.Z);
					halfW = Math.Max(halfW, Math.Abs(Vector3D.DotProduct(v, right)));
					halfH = Math.Max(halfH, Math.Abs(Vector3D.DotProduct(v, up)));
				}
			}
			if (halfW <= 0 && halfH <= 0) return;

			// 1.08 leaves a hair of margin so the outermost body does not touch the border
			double needW = halfW * 2 * 1.08;
			double needH = halfH * 2 * 1.08;
			double aspect = ActualWidth > 0 && ActualHeight > 0 ? ActualWidth / ActualHeight : 1.0;
			Camera.Width = Math.Max(needW, needH * aspect);
			RefreshLabels();
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
			// the whole point of this framing is to look square at the plane, so fill the view with
			// it — the sphere-sized width Load() picked is always too wide for one orientation
			FitToView();
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
			RefreshLabels();
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

		// lighter than their legend swatches for the same reason as the utilisation ramp, but NOT
		// so light that the lit face clips to white: #E0E0E0 came back as #FFFFFF, which on a white
		// panel makes an unchecked member disappear rather than read as grey.
		private static readonly SolidColorBrush ChordBrush = new(Color.FromRgb(0x9E, 0xAF, 0xB8));
		private static readonly SolidColorBrush NoCheckBrush = new(Color.FromRgb(0xC4, 0xC4, 0xC4));

		/// <summary>
		/// The utilisation ramp, delegated to <see cref="Models.UtilisationScale"/> so this view, the
		/// load-effect bar, the result rows and the legend cannot drift apart — they did, as three
		/// copies of a four-band ramp. The scale's LIT tones are the ones used here: a lit face never
		/// returns its own colour, so those are lighter and land near the flat legend swatches once
		/// WPF has multiplied them by (ambient + Σ lights).
		///
		/// Banded, not interpolated, because the legend shows bands and a reader compares a body
		/// against the legend rather than against a gradient.
		/// </summary>
		private static Brush UtilisationBrush(double util) => Models.UtilisationScale.LitBrush(util);

		/// <summary>
		/// The ramp and the no-check grey, exposed so the colour tests can measure what this view
		/// will actually paint rather than a copy of it — a test that restates the ramp cannot fail
		/// when the ramp changes.
		/// </summary>
		internal Brush UtilisationBrushForTest(double util) => UtilisationBrush(util);
		internal Brush NoCheckBrushForTest => NoCheckBrush;

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
			LabelLayer.Children.Clear();   // or the previous joint's names hang over an empty view
		}
	}
}
