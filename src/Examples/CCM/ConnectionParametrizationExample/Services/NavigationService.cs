using CommunityToolkit.Mvvm.ComponentModel;
using ConnectionParametrizationExample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionParametrizationExample.Services
{
	public interface INavigatonService
	{
		ViewModel CurrentView { get; }
		void NavigateTo<T>() where T : ViewModel; 
	}

	public class NavigationService : ObservableObject, INavigatonService
	{
		private ViewModel currentView;

		public ViewModel CurrentView
		{
			get => currentView;
			private set
			{
				currentView = value;
				OnPropertyChanged();
			}
		}

		public readonly Func<Type, ViewModel> viewModelFactory;

		public NavigationService(Func<Type, ViewModel> viewModelFactory)
		{
			this.viewModelFactory = viewModelFactory;
		}

		public void NavigateTo<TViewModel>() where TViewModel : ViewModel
		{
			ViewModel viewModel = viewModelFactory.Invoke(typeof(TViewModel));
			CurrentView = viewModel;
		}
	}
}
