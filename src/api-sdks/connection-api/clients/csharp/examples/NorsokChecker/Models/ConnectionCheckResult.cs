using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NorsokChecker.Models
{
	public class ConnectionCheckResult : INotifyPropertyChanged
	{
		private int _id;
		private string _name = string.Empty;
		private string _status = "Pending";
		private double _maxUtilization;
		private string _norsokPass = "-";
		private int _activeLoadEffects = -1;
		private int _totalLoadEffects = -1;
		private bool _selected = true;

		public int Id
		{
			get => _id;
			set { _id = value; OnPropertyChanged(); }
		}

		public string Name
		{
			get => _name;
			set { _name = value; OnPropertyChanged(); }
		}

		public string Status
		{
			get => _status;
			set { _status = value; OnPropertyChanged(); }
		}

		public double MaxUtilization
		{
			get => _maxUtilization;
			set { _maxUtilization = value; OnPropertyChanged(); OnPropertyChanged(nameof(MaxUtilizationDisplay)); }
		}

		/// <summary>
		/// Display string: "72.4%" for 0.724, capped at 999.9%.
		/// "N/A" shows an em dash, NOT "0.0%": nothing was assessed, and a zero utilisation
		/// reads as an excellent result rather than as an absent one - the same trap as the
		/// old "Norsok OK" on an unassessed joint.
		/// </summary>
		public string MaxUtilizationDisplay =>
			NorsokPass == "N/A" ? "—"
			: MaxUtilization > 9.999 ? ">999%" : $"{MaxUtilization * 100:F1}%";

		public string NorsokPass
		{
			get => _norsokPass;
			// MaxUtilizationDisplay reads this too, and the two are set in either order,
			// so it has to be re-raised here as well or the cell keeps the stale text.
			set { _norsokPass = value; OnPropertyChanged(); OnPropertyChanged(nameof(MaxUtilizationDisplay)); }
		}

		/// <summary>
		/// Whether this connection is assessed by the next run. On by default — opening a project
		/// and pressing Run means "check this project".
		///
		/// This is the USER's selection, unrelated to the grid's row selection (which only decides
		/// which connection the members table and the 3D view show).
		/// </summary>
		public bool Selected
		{
			get => _selected;
			set { _selected = value; OnPropertyChanged(); }
		}

		/// <summary>
		/// Load effects switched ON in the model, and how many the connection has in total.
		/// Both are -1 until the counts have actually been read, which is what
		/// <see cref="LoadEffectsDisplay"/> shows as an em dash: "0 / 0" would be a claim
		/// about the model, and an unread count is not the same as an empty one.
		/// </summary>
		public int ActiveLoadEffects
		{
			get => _activeLoadEffects;
			set { _activeLoadEffects = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoadEffectsDisplay)); }
		}

		public int TotalLoadEffects
		{
			get => _totalLoadEffects;
			set { _totalLoadEffects = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoadEffectsDisplay)); }
		}

		public string LoadEffectsDisplay =>
			_totalLoadEffects < 0 || _activeLoadEffects < 0 ? "—" : $"{_activeLoadEffects} / {_totalLoadEffects}";

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
