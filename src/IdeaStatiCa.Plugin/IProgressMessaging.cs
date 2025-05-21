using System.Globalization;

namespace IdeaStatiCa.Plugin
{
	public enum MessageSeverity
	{
		Info,
		Warning,
		Error
	}

	public enum LocalisedMessage
	{
		ImportingGroups,
		ImportingMembers,
		ImportingConnections,
		InternalSync,
		SyncingModel,
		SavingProject,
		ModelImport,
		CancellingImport,
		CreatingConnection,
		ImportingIOMObject,
		ImportingStructuralModel,
		ImportStarted,
		ImportFailed,
		ModelPostProcess,
		InternalImport,
		Member,
		SavingData,
		ConvertingModelFromXML,
		ProcessingSubstructure,
		ProcessingConnection,
		FinishingImport,
		SciaSDKError,
		AwaitingUserSelection,
		ImportDetails
	}

	public interface IProgressMessaging
	{
		bool GetCancellationFlag(); // @Todo: flags aren't the ideal solution
		void SendMessageLocalised(MessageSeverity severity, LocalisedMessage msg, string suffix = "");
		void SendMessage(MessageSeverity severity, string text);
		int SendMessageInteractive(MessageSeverity severity, string text, string[] buttons);
		void CancelMessage();
		void InitProgressDialog();

		void SetStageLocalised(int stage, int stageMax, LocalisedMessage msg, params object[] args);
		void SetStage(int stage, int stageMax, string name);
		void SetStageProgress(double percentage);

		string GetLocalizedText(LocalisedMessage msg);
		CultureInfo GetCurrentCulture();
	}
}
