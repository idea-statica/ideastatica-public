namespace IdeaStatiCa.CheckbotPlugin.PluginList.Descriptors
{
	public sealed class TabInfoDescriptor
	{
		private static readonly string _defaultTabName = "Plugins";

		public bool CreateSeparateTab => !TabName.Equals(_defaultTabName);

		public string TabName { get; set; } = _defaultTabName;

		public TabInfoDescriptor()
		{
		}

		public TabInfoDescriptor(string tabName)
		{
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

			return TabName == other.TabName;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(TabName);
		}
	}
}
