using CommunityToolkit.Mvvm.ComponentModel;
using IdeaStatiCa.Plugin.Api.RCS.Model;
using System;

namespace RcsApiClient.ViewModels
{
	public class SectionViewModel : ObservableObject
	{
		public SectionViewModel()
		{ }

		public SectionViewModel(RcsSectionModel rcsSectionModel)
		{
			if (rcsSectionModel == null || rcsSectionModel?.RCSId == null)
			{
				throw new ArgumentException("SectionViewModel() : invalid rcsSectionModel");
			}

			Id = rcsSectionModel.Id;
			Description = rcsSectionModel.Description;
			ReinforcedCssId = rcsSectionModel.RCSId.Value;
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


		private int rfId;
		public int ReinforcedCssId
		{
			get => rfId;
			set
			{
				rfId = value;
				OnPropertyChanged(nameof(ReinforcedCssId));
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
