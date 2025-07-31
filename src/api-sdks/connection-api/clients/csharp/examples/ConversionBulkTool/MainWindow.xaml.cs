using IdeaRS.OpenModel;
using IdeaStatiCa.Api.Connection.Model.Conversion;
using IdeaStatiCa.ConnectionApi;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace IdeaConWpfApp
{
	public partial class MainWindow : Window
	{
		private string? ideaPath;
		private string? selectedFolderPath;
		private ObservableCollection<ProjectItem> projectFiles = new();
		private IConnectionApiClient? conClient;
		private ConnectionApiServiceRunner? service;

		public MainWindow()
		{
			InitializeComponent();
			ProjectsList.ItemsSource = projectFiles;
			IdeaPathText.Text = ideaPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.0\";
			DesignCodeComboBox.SelectedIndex = 0;
			this.Closed += MainWindow_Closed;
		}

		private void MainWindow_Closed(object? sender, EventArgs e)
		{
			service?.Dispose();
			conClient?.Dispose();
		}

		private void LoadIdeaPath_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select folder where is API v25",
				SelectedPath = @"C:\Program Files\IDEA StatiCa\StatiCa 25.0\"
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				ideaPath = dialog.SelectedPath;
				IdeaPathText.Text = ideaPath;  // Show selected path in the TextBlock
			}
		}

		private void SelectFolder_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new System.Windows.Forms.FolderBrowserDialog()
			{
				Description = "Select folder with .ideaCon files",
				SelectedPath = selectedFolderPath // Default to last selected folder
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				selectedFolderPath = dialog.SelectedPath;
				SelectedFolderTextBox.Text = selectedFolderPath;  // Show selected folder in the read-only TextBox

				// Clear and repopulate the ListBox with .ideaCon files
				projectFiles.Clear();

				var files = Directory.GetFiles(selectedFolderPath!, "*.ideaCon", SearchOption.AllDirectories);
				foreach (var file in files)
				{
					var fileName = Path.GetFileName(file);
					projectFiles.Add(new ProjectItem { FilePath = file, FileName = fileName });
				}
			}
		}

		public static CountryCode GetCountryCode(string selectedCode) => selectedCode switch
		{
			"AISC (America)" => CountryCode.American,
			"IS800 (India)" => CountryCode.India,
			"CSA (Canada)" => CountryCode.Canada,
			"AS (Australia)" => CountryCode.Australia,
			"GB (China)" => CountryCode.CHN,
			"HKG (Honk Kong)" => CountryCode.HKG,
			_ => throw new NotImplementedException($"Selected code not implemented {selectedCode}")
		};

		private async void CustomConversion_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(selectedFolderPath))
			{
				MessageBox.Show("Please load IdeaPath first.");
				return;
			}
			string? designCode = ((ComboBoxItem)DesignCodeComboBox.SelectedItem).Content.ToString();

			var steel = new Dictionary<string, string>();
			var welds = new Dictionary<string, string>();
			var concrete = new Dictionary<string, string>();
			var boltGrades = new Dictionary<string, string>();
			var boltAssemblies = new Dictionary<string, string>();
			var crossSections = new Dictionary<string, string>();

			if(service is null)
			{
				service = new ConnectionApiServiceRunner(ideaPath);
			}
			
			var countryCode = GetCountryCode(designCode!);

			Dispatcher.Invoke(() => MessageLabel.Text = "Starting Connection API service ...");

			using (var conClient = await service.CreateApiClient())
			{
				await Task.Run(async () =>
				{
					Dispatcher.Invoke(() => MessageLabel.Text = "Service started.");
					var path = Path.GetDirectoryName(projectFiles[0].FilePath ?? throw new ArgumentException("FilePath"));
					DirectoryInfo? convertedDir = Directory.CreateDirectory(Path.Combine(path!, "Converted"));

					foreach (var project in projectFiles)
					{
						Dispatcher.Invoke(() => MessageLabel.Text = $"Processing {project.FilePath}");
						try
						{
							await conClient.Project.OpenProjectAsync(project.FilePath);
							var defaultMapping = await conClient.Conversion.GetConversionMappingAsync(conClient.ActiveProjectId, countryCode);

							defaultMapping.Steel.ForEach(x => {
								steel.TryAdd(x.SourceValue, x.TargetValue);
								});
							defaultMapping.Welds.ForEach(x => {
								welds.TryAdd(x.SourceValue, x.TargetValue);
							});
							defaultMapping.BoltGrade.ForEach(x => {
								boltGrades.TryAdd(x.SourceValue, x.TargetValue);
							});
							defaultMapping.Fasteners.ForEach(x =>
							{
								boltAssemblies.TryAdd(x.SourceValue, x.TargetValue);
							});
							defaultMapping.Concrete.ForEach(x =>
							{
								concrete.TryAdd(x.SourceValue, x.TargetValue);
							});
							defaultMapping.CrossSections.ForEach(x =>
							{
								crossSections.TryAdd(x.SourceValue, x.SourceValue);
							});

							await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
						}
						catch (Exception ex)
						{
							project.IsFailed = true;      // ❌ mark failure
							Dispatcher.Invoke(() => MessageLabel.Text = $"Failed: {project.FilePath}");
						}
					}

					Dispatcher.Invoke(() => MessageLabel.Text = "All done!");
				});


				var customWindow = new CustomConversionWindow(steel, welds, boltAssemblies, boltGrades, concrete, crossSections);
				customWindow.Owner = this;

				if (customWindow.ShowDialog() == true)
				{
					steel = customWindow.SteelMapping;
					welds = customWindow.WeldsMapping;
					boltAssemblies = customWindow.BoltsMapping;
					boltGrades = customWindow.BoltGradesMapping;
					concrete = customWindow.ConcreteMapping;
					crossSections = customWindow.CrossSectionsMapping;
				}

				await Task.Run(async () =>
				{
					Dispatcher.Invoke(() => MessageLabel.Text = "Service started.");
					var path = Path.GetDirectoryName(projectFiles[0].FilePath ?? throw new ArgumentException("FilePath"));
					DirectoryInfo? convertedDir = Directory.CreateDirectory(Path.Combine(path!, "Converted"));

					foreach (var project in projectFiles)
					{
						Dispatcher.Invoke(() => MessageLabel.Text = $"Processing {project.FilePath}");
						try
						{
							await conClient.Project.OpenProjectAsync(project.FilePath);
							var defaultMapping = await conClient.Conversion.GetConversionMappingAsync(conClient.ActiveProjectId, countryCode);

							ApplyMappings(defaultMapping.Steel, steel);
							ApplyMappings(defaultMapping.Welds, welds);
							ApplyMappings(defaultMapping.Fasteners, boltAssemblies);
							ApplyMappings(defaultMapping.BoltGrade, boltGrades);
							ApplyMappings(defaultMapping.Concrete, concrete);
							ApplyMappings(defaultMapping.CrossSections, crossSections);

							await conClient.Conversion.ChangeCodeAsync(conClient.ActiveProjectId, defaultMapping);

							var output = Path.Combine(convertedDir.FullName,
								Path.GetFileNameWithoutExtension(project.FilePath) + "-" + designCode + ".ideaCon");

							await conClient.Project.SaveProjectAsync(conClient.ActiveProjectId, output);

							await conClient.Project.CloseProjectAsync(conClient.ActiveProjectId);
							project.IsProcessed = true;   // ✅ mark success
						}
						catch (Exception ex)
						{
							project.IsFailed = true;      // ❌ mark failure
							Dispatcher.Invoke(() => MessageLabel.Text = $"Failed: {project.FilePath}");
						}
					}

					Dispatcher.Invoke(() => MessageLabel.Text = "All done!");
				});
			}
		}

		void ApplyMappings(List<ConversionMapping> targetList, Dictionary<string, string> map)
		{
			foreach (var item in targetList)
			{
				if (map.TryGetValue(item.SourceValue, out var mapped))
				{
					item.TargetValue = mapped;
				}
			}
		}

	}

	public class ProjectItem : INotifyPropertyChanged
	{
		public string? FilePath { get; set; }

		public string? FileName { get; set; }

		private bool _isProcessed;
		public bool IsProcessed
		{
			get => _isProcessed;
			set
			{
				_isProcessed = value;
				OnPropertyChanged(nameof(IsProcessed));
			}
		}

		private bool _isFailed;
		public bool IsFailed
		{
			get => _isFailed;
			set
			{
				_isFailed = value;
				OnPropertyChanged(nameof(IsFailed));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
