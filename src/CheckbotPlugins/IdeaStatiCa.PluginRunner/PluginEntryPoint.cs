using IdeaStatiCa.CheckbotPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IdeaStatiCa.PluginRunner
{
	internal static class PluginEntryPoint
	{
		public static Type GetEntryPointClass(Assembly assembly, string? className)
		{
			Type entryPointClass;

			if (className is null)
			{
				entryPointClass = FindEntryPointClassByType(assembly) ?? throw new Exception("No entrypoint");
			}
			else
			{
				entryPointClass = FindEntryPointClassByName(assembly, className) ?? throw new Exception("No entrypoint");
			}

			if (!HasNonParametricConstructor(entryPointClass))
			{
				throw new Exception("No entrypoint");
			}

			return entryPointClass;
		}

		public static IPlugin GetInstance(Type type)
		{
			IPlugin? instance = (IPlugin?)Activator.CreateInstance(type);
			return instance ?? throw new Exception("Cannot initialize class");
		}

		private static Type? FindEntryPointClassByType(Assembly assembly)
		{
			return GetCandidateTypes(assembly)
				.FirstOrDefault();
		}

		private static Type? FindEntryPointClassByName(Assembly assembly, string className)
		{
			return GetCandidateTypes(assembly)
				.FirstOrDefault(x => x.FullName == className);
		}

		private static bool HasNonParametricConstructor(Type type)
		{
			return type.GetConstructors()
				.Where(x => x.IsPublic)
				.Any(x => x.GetParameters().Length == 0);
		}

		private static IEnumerable<Type> GetCandidateTypes(Assembly assembly)
		{
			return assembly.GetExportedTypes()
				.Where(x => x.IsClass)
				.Where(x => x.IsSubclassOf(typeof(IPlugin)));
		}
	}
}