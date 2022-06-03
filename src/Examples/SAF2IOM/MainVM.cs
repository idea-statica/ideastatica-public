using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using IdeaStatiCa.SAF2IOM;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

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

		public ModelBIM BimModel { get; set; }

		private bool CanOpenSaf(object param)
		{
			return string.IsNullOrEmpty(IOM);
		}

		private void OpenSaf(object param)
		{
			OpenFileDialog openFilleDlg = new OpenFileDialog();
			openFilleDlg.Filter = "SAF documents (.xlsx)|*.xlsx";
			openFilleDlg.CheckFileExists = true;
			var res = openFilleDlg.ShowDialog();
			if(res == true)
			{
				try
				{
					using (var ms = new MemoryStream())
					{
						using (var safStrema = new FileStream(openFilleDlg.FileName, FileMode.Open, FileAccess.Read))
						{
							BimModel = SafToIomConverter.Convert(new SafConvertInput(IdeaRS.OpenModel.CountryCode.ECEN, safStrema, ms));
							if (BimModel?.Model != null)
							{
								IOM = Tools.SerializeObject<OpenModel>(BimModel.Model);
							}
						}
					}
				}
				catch(Exception ex)
				{
					Logger.LogDebug("Importing SAF model failed", ex);
					MessageBox.Show("Can not open document", "Error", MessageBoxButton.OK);
					IOM = $"Converting SAF to IOM failed : {ex.Message}";
				}
			}
		}

		private bool CanSaveIom(object param)
		{
			return !string.IsNullOrEmpty(IOM);
		}

		private void SaveIom(object param)
		{

		}

		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
