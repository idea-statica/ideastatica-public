using Autofac.Core.Activators.Reflection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace IdeaStatiCa.RamToIdea.Utilities
{
	internal class AllConstructorFinder : IConstructorFinder
	{
		private static readonly ConcurrentDictionary<Type, ConstructorInfo[]> _cache =
			new ConcurrentDictionary<Type, ConstructorInfo[]>();

		public ConstructorInfo[] FindConstructors(Type targetType)
		{
			ConstructorInfo[] result = _cache.GetOrAdd(targetType,
				x => x.GetTypeInfo().DeclaredConstructors.Where(y => !y.IsStatic).ToArray());

			return result.Length > 0 ? result : throw new NoConstructorsFoundException(targetType);
		}
	}
}