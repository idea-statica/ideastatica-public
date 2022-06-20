using IdeaStatiCa.CheckbotPlugin;
using IdeaStatiCa.CheckbotPlugin.Common;
using System;

namespace IdeaStatiCa.PluginSystem.PluginList.Descriptors
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
		/// Full name (namespace + class name) a class that implements <see cref="IPlugin"/>.
		/// Optional, is set to null when not used.
		/// When null, the plugin runner will use an public class implementing <see cref="IPlugin"/>.
		/// </summary>
		public string ClassName { get; }

		public DotNetRunnerDriverDescriptor(string path)
		{
			Ensure.NotEmpty(path, nameof(path));

			Path = path;
			ClassName = null;
		}

		public DotNetRunnerDriverDescriptor(string path, string className)
		{
			Ensure.NotEmpty(path, nameof(path));
			Ensure.NotEmpty(className, nameof(className));

			Path = path;
			ClassName = className;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			return Equals(obj as DotNetRunnerDriverDescriptor);
		}

		public bool Equals(DotNetRunnerDriverDescriptor other)
		{
			if (other is null)
			{
				return false;
			}

			return Path == other.Path
				&& ClassName == other.ClassName;
		}

		public override int GetHashCode() => HashCode.Combine(Path, ClassName);
	}
}