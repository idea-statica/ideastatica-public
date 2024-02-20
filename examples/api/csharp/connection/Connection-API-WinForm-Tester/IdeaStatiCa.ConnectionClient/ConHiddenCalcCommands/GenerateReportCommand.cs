using IdeaStatiCa.ConnectionClient.Model;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Public;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public enum ConnReportTypeEnum
	{
		Pdf,
		Word,
		Zip
	}

	public class GenerateReportCommand : ConnHiddenCalcCommandBase
	{
		private ConnReportTypeEnum connReportTypeEnum;
		public GenerateReportCommand(IConHiddenCalcModel model, ConnReportTypeEnum reportType) : base(model)
		{
			connReportTypeEnum = reportType;
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		public override void Execute(object parameter)
		{
			var res = string.Empty;
			Model.SetResults("GenerateReport");
			IsCommandRunning = true;

			if (connReportTypeEnum != ConnReportTypeEnum.Zip)
			{
				GenerateReportFile(parameter, connReportTypeEnum);
			}
			else
			{
				GenerateReportFolder(parameter);
			}
		}

		private Task GenerateReportFolder(object parameter)
		{
			return Task.Run(() =>
			{
				try
				{
					var connection = (IConnectionId)parameter;
					var service = Model.GetConnectionService();

					// generate report for connection
					var settings = new ConnReportSettings();
					var resData = service.GenerateReport(connection.ConnectionId, settings);

					// get storage with generated report
					GrpcBlobStorageClient grpcBlobStorageClient = new GrpcBlobStorageClient(Logger, resData.Port);
					var blobStorage = new BlobStorageGrpc(grpcBlobStorageClient, resData.ReportId);

					try
					{
						SaveFileDialog saveFileDialog = new SaveFileDialog();
						saveFileDialog.Filter = "zip | *.zip";

						if (saveFileDialog.ShowDialog() == true)
						{
							using (var stream = saveFileDialog.OpenFile())
							{
								// copy html content to zip file
								using (BlobStorageInArchive archive = new BlobStorageInArchive(stream))
								{
									archive.CopyFrom(blobStorage);
								}
							}
						}
					}
					finally
					{
						grpcBlobStorageClient.Dispose();
					}

					Model.SetResults(resData);
				}
				catch (Exception e)
				{
					Model.SetStatusMessage(e.Message);
				}
				finally
				{
					IsCommandRunning = false;
				}
			});
		}

		private Task GenerateReportFile(object parameter, ConnReportTypeEnum reportType)
		{
			return Task.Run(() =>
			{
				try
				{
					var connection = (IConnectionId)parameter;
					var service = Model.GetConnectionService();

					// generate report for connection
					var settings = new ConnReportSettings();
					var saveFileDialog = new SaveFileDialog
					{
						Filter = reportType switch 
						{
							ConnReportTypeEnum.Pdf => "pdf | *.pdf", 
							ConnReportTypeEnum.Word => "doc | *.doc", 
							_ => throw new NotImplementedException($"Conversion for report type {reportType} is not implemented.") 
						}
					};

					if (saveFileDialog.ShowDialog() != true)
					{
						return;
					}

					switch(reportType)
					{
						case ConnReportTypeEnum.Pdf:
							service.GenerateReportPdf(connection.ConnectionId, saveFileDialog.FileName, new ConnReportSettings());
							break;
						case ConnReportTypeEnum.Word:
							service.GenerateReportWord(connection.ConnectionId, saveFileDialog.FileName, new ConnReportSettings());
							break;
					}			
				}
				catch (Exception e)
				{
					Model.SetStatusMessage(e.Message);
				}
				finally
				{
					IsCommandRunning = false;
				}
			});
		}

	}
}
