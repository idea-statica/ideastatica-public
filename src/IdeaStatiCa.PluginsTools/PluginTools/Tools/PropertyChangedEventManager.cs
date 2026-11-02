using CI.Common;

namespace CI.DataModel
{
	/// <summary>
	/// Weak PropertyChangedEvent 
	/// </summary>
	public class PropertyChangedEventManager : WeakEventManagerBase<PropertyChangedEventManager, ElementBase>
	{
		protected override void StartListeningTo(ElementBase source)
		{
#if !CLOUD
			if (!OSHelper.DisableWeakEventListening)
			{
				source.ObjectChanged += DeliverEvent;
			}
#endif
		}

		protected override void StopListeningTo(ElementBase source)
		{
#if !CLOUD
			if (!OSHelper.DisableWeakEventListening)
			{
				source.ObjectChanged -= DeliverEvent;
			}
#endif
		}
	}
}
