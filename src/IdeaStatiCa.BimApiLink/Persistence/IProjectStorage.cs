namespace IdeaStatiCa.BimApiLink.Persistence
{
	public interface IProjectStorage
	{
		void Load();

		void Save();

		bool IsValid();
	}
}