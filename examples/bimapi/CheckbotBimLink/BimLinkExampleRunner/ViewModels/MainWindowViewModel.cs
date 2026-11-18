using CadExampleApi;
using CommunityToolkit.Mvvm.Input;
using FeaExampleApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BimLinkExampleRunner.ViewModels
{
	/// <summary>
	/// What the window shows: the link to start, the path to Checkbot, and the log of the link.
	/// </summary>
	internal class MainWindowViewModel
	{
		public MainWindowViewModel()
		{
			// A link that fails has logged why, so its exception goes to the task scheduler instead of closing the
			// window.
			RunCheckbotCommand = new AsyncRelayCommand(
				RunCheckbotAsync,
				() => File.Exists(CheckbotLocation),
				AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
			Logger = new AppLogger(Dispatcher.CurrentDispatcher);
		}

		/// <summary>
		/// The links the runner can start.
		/// </summary>
		public IReadOnlyList<ApplicationType> ApplicationTypes { get; } = new[] { ApplicationType.CAD, ApplicationType.FEA };

		/// <summary>
		/// The link to start. The runner keeps it for its next start.
		/// </summary>
		public ApplicationType ApplicationType
		{
			get => Properties.Settings.Default.ApplicationType;
			set
			{
				Properties.Settings.Default.ApplicationType = value;
				Properties.Settings.Default.Save();
			}
		}

		/// <summary>
		/// Full path to IdeaCheckbot.exe. The runner keeps it for its next start.
		/// </summary>
		public string CheckbotLocation
		{
			get => Properties.Settings.Default.CheckbotLocation;
			set
			{
				Properties.Settings.Default.CheckbotLocation = value;
				Properties.Settings.Default.Save();
				RunCheckbotCommand.NotifyCanExecuteChanged();
			}
		}

		/// <summary>
		/// Starts the selected link. It is off while <see cref="CheckbotLocation"/> is not an existing file, and while
		/// the link runs, which is until Checkbot is closed.
		/// </summary>
		public AsyncRelayCommand RunCheckbotCommand { get; }

		public AppLogger Logger { get; }

		/// <summary>
		/// Runs the selected link until Checkbot is closed. The link waits a few seconds for Checkbot to start before
		/// it returns its task, so it runs on a pool thread to keep the window responsive.
		/// </summary>
		private Task RunCheckbotAsync()
		{
			string checkbotPath = CheckbotLocation;

			switch (ApplicationType)
			{
				case ApplicationType.CAD:
					string cadProjectPath = ProjectPath("CadExampleCheckbotProject");
					return Task.Run(() => BimApiLinkCadExample.CheckbotLink.Run(checkbotPath, cadProjectPath, new CadApi(), Logger));

				case ApplicationType.FEA:
					string feaProjectPath = ProjectPath("FeaExampleCheckbotProject");
					return Task.Run(() => BimApiLinkFeaExample.CheckbotLink.Run(checkbotPath, feaProjectPath, new FeaApi(), Logger));

				default:
					throw new InvalidOperationException($"No link for the application type {ApplicationType}.");
			}
		}

		/// <summary>
		/// The Checkbot project of a link is kept in a folder next to the runner, so the next run opens it again.
		/// </summary>
		private static string ProjectPath(string folderName)
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
		}
	}
}
