using IdeaStatiCa.BimImporter.Persistence;

namespace IdeaStatiCa.RamToIdea
{
	/// <summary>
	/// Stores object type and number for recreating the object at any time.
	/// </summary>
	internal class PersistenceToken : AbstractPersistenceToken
	{
		/// <summary>
		/// Ram Unique ID Number
		/// </summary>
		public int UID { get; set; }

		public PersistenceToken(TokenObjectType tokenType, int uID)
		{
			Type = tokenType;
			UID = uID;
		}
	}
}
