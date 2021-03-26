using IdeaStatiCa.ConnectionClient.Commands;
using IdeaStatiCa.ConnectionClient.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IdeaStatiCa.ConnectionClient.ConHiddenCalcCommands
{
	public class ApplySimpleTemplateCommand : ConnHiddenCalcCommandBase
	{
		public ApplySimpleTemplateCommand(IConHiddenCalcModel model) : base(model)
		{
		}

		public override bool CanExecute(object parameter)
		{
			return (Model.IsIdea && Model.IsService && Model.SelectedConnection != null && !IsCommandRunning);
		}

		/// <summary>
		/// Apply the simple connection template from a user's file
		/// </summary>
		/// <param name="parameter">It's a field that contains three items. on: [0] is informations about operation, [1] is supporting Beam, [2] is connected beam </param>
		public override void Execute(object parameter)
		{
			var values = (object[])parameter;

			List<int> attachedMembers = new List<int>();

			var mainMember = int.Parse(values[1].ToString());
			attachedMembers.Add(int.Parse(values[2].ToString()));

			var res = string.Empty;
			Model.SetResults("Apply Simple Template");
			IsCommandRunning = true;

			string connTemplateFileName = string.Empty;

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Idea Connection Template| *.contemp";
			if (openFileDialog.ShowDialog() != true)
			{
				return;
			}
			else
			{
				connTemplateFileName = openFileDialog.FileName;
			}

			var applySimpleTemplateTask = Task.Run(() =>
			{
				try
				{
					var connection = (IConnectionId)values[0];
					var service = Model.GetConnectionService();

					var resData = service.ApplySimpleTemplate(connection.ConnectionId, connTemplateFileName, Model.TemplateSetting, mainMember, attachedMembers);

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
