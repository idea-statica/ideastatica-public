using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Importers
{
	internal class ImporterDispatcher : IBimApiImporter
	{
		private readonly Dictionary<Type, int> _interfaceRank = new();
		private readonly ImporterManager _importerManager;

		public ImporterDispatcher(ImporterManager importerManager)
		{
			_importerManager = importerManager;
			
			_interfaceRank[typeof(IIdeaObject)] = 0;
			_interfaceRank[typeof(IIdeaPersistentObject)] = 0;
			_interfaceRank[typeof(IIdeaObjectWithResults)] = 0;
		}

		public T Get<T>(Identifier<T> identifier)
			where T : IIdeaObject
		{
			foreach (Type? type in GetSortedInterfaces(typeof(T)))
			{
				if (_importerManager.TryResolve(type, out IImporter? importer))
				{
					return importer.Create(identifier);
				}
			}

			throw new ArgumentException();
		}

		public IIdeaObject Get(IIdentifier identifier)
		{
			foreach (Type? type in GetSortedInterfaces(identifier.ObjectType))
			{
				if (_importerManager.TryResolve(type, out IImporter? importer))
				{
					return importer.Create(identifier);
				}
			}

			throw new ArgumentException();
		}

		private IEnumerable<Type> GetSortedInterfaces(Type type)
		{
			return type
				.GetInterfaces()
				.Concat(new Type[] { type })
				.OrderByDescending(x => GetInterfaceRank(x));
		}

		private int GetInterfaceRank(Type type)
		{
			if (_interfaceRank.TryGetValue(type, out int rank))
			{
				return rank;
			}

			rank = GetInterfaceRankInternal(type);
			_interfaceRank[type] = rank;

			return rank;
		}

		private int GetInterfaceRankInternal(Type type)
		{
			Type[]? interfaces = type.GetInterfaces();

			if (interfaces.Length == 0)
			{
				return 0;
			}

			return interfaces
				.Select(x => GetInterfaceRank(x) + 1)
				.Min();
		}
	}
}