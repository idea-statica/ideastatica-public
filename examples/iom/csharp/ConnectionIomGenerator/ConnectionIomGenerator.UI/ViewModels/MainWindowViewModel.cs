using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using ConnectionIomGenerator.UI.Services;

namespace ConnectionIomGenerator.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly IProjectService projectService;
		public MainWindowViewModel(IProjectService projectService)
		{
			this.projectService = projectService;
			DoSomethingCommand = new AsyncRelayCommand<string>(DoSomethingAsync, CanDoSomethingAsync);
		}

		public IAsyncRelayCommand<string> DoSomethingCommand { get; }

		private bool CanDoSomethingAsync(string? name)
		{
			return true;
		}

		private async Task DoSomethingAsync(string? name)
		{
			var projInfo = await projectService.CreateProjectAsync();
			Status = projInfo.Id;
		}

		private string? status;
		public string? Status
		{
			get => status;
			set => SetProperty(ref status, value);
		}
	}
}
