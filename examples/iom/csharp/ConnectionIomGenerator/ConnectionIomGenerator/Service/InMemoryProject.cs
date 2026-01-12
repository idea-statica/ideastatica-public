using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;

namespace ConnectionIomGenerator.Service
{
	/// <summary>
	/// Simple in-memory implementation of IProject for ID mapping
	/// </summary>
	internal class InMemoryProject : IProject
	{
		private readonly Dictionary<string, int> _bimToIomIdMap = new();
		private readonly Dictionary<int, string> _iomToBimIdMap = new();
		private int _nextId = 1;

		public int GetIomId(string bimApiId)
		{
			if (string.IsNullOrEmpty(bimApiId))
			{
				throw new ArgumentNullException(nameof(bimApiId));
			}

			if (!_bimToIomIdMap.TryGetValue(bimApiId, out int iomId))
			{
				throw new KeyNotFoundException($"No mapping found for BimApi ID: {bimApiId}");
			}

			return iomId;
		}

		public string GetBimApiId(int iomId)
		{
			if (!_iomToBimIdMap.TryGetValue(iomId, out string bimApiId))
			{
				throw new KeyNotFoundException($"No mapping found for IOM ID: {iomId}");
			}

			return bimApiId;
		}

		public IIdeaObject GetBimObject(int iomId)
		{
			// Not needed for import, only for re-import scenarios
			throw new NotImplementedException("GetBimObject is not implemented for initial import scenarios");
		}

		public int GetIomId(IIdeaObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			if (!_bimToIomIdMap.TryGetValue(obj.Id, out int iomId))
			{
				iomId = _nextId++;
				_bimToIomIdMap[obj.Id] = iomId;
				_iomToBimIdMap[iomId] = obj.Id;
			}

			return iomId;
		}

		public IIdeaPersistenceToken GetPersistenceToken(int iomId)
		{
			// Return a simple token with the BimApi ID
			string bimApiId = GetBimApiId(iomId);
			return new SimplePersistenceToken(bimApiId);
		}

		/// <summary>
		/// Simple implementation of IIdeaPersistenceToken
		/// </summary>
		private class SimplePersistenceToken : IIdeaPersistenceToken
		{
			public SimplePersistenceToken(string token)
			{
				Token = token ?? throw new ArgumentNullException(nameof(token));
			}

			public string Token { get; }
		}
	}
}