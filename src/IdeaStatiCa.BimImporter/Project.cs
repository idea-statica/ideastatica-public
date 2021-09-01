﻿using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Common;
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
		private int _nextId = 1;

		private Map<string, int> _map = new Map<string, int>();
		private Dictionary<int, IIdeaObject> _objectMapping = new Dictionary<int, IIdeaObject>();
		private Dictionary<int, IIdeaPersistenceToken> _persistenceTokens = new Dictionary<int, IIdeaPersistenceToken>();

		private readonly IPluginLogger _logger;
		private readonly IPersistence _persistence;
		private readonly IObjectRestorer _objectRestorer;

		/// <summary>
		/// Creates an instance of Project.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="persistence">Instance of IPersistence for storing of id mapping.</param>
		/// <exception cref="ArgumentNullException">Throws if any argument is null.</exception>
		public Project(IPluginLogger logger, IPersistence persistence, IObjectRestorer objectRestorer)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
			_objectRestorer = objectRestorer ?? throw new ArgumentNullException(nameof(objectRestorer));

			persistence.DataLoaded += ReloadMapping;
			ReloadMapping();
		}

		/// <inheritdoc cref="IProject.GetIomId(string)"/>
		public int GetIomId(string bimId)
		{
			return _map.GetRight(bimId);
		}

		public string GetBimApiId(int iomId)
		{
			return _map.GetLeft(iomId);
		}

		/// <inheritdoc cref="IProject.GetIomId(IIdeaObject)"/>
		/// <remarks>Stores all newly created id mappings. Also stores token of all unseen <see cref="IIdeaPersistentObject"/>.</remarks>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="obj"/> is null.</exception>
		public int GetIomId(IIdeaObject obj)
		{
			if (obj is null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			string bimApiId = obj.Id;

			if (_map.TryGetRight(bimApiId, out int iomId))
			{
				return iomId;
			}

			iomId = _nextId++;

			_map.Add(bimApiId, iomId);
			_objectMapping.Add(iomId, obj);

			_persistence.StoreMapping(iomId, bimApiId);

			if (obj is IIdeaPersistentObject persistentObject)
			{
				IIdeaPersistenceToken token = persistentObject.Token;
				_persistence.StoreToken(bimApiId, token);
				_persistenceTokens.Add(iomId, token);
			}

			_logger.LogDebug($"Created new id mapping: BimApi id {bimApiId}, IOM id {iomId}");

			return iomId;
		}

		/// <inheritdoc cref="IProject.GetBimObject(int)"/>
		public IIdeaObject GetBimObject(int id)
		{
			if (_objectMapping.TryGetValue(id, out IIdeaObject obj))
			{
				return obj;
			}

			if (_persistenceTokens.TryGetValue(id, out IIdeaPersistenceToken token))
			{
				obj = _objectRestorer.Restore(token);
				_objectMapping.Add(id, obj);
				return obj;
			}

			throw new KeyNotFoundException();
		}

		public IIdeaPersistenceToken GetPersistenceToken(int iomId)
		{
			return _persistenceTokens[iomId];
		}

		private void ReloadMapping()
		{
			_nextId = 1;
			_map.Clear();
			_persistenceTokens.Clear();

			foreach ((int iomId, string bimApiId) in _persistence.GetMappings())
			{
				_map.Add(bimApiId, iomId);

				// If current Id is 1 the next should be 2 so we do iomId+1
				_nextId = Math.Max(_nextId, iomId + 1);
			}

			foreach ((string bimApiId, IIdeaPersistenceToken token) in _persistence.GetTokens())
			{
				_persistenceTokens.Add(_map.GetRight(bimApiId), token);
			}
		}
	}
}