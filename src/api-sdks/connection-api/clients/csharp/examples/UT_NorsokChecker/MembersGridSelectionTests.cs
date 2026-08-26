using System.Windows.Controls;
using NorsokChecker.Models;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Pins the two DataGrid behaviours the 3D-click-to-row wiring depends on: that selecting a row
	/// by member id works, and that the current member can be read back so leaving a hovered row
	/// falls back to the selection instead of clearing the 3D highlight.
	///
	/// STA is required: DataGrid is a WPF control.
	/// </summary>
	[TestFixture, Apartment(System.Threading.ApartmentState.STA)]
	public class MembersGridSelectionTests
	{
		private static (DataGrid Grid, List<MemberDisplayInfo> Rows) Grid()
		{
			var rows = new List<MemberDisplayInfo>
			{
				new() { Id = 1, Name = "M1", Role = "Brace" },
				new() { Id = 2, Name = "M2", Role = "Chord" },
				new() { Id = 3, Name = "M3", Role = "Brace" },
			};
			// mirrors the members grid: read-only, one whole row at a time
			var grid = new DataGrid
			{
				AutoGenerateColumns = false,
				CanUserAddRows = false,
				IsReadOnly = true,
				SelectionMode = DataGridSelectionMode.Single,
				SelectionUnit = DataGridSelectionUnit.FullRow,
				ItemsSource = rows,
			};
			grid.Columns.Add(new DataGridTextColumn { Binding = new System.Windows.Data.Binding("Name") });
			grid.Columns.Add(new DataGridTextColumn { Binding = new System.Windows.Data.Binding("Role") });
			return (grid, rows);
		}

		/// <summary>What Joint3D_MemberClicked does: find the row by member id and select it.</summary>
		[Test]
		public void SelectingByMemberIdSelectsThatRow()
		{
			var (grid, rows) = Grid();

			var target = rows.First(r => r.Id == 3);
			grid.SelectedItem = target;

			Assert.Multiple(() =>
			{
				Assert.That(grid.SelectedItem, Is.SameAs(target));
				Assert.That(((MemberDisplayInfo)grid.SelectedItem!).Id, Is.EqualTo(3));
			});
		}

		/// <summary>
		/// The reverse direction, which MembersGrid_ClearHighlight relies on: after a selection the
		/// grid can be asked which member is current, so leaving a hovered row falls back to it
		/// instead of clearing the 3D highlight.
		/// </summary>
		[Test]
		public void TheSelectedMemberCanBeReadBackForTheHoverFallback()
		{
			var (grid, rows) = Grid();
			grid.SelectedItem = rows.First(r => r.Id == 2);

			int fallback = grid.SelectedItem is MemberDisplayInfo sel ? sel.Id : -1;

			Assert.That(fallback, Is.EqualTo(2));
		}

		/// <summary>And with nothing selected the fallback is the clear value, not a stale id.</summary>
		[Test]
		public void WithNothingSelectedTheFallbackClears()
		{
			var (grid, _) = Grid();

			int fallback = grid.SelectedItem is MemberDisplayInfo sel ? sel.Id : -1;

			Assert.That(fallback, Is.EqualTo(-1));
		}
	}
}
