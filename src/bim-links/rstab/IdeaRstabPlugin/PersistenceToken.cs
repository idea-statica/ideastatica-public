using IdeaStatiCa.BimImporter.Persistence;

namespace IdeaRstabPlugin
{
	/// <summary>
	/// Stores object type and number for recreating the object at any time.
	/// </summary>
	internal class PersistenceToken : AbstractPersistenceToken
	{
		/// <summary>
		/// RSTAB Number
		/// </summary>
		public int No { get; set; }

		public PersistenceToken(TokenObjectType tokenType, int no)
		{
			Type = tokenType;
			No = no;
		}
	}
}