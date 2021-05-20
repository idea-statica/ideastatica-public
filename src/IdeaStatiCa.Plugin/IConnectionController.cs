namespace IdeaStatiCa.Plugin
{
	public interface IConnectionController
	{
		bool IsConnected { get; }

		int OpenProject(string fileName);

		int CloseProject();

	}
}
