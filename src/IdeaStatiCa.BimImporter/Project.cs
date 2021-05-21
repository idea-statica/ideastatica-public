using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <inheritdoc cref="IProject"/>
	/// <remarks>This class is not thread-safe.</remarks>
	public class Project : IProject
	{
		private int _nextId = 0;

		private Dictionary<string, int> _idMapping = new Dictionary<string, int>();
		private Dictionary<int, IIdeaObject> _objectMapping = new Dictionary<int, IIdeaObject>();

		private readonly IPluginLogger _logger;
		private readonly IPersistence _persistence;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="logger"/> is null.</exception>
		public Project(IPluginLogger logger, IObjectRestorer objectRestorer, IPersistence persistence)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_persistence = persistence;

			Load(objectRestorer);
		}

		/// <inheritdoc cref="IProject.GetIomId(string)"/>
		public int GetIomId(string bimId)
		{
			return _idMapping[bimId];
		}

		/// <inheritdoc cref="IProject.GetIomId(IIdeaObject)"/>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="obj"/> is null.</exception>
		public int GetIomId(IIdeaObject obj)
		{
			if (obj is null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			string bimApiId = obj.Id;

			if (_idMapping.TryGetValue(bimApiId, out int iomId))
			{
				return iomId;
			}

			iomId = _nextId++;

			_idMapping.Add(bimApiId, iomId);
			_objectMapping.Add(iomId, obj);

			_persistence.StoreMapping(iomId, bimApiId);

			if (obj is IIdeaPersistentObject persistentObject)
			{
				_persistence.StoreToken(persistentObject.Token);
			}

			_logger.LogDebug($"Created new id mapping: BimApi id {bimApiId}, IOM id {iomId}");

			return iomId;
		}

		/// <inheritdoc cref="IProject.GetBimObject(int)"/>
		public IIdeaObject GetBimObject(int id)
		{
			return _objectMapping[id];
		}

		private void Load(IObjectRestorer objectRestorer)
		{
			Dictionary<string, int> idMapping = new Dictionary<string, int>();
			Dictionary<int, IIdeaObject> objectMapping = new Dictionary<int, IIdeaObject>();

			int maxId = 0;

			foreach ((int iomId, string bimApiId) in _persistence.GetMappings())
			{
				idMapping.Add(bimApiId, iomId);
				maxId = Math.Max(maxId, iomId);
			}

			foreach (IIdeaPersistenceToken token in _persistence.GetTokens())
			{
				IIdeaObject obj = objectRestorer.Restore(token);
				objectMapping.Add(idMapping[obj.Id], obj);
			}

			_idMapping = idMapping;
			_objectMapping = objectMapping;
			_nextId = maxId;
		}
	}
}