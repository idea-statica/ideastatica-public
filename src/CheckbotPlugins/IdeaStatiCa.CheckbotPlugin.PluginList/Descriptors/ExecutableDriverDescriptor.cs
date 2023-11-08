using IdeaStatiCa.CheckbotPlugin.Common;

namespace IdeaStatiCa.PluginSystem.PluginList.Descriptors
{
	public sealed class ExecutableDriverDescriptor : PluginDriverDescriptor, IEquatable<ExecutableDriverDescriptor>
	{
		public string Path { get; }

		public string[] AdditionalArguments { get; }

		public ExecutableDriverDescriptor(string path, string[] additionalArguments)
		{
			Ensure.NotEmpty(path, nameof(path));

			Path = path;
			AdditionalArguments = additionalArguments ?? new string[0];
		}

		public override bool Equals(object? obj)
		{
			if (obj is null)
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			return Equals(obj as ExecutableDriverDescriptor);
		}

		public bool Equals(ExecutableDriverDescriptor? other)
		{
			if (other is null)
			{
				return false;
			}

			return Path == other.Path
				&& EqualityComparer<string[]>.Default.Equals(AdditionalArguments, other.AdditionalArguments);
		}

		public override int GetHashCode()
			=> HashCode.Combine(Path, AdditionalArguments);
	}
}