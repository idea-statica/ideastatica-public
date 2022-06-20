using IdeaStatiCa.CheckbotPlugin.Common;
using System;

namespace IdeaStatiCa.PluginSystem.PluginList.Descriptors
{
	public sealed class PluginDescriptor : IEquatable<PluginDescriptor>
	{
		/// <summary>
		/// An unique name of the plugin, it should stay the same across different versions.
		/// This name won't be shown to the user, for that use the display name in the PluginHello.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Whether the plugin is of <see cref="PluginType.Check"/> or <see cref="PluginType.Import"/> type.
		/// </summary>
		public PluginType Type { get; }

		/// <summary>
		/// Driver descriptors says how the plugin is launched.
		/// </summary>
		public PluginDriverDescriptor DriverDescriptor { get; }

		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="driverDescriptor"></param>
		public PluginDescriptor(string name, PluginType type, PluginDriverDescriptor driverDescriptor)
		{
			Ensure.NotEmpty(name, nameof(name));
			Ensure.NotNull(driverDescriptor, nameof(driverDescriptor));

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