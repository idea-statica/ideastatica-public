using IdeaStatiCa.Plugin;

namespace IdeaStatiCa.BimApiLink.Plugin
{
	public interface IProgressMessagingAware
	{
		IProgressMessaging ProgressMessaging { get; set; }
	}
}
