using System;
using System.Threading.Tasks;
using IdeaStatiCa.CheckbotPlugin.Models;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public static class EventServiceExtensions
	{
		public static void Subscribe(this IEventService eventService, Action<Event> handler)
			=> eventService.Subscribe(new EventListener(handler));

		public static void Subscribe(this IEventService eventService, IAsyncEventListener asyncListener)
			=> eventService.Subscribe(new AsyncEventListenerAdapter(asyncListener.OnEvent));

		public static void Subscribe(this IEventService eventService, Func<Event, Task> handler)
			=> eventService.Subscribe(new AsyncEventListenerAdapter(handler));

		internal class EventListener : IEventListener
		{
			private readonly Action<Event> _handler;

			public EventListener(Action<Event> handler)
			{
				_handler = handler;
			}

			public void OnEvent(Event e) => _handler(e);
		}

		internal class AsyncEventListenerAdapter : IEventListener
		{
			private readonly Func<Event, Task> _handler;

			public AsyncEventListenerAdapter(Func<Event, Task> handler)
			{
				_handler = handler;
			}

			public void OnEvent(Event e)
			{
				Task.Run(() => _handler(e));
			}
		}
	}
}