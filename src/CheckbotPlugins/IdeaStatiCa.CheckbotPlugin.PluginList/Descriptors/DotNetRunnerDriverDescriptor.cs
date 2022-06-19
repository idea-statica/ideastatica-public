using System;

namespace IdeaStatiCa.PluginSystem.PluginList.Descriptors
{
	public sealed class DotNetRunnerDriverDescriptor : PluginDriverDescriptor, IEquatable<DotNetRunnerDriverDescriptor>
	{
		public string Path { get; }

		public string ClassName { get; }

		public DotNetRunnerDriverDescriptor(string path, string className)
		{
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