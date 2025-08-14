using IdeaStatiCa.CheckbotPlugin.PluginList.Utils;

namespace IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors
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
		/// Describe how the plugin tab should be displayed in the UI.
		/// </summary>
		public TabInfoDescriptor TabInfoDescriptor { get; }

		public SystemActionsDescriptor? SystemActionsDescriptor { get; }

		public ActionButtonDescriptor[]? CustomActionDescriptors { get; }

		/// <summary>
		/// Ctor.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="driverDescriptor"></param>
		/// <param name="tabInfoDescriptor"></param>
		/// <param name="systemActionsDescriptor"></param>
		/// <param name="customActionDescriptors"></param>
		public PluginDescriptor(
			string name,
			PluginType type,
			PluginDriverDescriptor driverDescriptor,
			TabInfoDescriptor tabInfoDescriptor,
			SystemActionsDescriptor? systemActionsDescriptor = null,
			ActionButtonDescriptor[]? customActionDescriptors = null)
		{
			Ensure.NotEmpty(name, nameof(name));
			Ensure.NotNull(driverDescriptor, nameof(driverDescriptor));

			Name = name;
			Type = type;
			DriverDescriptor = driverDescriptor;
			TabInfoDescriptor = tabInfoDescriptor;
			SystemActionsDescriptor = systemActionsDescriptor;
			CustomActionDescriptors = customActionDescriptors;
		}

		public bool Equals(PluginDescriptor? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Name == other.Name
				&& Type == other.Type
				&& DriverDescriptor.Equals(other.DriverDescriptor)
				&& TabInfoDescriptor.Equals(other.TabInfoDescriptor);
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as PluginDescriptor);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Type, DriverDescriptor, TabInfoDescriptor);
		}

		public static bool operator ==(PluginDescriptor left, PluginDescriptor right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(PluginDescriptor left, PluginDescriptor right)
		{
			return !Equals(left, right);
		}
	}
}