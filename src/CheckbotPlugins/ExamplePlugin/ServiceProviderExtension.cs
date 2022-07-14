namespace ExamplePlugin
{
	internal static class ServiceProviderExtension
	{
		public static T GetService<T>(this IServiceProvider serviceProvider)
		{
			return (T?)serviceProvider.GetService(typeof(T)) ?? throw new ArgumentException();
		}
	}
}