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
		private int _highlighted = -1;

		/// <summary>Camera width that frames the model — the zoom is relative to this.</summary>
		private double _fitWidth = 1.5;
		private Point? _dragFrom;

		/// <summary>Where the press landed, to tell a click from the end of a drag.</summary>
		private Point? _pressAt;

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
				if (_dragFrom is not { } from || e.LeftButton != MouseButtonState.Pressed) return;
				var now = e.GetPosition(this);
				RotateZ.Angle += (now.X - from.X) * 0.5;
				// clamped so the model cannot be turned upside down, which loses the sense of up
				RotateTilt.Angle = Math.Clamp(RotateTilt.Angle + (now.Y - from.Y) * 0.5, -89.0, 89.0);
				_dragFrom = now;
			};
			MouseWheel += (_, e) =>
			{
				// a narrower camera is a closer look; 1.15 per notch is about 12 notches end to end
				double f = e.Delta > 0 ? 1.0 / 1.15 : 1.15;
				Camera.Width = Math.Clamp(Camera.Width * f, _fitWidth * 0.1, _fitWidth * 6.0);
			};
			MouseDoubleClick += (_, _) => ResetView();
		}

		/// <summary>Back to the framing and orientation the model loaded with.</summary>
		public void ResetView()
		{
			Camera.Width = _fitWidth;
			RotateZ.Angle = 0;
			RotateTilt.Angle = 0;
		}

		/// <summary>Replace the view's contents with these member bodies.</summary>
		public void Load(IReadOnlyList<MemberMesh> meshes)
		{
			MembersGroup.Children.Clear();
			_byMember.Clear();
			_names.Clear();
			_highlighted = -1;

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

		/// <summary>Paint one member's body in the highlight colour; -1 clears.</summary>
		public void HighlightMember(int memberId)
		{
			if (memberId == _highlighted) return;

			if (_byMember.TryGetValue(_highlighted, out var previous))
			{
				previous.Material = new DiffuseMaterial(MemberBrush);
				previous.BackMaterial = new DiffuseMaterial(MemberBrush);
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
			_highlighted = -1;
			Placeholder.Text = "Select a connection to see its members";
			Placeholder.Visibility = Visibility.Visible;
			HintLabel.Text = "";
		}
	}
}
