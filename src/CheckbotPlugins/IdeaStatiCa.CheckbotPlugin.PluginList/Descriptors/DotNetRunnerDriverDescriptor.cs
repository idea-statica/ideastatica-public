using IdeaStatiCa.CheckbotPlugin.PluginList.Utils;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors
{
	/// <summary>
	/// Plugins with <see cref="DotNetRunnerDriverDescriptor"/> are started by the Plugin Runner.
	/// </summary>
	public sealed class DotNetRunnerDriverDescriptor : PluginDriverDescriptor, IEquatable<DotNetRunnerDriverDescriptor>
	{
		/// <summary>
		/// Full path for the plugin dll.
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// Full name (namespace + class name) a class that implements <c>IdeaStatiCa.CheckbotPlugin.IPlugin</c>.
		/// Optional, is set to null when not used.
		/// When null, the plugin runner will use an public class implementing <c>IdeaStatiCa.CheckbotPlugin.IPlugin</c>.
		/// </summary>
		public string? ClassName { get; }

		public DotNetRunnerDriverDescriptor(string path)
		{
			Ensure.NotEmpty(path);

			Path = path;
			ClassName = null;
		}

		public DotNetRunnerDriverDescriptor(string path, string? className)
		{
			Ensure.NotEmpty(path);

			Path = path;
			ClassName = string.IsNullOrEmpty(className) ? null : className;
		}

		public override bool Equals(object? obj)
			=> Equals(obj as DotNetRunnerDriverDescriptor);

		public bool Equals(DotNetRunnerDriverDescriptor? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Path == other.Path
				&& ClassName == other.ClassName;
		}

		public override int GetHashCode()
			=> HashCode.Combine(Path, ClassName);
	}
}