using System;

namespace CI.DataModel
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class NotForAutoCopy : Attribute
	{
	}
}
