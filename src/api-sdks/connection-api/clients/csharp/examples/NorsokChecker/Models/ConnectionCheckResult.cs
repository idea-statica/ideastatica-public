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

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
