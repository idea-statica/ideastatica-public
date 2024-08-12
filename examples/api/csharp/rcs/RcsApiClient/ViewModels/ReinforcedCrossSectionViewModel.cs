using CommunityToolkit.Mvvm.ComponentModel;
using IdeaStatiCa.Api.RCS.Model;

namespace RcsApiClient.ViewModels
{
	public class ReinforcedCrossSectionViewModel : ObservableObject
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
