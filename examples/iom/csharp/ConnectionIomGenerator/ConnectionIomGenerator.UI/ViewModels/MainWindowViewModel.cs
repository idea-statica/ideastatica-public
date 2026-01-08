using CommunityToolkit.Mvvm.Input;
using ConnectionIomGenerator.Model;
using ConnectionIomGenerator.UI.Services;
using ConnectionIomGenerator.UI.Tools;
using System.Threading.Tasks;

namespace ConnectionIomGenerator.UI.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		private readonly IProjectService projectService;
		public MainWindowViewModel(IProjectService projectService)
		{
			this.projectService = projectService;
			GenerateIomCommand = new AsyncRelayCommand(DoSomethingAsync, CanDoSomethingAsync);

			ConnectionDefinitionJson = JsonTools.GetJsonText<ConnectionInput>(ConnectionInput.GetDefaultECEN());
		}

		public AsyncRelayCommand GenerateIomCommand { get; }

		private bool CanDoSomethingAsync()
		{
			return true;
		}

		private async Task DoSomethingAsync()
		{
			var projInfo = await projectService.CreateProjectAsync();
			MessageText = projInfo.Id;
		}

		private string? connectionDefinitionJson;
		public string? ConnectionDefinitionJson
		{
			get => connectionDefinitionJson;
			set => SetProperty(ref connectionDefinitionJson, value);
		}

		private string? messageText;
		public string? MessageText
		{
			get => messageText;
			set => SetProperty(ref messageText, value);
		}

		private string? status;
		public string? Status
		{
			get => status;
			set => SetProperty(ref status, value);
		}
	}
}
