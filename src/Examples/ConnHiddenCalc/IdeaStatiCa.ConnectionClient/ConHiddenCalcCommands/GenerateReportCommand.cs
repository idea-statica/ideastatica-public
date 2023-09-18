using IdeaStatiCa.ConnectionClient.Model;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.Plugin.Grpc;
using IdeaStatiCa.Public;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;

namespace IdeaStatiCa.ConnectionClient.Commands
{
	public class GenerateReportCommand : ConnHiddenCalcCommandBase
	{
		public GenerateReportCommand(IConHiddenCalcModel model) : base(model)
		{
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

			var connCalculatorTask = Task.Run(() =>
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
	}
}
