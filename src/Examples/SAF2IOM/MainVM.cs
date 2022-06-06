using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.SAF2IOM;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SAF2IOM
{
	public class MainVM : INotifyPropertyChanged
	{
		private static IdeaStatiCa.Plugin.IPluginLogger Logger { get; set; }
		private static readonly SAF2IOMConverter SafToIomConverter = null;
		public event PropertyChangedEventHandler PropertyChanged;

		public MainVM()
		{
			this.OpenSafCommand = new CustomCommand(CanOpenSaf, OpenSaf);
			this.SaveIomCommand = new CustomCommand(CanSaveIom, SaveIom);
		}

		static MainVM()
		{
			// initialize logger
			SerilogFacade.Initialize();
			Logger = LoggerProvider.GetLogger("saf2iom");
			// create PluginLogger for import IOM from SAF and initialize it
			SafIomLogger.Init(Logger);
			SafToIomConverter = new SAF2IOMConverter();
		}

		public ICommand OpenSafCommand {get;set;}
		public ICommand SaveIomCommand { get; set; }

		private string iom;
		public string IOM
		{
			get { return iom; }
			set
			{
				iom = value;
				NotifyPropertyChanged("IOM");
			}
		}

	private bool isWorking;
		public bool IsWorking
		{
			get { return isWorking; }
			set
			{
				isWorking = value;
				NotifyPropertyChanged("IsWorking");
			}
		}

		public ModelBIM BimModel { get; set; }

		private bool CanOpenSaf(object param)
		{
			if (IsWorking)
			{
				return false;
			}

			return true;
		}

		private async void OpenSaf(object param)
		{
			OpenFileDialog openFileDlg = new OpenFileDialog();
			openFileDlg.Filter = "SAF documents (.xlsx)|*.xlsx";
			openFileDlg.CheckFileExists = true;
			var res = openFileDlg.ShowDialog();
			if(res == true)
			{
				try
				{
					IsWorking = true;
					Mouse.OverrideCursor = Cursors.Wait;
					IOM = "Generating IOM";
					IOM = await GenerateIOM(openFileDlg.FileName);
				}
				finally
				{
					IsWorking = false;
					Mouse.OverrideCursor = null;
				}
			}
		}

		private async Task<string> GenerateIOM(string safFileName)
		{
			var generateIomTask = Task<string>.Run(() =>
			{
				string res = string.Empty;
				try
				{
					using (var ms = new MemoryStream())
					{
						using (var safStrema = new FileStream(safFileName, FileMode.Open, FileAccess.Read))
						{
							BimModel = SafToIomConverter.Convert(new SafConvertInput(IdeaRS.OpenModel.CountryCode.ECEN, safStrema, ms));
							if (BimModel?.Model != null)
							{
								res = Tools.SerializeObject<OpenModel>(BimModel.Model);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.LogDebug("Importing SAF model failed", ex);
					MessageBox.Show("Can not open document", "Error", MessageBoxButton.OK);
					res = $"Converting SAF to IOM failed : {ex.Message}";
				}
				return res;
			});

			var iomResult = await generateIomTask;
			return iomResult;
		}

		private bool CanSaveIom(object param)
		{
			if(IsWorking)
			{
				return false;
			}

			return !string.IsNullOrEmpty(IOM);
		}

		private void SaveIom(object param)
		{
			SaveFileDialog saveFileDlg = new SaveFileDialog();
			saveFileDlg.Filter = "IOM documents (.iom)|*.iom";
			saveFileDlg.CheckPathExists = true;
			saveFileDlg.OverwritePrompt = true;

			var res = saveFileDlg.ShowDialog();
			if (res == true)
			{
				File.WriteAllText(saveFileDlg.FileName, IOM);
			}
		}

		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
