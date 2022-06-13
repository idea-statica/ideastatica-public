using IdeaStatiCa.CheckbotPlugin.Protos;
using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner.Utils;
using System.Reactive.Linq;
using Models = IdeaStatiCa.CheckbotPlugin.Models;

using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class EventService : IEventService
	{
		internal class EventNone : Models.Event
		{ }

		internal static readonly Models.Event None = new EventNone();

		private readonly Protos.EventService.EventServiceClient _client;
		private IObservable<Models.Event>? _events;

		public EventService(Protos.EventService.EventServiceClient client)
		{
			_client = client;
		}

		public void Subscribe(IEventListener eventListener)
		{
			GetObserable()
				.Subscribe(x => eventListener.OnEvent(x));
		}

		private IObservable<Models.Event> GetObserable()
		{
			if (_events is null)
			{
				SubscribeReq req = new();
				_events = _client.Subscribe(req).ResponseStream
					.AsObservable()
					.Select(x => TranslateEvent(x))
					.Where(x => x != None);
			}

			return _events;
		}

		private static Models.Event TranslateEvent(Event evt)
		{
			return evt.EventCase switch
			{
				Event.EventOneofCase.None => None,
				Event.EventOneofCase.PluginStop => new Models.EventPluginStop(),
				Event.EventOneofCase.OperationBegin => new Models.EventOperationBegin(),
				Event.EventOneofCase.OpenCheckApplication => new Models.EventOpenCheckApplication(evt.OpenCheckApplication.Object.FromProto()),
				_ => throw new NotImplementedException(),
			};
		}
	}
}