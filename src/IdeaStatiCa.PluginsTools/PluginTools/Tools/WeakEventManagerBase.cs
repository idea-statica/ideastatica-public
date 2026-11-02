using System.Windows;

namespace CI.Common
{
	/// <summary>
	/// WeakEventManagerBase
	/// </summary>
	/// <typeparam name="TManager">Manager</typeparam>
	/// <typeparam name="TEventSource">Event source</typeparam>
	public abstract class WeakEventManagerBase<TManager, TEventSource> : WeakEventManager
		where TManager : WeakEventManagerBase<TManager, TEventSource>, new()
		where TEventSource : class
	{
		/// <summary>
		/// Adds a listener
		/// </summary>
		/// <param name="source">The source of the event, should be null if listening to static events</param>
		/// <param name="listener">The listener of the event. This is the class that will recieve the ReceiveWeakEvent method call</param>
		public static void AddListener(object source, IWeakEventListener listener)
		{
			if (!OSHelper.DisableWeakEventListening)
			{
				CurrentManager?.ProtectedAddListener(source, listener);
			}
		}

		/// <summary>
		/// Removes a listener
		/// </summary>
		/// <param name="source">The source of the event, should be null if listening to static events</param>
		/// <param name="listener">The listener of the event. This is the class that will recieve the ReceiveWeakEvent method call</param>
		public static void RemoveListener(object source, IWeakEventListener listener)
		{
			if (!OSHelper.DisableWeakEventListening)
			{
				CurrentManager?.ProtectedRemoveListener(source, listener);
			}
		}

		/// <inheritdoc/>
		protected sealed override void StartListening(object source)
		{
			if (!OSHelper.DisableWeakEventListening)
			{
				StartListeningTo((TEventSource)source);
			}
		}

		/// <inheritdoc/>
		protected sealed override void StopListening(object source)
		{
			if (!OSHelper.DisableWeakEventListening)
			{
				StopListeningTo((TEventSource)source);
			}
		}

		/// <summary>
		/// Attaches the event handler.
		/// </summary>
		protected abstract void StartListeningTo(TEventSource source);

		/// <summary>
		/// Detaches the event handler.
		/// </summary>
		protected abstract void StopListeningTo(TEventSource source);

		/// <summary>
		/// Gets the current manager
		/// </summary>
		private static TManager CurrentManager
		{
#if CLOUD
			get
			{
				return null;
			}
#else
			get
			{
				if (!OSHelper.DisableWeakEventListening)
				{
					return null;
				}

				var mType = typeof(TManager);
				var mgr = (TManager)GetCurrentManager(mType);
				if (mgr == null)
				{
					mgr = new TManager();
					SetCurrentManager(mType, mgr);
				}
				return mgr;
			}
#endif
		}
	}
}