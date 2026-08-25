using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Common;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <inheritdoc cref="IProject"/>
	/// <remarks>This class is not thread-safe.</remarks>
	public class ProjectNoCache : IProject, IBimIdMapAccess
	{
		private int _nextId = 1;

		private readonly Map<string, int> _map = new Map<string, int>();
		protected readonly Dictionary<int, IIdeaObject> _objectMapping = new Dictionary<int, IIdeaObject>();
		protected readonly Dictionary<int, IIdeaPersistenceToken> _persistenceTokens = new Dictionary<int, IIdeaPersistenceToken>();

		private readonly IPluginLogger _logger;
		private readonly IPersistence _persistence;
		protected readonly IObjectRestorer _objectRestorer;

		/// <summary>
		/// Creates an instance of Project.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="persistence">Instance of IPersistence for storing of id mapping.</param>
		/// <param name="objectRestorer">Object restorer</param>
		/// <exception cref="ArgumentNullException">Throws if any argument is null.</exception>
		public ProjectNoCache(IPluginLogger logger, IPersistence persistence, IObjectRestorer objectRestorer)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
			_objectRestorer = objectRestorer;

			persistence.DataLoaded += ReloadMapping;
			ReloadMapping();
		}

		/// <summary>
		/// Creates an instance of Project.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="persistence">Instance of IPersistence for storing of id mapping.</param>
		/// <exception cref="ArgumentNullException">Throws if any argument is null.</exception>
		public ProjectNoCache(IPluginLogger logger, IPersistence persistence)
			: this(logger, persistence, null)
		{
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

			StorePersistenceToken(obj, bimApiId, iomId);

			_logger.LogDebug($"Created new id mapping: BimApi id {bimApiId}, IOM id {iomId}");

			return iomId;
		}

		private void StorePersistenceToken(IIdeaObject obj, string bimApiId, int iomId)
		{
			if (obj is IIdeaPersistentObject persistentObject)
			{
				IIdeaPersistenceToken token = persistentObject.Token;

				if (token is null)
				{
					return;
				}
				_persistence.StoreToken(bimApiId, token);
				_persistenceTokens.Add(iomId, token);
			}
		}

		/// <inheritdoc cref="IProject.GetBimObject(int)"/>
		public virtual IIdeaObject GetBimObject(int id)
		{
			IIdeaObject obj;
			if (!(_objectRestorer is null))
			{
				if (_persistenceTokens.TryGetValue(id, out IIdeaPersistenceToken token))
				{
					obj = _objectRestorer.Restore(token);
					return obj;
				}
			}

			throw new KeyNotFoundException();
		}

		public IIdeaPersistenceToken GetPersistenceToken(int iomId)
		{
			return _persistenceTokens[iomId];
		}

		private static readonly PersistenceTokenConverter _sourceIdTokenConverter = new PersistenceTokenConverter();

		private static JsonSerializerSettings SourceIdTokenSettings()
			=> new JsonSerializerSettings
			{
				Formatting = Formatting.None,
				Converters = new List<JsonConverter> { _sourceIdTokenConverter },
				TypeNameHandling = TypeNameHandling.None,
			};

		// The durable, opaque form of one entity's identity: its BimApi id plus its persistence token. The
		// token is serialized with the same version-tolerant converter the on-disk store uses, so a token
		// stored by Model Coordinator survives a plugin-assembly version change.
		private sealed class PackedIdentity
		{
			public string BimApiId { get; set; }
			public IIdeaPersistenceToken Token { get; set; }
		}

		/// <inheritdoc cref="IBimIdMapAccess.ExportIdMap"/>
		public IReadOnlyCollection<(int IomId, string SourceIdToken)> ExportIdMap()
		{
			List<(int, string)> entries = new List<(int, string)>();
			foreach ((int iomId, string bimApiId) in _persistence.GetMappings())
			{
				_persistenceTokens.TryGetValue(iomId, out IIdeaPersistenceToken token);
				string packed = JsonConvert.SerializeObject(
					new PackedIdentity { BimApiId = bimApiId, Token = token },
					SourceIdTokenSettings());
				entries.Add((iomId, packed));
			}

			return entries;
		}

		/// <inheritdoc cref="IBimIdMapAccess.ImportIdMap"/>
		public void ImportIdMap(IEnumerable<(int IomId, string SourceIdToken)> entries)
		{
			foreach ((int iomId, string sourceIdToken) in entries)
			{
				PackedIdentity identity = JsonConvert.DeserializeObject<PackedIdentity>(sourceIdToken, SourceIdTokenSettings());
				if (identity is null)
				{
					continue;
				}

				_persistence.StoreMapping(iomId, identity.BimApiId);
				if (identity.Token != null)
				{
					_persistence.StoreToken(identity.BimApiId, identity.Token);
				}
			}

			ReloadMapping();
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