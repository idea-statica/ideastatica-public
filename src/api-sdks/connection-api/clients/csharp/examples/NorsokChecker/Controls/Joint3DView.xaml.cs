using System.Windows;
using System.Windows.Controls;
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

		public Joint3DView() => InitializeComponent();

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
			// camera width has to match the joint's size.
			Camera.Width = Math.Max(0.2, extent * 2.4);
			Placeholder.Visibility = Visibility.Collapsed;
			HintLabel.Text = $"{meshes.Count} members";
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
