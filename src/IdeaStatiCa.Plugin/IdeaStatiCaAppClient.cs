using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;

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

		public List<CrossSectionProjectItem> GetCssInProjectV2()
		{
			return Service.GetCssInProjectV2();
		}

		public List<LibraryItem> GetMaterialsInMPRL(CountryCode countryCode)
		{
			return Service.GetMaterialsInMPRL(countryCode);
		}

		public List<ProjectItem> GetMaterialsInProject()
		{
			return Service.GetMaterialsInProject();
		}

		/// <inheritdoc cref="IIdeaStaticaApp.GetConnectionModel(int)"/>
		public ConnectionData GetConnectionModel(int connectionId)
		{
			// get OpenModelContainer in XML format
			string completeModelXml = GetAllConnectionData(connectionId);
			var completeModel = Tools.OpenModelContainerFromXml(completeModelXml);

			var iom = completeModel?.OpenModel;
			if (iom == null)
			{
				throw new Exception("GetConnectionModel - not valid IOM is provided by IdeaStatiCa");
			}

			ConnectionData conData = iom?.Connections.FirstOrDefault();
			if (conData == null)
			{
				throw new Exception("GetConnectionModel - no geometrical data of the requested connection is provided by IdeaStatiCa");
			}

			return conData;
		}

		/// <inheritdoc cref="IIdeaStaticaApp.GetAllConnectionData(int)"/>
		public string GetAllConnectionData(int connectionId)
		{
			return Service.GetAllConnectionData(connectionId);
		}

		protected IIdeaStaticaApp Service => base.Channel;
	}
}