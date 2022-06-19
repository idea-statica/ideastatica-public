using System;

namespace IdeaStatiCa.PluginSystem.PluginList.Descriptors
{
	public sealed class PluginDescriptor : IEquatable<PluginDescriptor>
	{
		public string Name { get; }

		public PluginType Type { get; }

		public PluginDriverDescriptor DriverDescriptor { get; }

		public PluginDescriptor(string name, PluginType type, PluginDriverDescriptor driverDescriptor)
		{
			Name = name;
			Type = type;
			DriverDescriptor = driverDescriptor;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			return Equals(obj as PluginDescriptor);
		}

		public bool Equals(PluginDescriptor other)
		{
			if (other is null)
			{
				return false;
			}

			return Name == other.Name
				&& Type == other.Type
				&& DriverDescriptor.Equals(other.DriverDescriptor);
		}

		public override int GetHashCode() => HashCode.Combine(Name, Type, DriverDescriptor);
	}
}