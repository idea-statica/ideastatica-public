using AutoMapper;
using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner.Utils;
using System.Reactive.Linq;
using Models = IdeaStatiCa.CheckbotPlugin.Models;

using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class EventService : IEventService
	{
		private static Mapper _mapper = Mapping.GetMapper();

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
				Protos.SubscribeReq req = new();
				_events = _client.Subscribe(req).ResponseStream
					.AsObservable()
					.Select(x => _mapper.Map<Models.Event>(x));
			}

			return _events;
		}
	}
}