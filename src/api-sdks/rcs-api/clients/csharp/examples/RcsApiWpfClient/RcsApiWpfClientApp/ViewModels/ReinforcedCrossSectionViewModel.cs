using IdeaStatiCa.Api.RCS.Model;

namespace RcsApiWpfClientApp.ViewModels
{
	public class ReinforcedCrossSectionViewModel : ViewModelBase
	{
		public ReinforcedCrossSectionViewModel()
		{ }

		public ReinforcedCrossSectionViewModel(RcsReinforcedCrossSection reinfCssModel)
		{
			Id = reinfCssModel.Id;
			Name = reinfCssModel.Name;
		}

		private int id;
		public int Id
		{
			get => id;
			set
			{
				id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		private string name = string.Empty;
		public string Name
		{
			get => name;
			set
			{
				name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

	}
}
