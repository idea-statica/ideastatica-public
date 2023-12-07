using CommunityToolkit.Mvvm.ComponentModel;
using IdeaStatiCa.Plugin.Api.RCS.Model;

namespace RcsApiClient.ViewModels
{
	public class SectionViewModel : ObservableObject
	{
		public SectionViewModel()
		{ }

		public SectionViewModel(RcsSectionModel rcsSectionModel)
		{
			Id = rcsSectionModel.Id;
			Description = rcsSectionModel.Description;
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

		private string description = string.Empty;
		public string Description
		{
			get => description;
			set
			{
				description = value;
				OnPropertyChanged(nameof(Description));
			}
		}
	}
}
