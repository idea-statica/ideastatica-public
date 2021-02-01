namespace IdeaStatiCa.Plugin
{
	public class IdeaStatiCaClient<T> : System.ServiceModel.ClientBase<T> where T : class
	{
		public IdeaStatiCaClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public T Service => base.Channel;
	}
}