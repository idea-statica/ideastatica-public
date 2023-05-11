using CommunityToolkit.Mvvm.Input;
using ConnectionParametrizationExample.Models;
using ConnectionParametrizationExample.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ConnectionParametrizationExample.ViewModels
{
	public class MainWindowViewModel : ViewModel
	{
		private INavigatonService navigatonService;

		public INavigatonService Navigation
		{
			get => navigatonService;
			set
			{
				navigatonService = value;
				OnPropertyChanged();
			}
		}

		public RelayCommand NavigateToParamatrizationCommand { get; }

		public RelayCommand NavigateToModelInfoCommand { get; }

		public MainWindowViewModel(INavigatonService navigationService)
		{
			Navigation = navigationService;
			NavigateToParamatrizationCommand = new RelayCommand(() => Navigation.NavigateTo<ParametrizationViewModel>());
			NavigateToModelInfoCommand = new RelayCommand(() => Navigation.NavigateTo<ModelInfoViewModel>());
			Navigation.NavigateTo<ParametrizationViewModel>();
		}
	}
}
