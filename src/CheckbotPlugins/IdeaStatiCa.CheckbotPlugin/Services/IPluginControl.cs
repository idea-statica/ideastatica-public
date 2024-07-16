using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public enum StopReason
	{
		Event,
		Call
	}

	public interface IPluginControl
	{
		event Action<StopReason> StopInvoked;

		IPluginControlBehaviour Behaviour { get; set; }

		Task AnnounceReady();

		void Stop();
	}
}