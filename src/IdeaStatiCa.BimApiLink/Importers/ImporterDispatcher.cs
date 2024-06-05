using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Hooks;
using IdeaStatiCa.BimApiLink.Identifiers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimApiLink.Importers
{
	internal class ImporterDispatcher : IBimApiImporter
	{
		private readonly Dictionary<Type, int> _interfaceRank = new Dictionary<Type, int>();
		private readonly ImporterManager _importerManager;
		private readonly IImporterHook _importerHookManager;

		public ImporterDispatcher(ImporterManager importerManager, IImporterHook importerHookManager)
		{
			_importerManager = importerManager;
			_importerHookManager = importerHookManager;
			_interfaceRank[typeof(IIdeaObject)] = 0;
			_interfaceRank[typeof(IIdeaPersistentObject)] = 0;
			_interfaceRank[typeof(IIdeaObjectWithResults)] = 0;
		}

		public T Get<T>(Identifier<T> identifier)
			where T : IIdeaObject
		{
			foreach (Type type in GetSortedInterfaces(typeof(T)))
			{
				if (_importerManager.TryResolve(type, out IImporter importer))
				{
					T obj;

					_importerHookManager.EnterCreate(identifier);
					try
					{
						obj = importer.Create(identifier);
						_importerHookManager.ExitCreate(identifier, obj);
					}
					catch
					{
						_importerHookManager.ExitCreate(identifier, null);
						throw;
					}

					return obj;
				}
			}

			throw new ArgumentException();
		}

		public IIdeaObject Get(IIdentifier identifier)
		{
			foreach (Type type in GetSortedInterfaces(identifier.ObjectType))
			{
				if (_importerManager.TryResolve(type, out IImporter importer))
				{
					IIdeaObject obj;

					_importerHookManager.EnterCreate(identifier);
					try
					{
						obj = importer.Create(identifier);
						_importerHookManager.ExitCreate(identifier, obj);
					}
					catch
					{
						_importerHookManager.ExitCreate(identifier, null);
						throw;
					}

					return obj;
				}
			}

			throw new ArgumentException();
		}

		public T Check<T>(Identifier<T> identifier)
			where T : IIdeaObject
		{
			foreach (Type type in GetSortedInterfaces(typeof(T)))
			{
				if (_importerManager.TryResolve(type, out IImporter importer))
				{
					T obj;

					obj = importer.Check(identifier);

					return obj;
				}
			}

			throw new ArgumentException();
		}

		public IIdeaObject Check(IIdentifier identifier)
		{
			foreach (Type type in GetSortedInterfaces(identifier.ObjectType))
			{
				if (_importerManager.TryResolve(type, out IImporter importer))
				{
					IIdeaObject obj;

					obj = importer.Check(identifier);

					return obj;
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
			Type[] interfaces = type.GetInterfaces();

			if (interfaces.Length == 0)
			{
				return 0;
			}

			return interfaces
				.Select(x => GetInterfaceRank(x) + 1)
				.Max();
		}
	}
}