using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter;

namespace IdeaStatica.BimApiLink.Plugin
{
	internal static class PluginExtension
	{
		public static Identifier<T> GetIdentifier<T>(this IProject project, int iomId)
			where T : IIdeaObject
		{
			IIdentifier identifier = project.GetIdentifier(iomId);

			if (identifier.ObjectType == typeof(T) && identifier is Identifier<T> id)
			{
				return id;
			}

			throw new ArgumentException();
		}

		public static IIdentifier GetIdentifier(this IProject project, int iomId)
		{
			IIdeaPersistenceToken token = project.GetPersistenceToken(iomId);

			if (token is IIdentifier identifier)
			{
				return identifier;
			}

			throw new ArgumentException();
		}
	}
}