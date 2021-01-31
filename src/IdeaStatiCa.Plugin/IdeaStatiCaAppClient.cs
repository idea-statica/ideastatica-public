using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace IdeaStatiCa.Plugin
{
	public class IdeaStatiCaAppClient : System.ServiceModel.ClientBase<IIdeaStaticaApp>, IIdeaStaticaApp
	{
		public IdeaStatiCaAppClient(string id) : base(GetBinding(), GetAddress(id))
		{
		}

		private static System.ServiceModel.Channels.Binding GetBinding()
		{
			NetNamedPipeBinding binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 2147483647, OpenTimeout = TimeSpan.MaxValue, CloseTimeout = TimeSpan.MaxValue, ReceiveTimeout = TimeSpan.MaxValue, SendTimeout = TimeSpan.MaxValue };

			return binding;
		}

		private static System.ServiceModel.EndpointAddress GetAddress(string id)
		{
			return new EndpointAddress(string.Format(Constants.DefaultIdeaStaticaAutoUrlFormat, id));
		}

		public List<LibraryItem> GetCssInMPRL(CountryCode countryCode)
		{
			return Service.GetCssInMPRL(countryCode);
		}

		public List<ProjectItem> GetCssInProject()
		{
			return Service.GetCssInProject();
		}

		public List<LibraryItem> GetMaterialsInMPRL(CountryCode countryCode)
		{
			return Service.GetMaterialsInMPRL(countryCode);
		}

		public List<ProjectItem> GetMaterialsInProject()
		{
			return Service.GetMaterialsInProject();
		}

		public ConnectionData GetConnectionModel(int connectionId)
		{
			return Service.GetConnectionModel(connectionId);
		}

		public string GetAllConnectionData(int connectionId)
		{
			return Service.GetAllConnectionData(connectionId);
		}

		protected IIdeaStaticaApp Service => base.Channel;
	}
}