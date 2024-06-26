namespace IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors
{
	public sealed class TabInfoDescriptor
	{
		public bool CreateSeparateTab { get; set; } = false;

		public string TabName { get; set; } = "Plugins";

		public TabInfoDescriptor()
		{
		}

		public TabInfoDescriptor(bool createSeparateTab, string tabName)
		{
			CreateSeparateTab = createSeparateTab;
			TabName = tabName;
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

			if (obj.GetType() != GetType())
			{
				return false;
			}

			return Equals((TabInfoDescriptor)obj);
		}

		public bool Equals(TabInfoDescriptor other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return CreateSeparateTab == other.CreateSeparateTab
				&& TabName == other.TabName;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(CreateSeparateTab, TabName);
		}
	}
}
