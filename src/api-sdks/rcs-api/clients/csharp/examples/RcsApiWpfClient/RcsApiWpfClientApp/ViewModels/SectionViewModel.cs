using IdeaStatiCa.Api.RCS.Model;

namespace RcsApiWpfClientApp.ViewModels
{
	public class SectionViewModel : ViewModelBase
	{
		string? _name;
		int _id;

		public SectionViewModel(RcsSection section)
		{
			Id = section.Id;
			Name = section.Description;
			ReinforcedCssId = section.RCSId;
		}

		private int? rfId;
		public int? ReinforcedCssId
		{
			get => rfId;
			set
			{
				rfId = value;
				OnPropertyChanged(nameof(ReinforcedCssId));
			}
		}

		public int Id
		{
			get { return _id; }
			set { SetProperty(ref _id, value); }
		}

		public string? Name
		{
			get { return _name; }
			set { SetProperty(ref _name, value); }
		}
	}
}
