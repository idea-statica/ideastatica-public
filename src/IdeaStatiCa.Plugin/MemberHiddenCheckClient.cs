#if NET48

using System.Threading;

namespace IdeaStatiCa.Plugin
{
	public class MemberHiddenCheckClient : System.ServiceModel.ClientBase<IMemberHiddenCheck>, IMemberHiddenCheck
	{
		public static int HiddenCalculatorId { get; set; }

		static MemberHiddenCheckClient()
		{
			HiddenCalculatorId = -1;
		}

		public MemberHiddenCheckClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public void OpenProject(string projectLocation)
		{
			Service.OpenProject(projectLocation);
		}

		public void CloseProject()
		{
			Service.CloseProject();
		}

		public string Calculate(int subStructureId)
		{
			return Service.Calculate(subStructureId);
		}

		public void Cancel()
		{
			if (HiddenCalculatorId < 0)
			{
				return;
			}

			EventWaitHandle syncEvent;
			var cancelEventName = string.Format(Constants.MemHiddenCalcCancelEventFormat, HiddenCalculatorId);
			if (EventWaitHandle.TryOpenExisting(cancelEventName, out syncEvent))
			{
				syncEvent.Set();
				syncEvent.Dispose();
			}
		}

		protected IMemberHiddenCheck Service => base.Channel;
	}
}

#endif