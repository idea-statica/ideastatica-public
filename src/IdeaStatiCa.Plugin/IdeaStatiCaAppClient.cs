#if NET48

using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Globalization;

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
		/// <summary>
		/// Get all cross-sections from IDEA StatiCa MPRL (material and product range library) which belongs to <paramref name="countryCode"/>
		/// </summary>
		/// <param name="countryCode">Country code filter</param>
		/// <returns>Cross-sections in the MPRL</returns>
		public List<LibraryItem> GetCssInMPRL(CountryCode countryCode)
		{
			return Service.GetCssInMPRL(countryCode);
		}
		/// <summary>
		/// Get all cross-sections in the currently open project
		/// </summary>
		/// <returns>Cross-sections in the project</returns>
		public List<ProjectItem> GetCssInProject()
		{
			return Service.GetCssInProject();
		}
		/// <summary>
		/// Get all cross-sections in the currently open project
		/// </summary>
		/// <returns>Cross-sections with assigned material in the project</returns>
		public List<CrossSectionProjectItem> GetCssInProjectV2()
		{
			return Service.GetCssInProjectV2();
		}
		/// <summary>
		/// Get all materials from IDEA StatiCa MPRL (material and product range library) which belongs to <paramref name="countryCode"/>
		/// </summary>
		/// <param name="countryCode">Country code filter</param>
		/// <returns>Materials in the MPRL</returns>
		public List<LibraryItem> GetMaterialsInMPRL(CountryCode countryCode)
		{
			return Service.GetMaterialsInMPRL(countryCode);
		}

		/// <summary>
		/// Get all materials in the currently open project
		/// </summary>
		/// <returns>Materials in the project</returns>
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

		public void SendMessage(MessageSeverity severity, string text)
		{
			throw new NotImplementedException();
			//Service.SendMessage(severity, text);
		}

		public void SetStage(int stage, int stageMax, string name)
		{
			throw new NotImplementedException();
		}

		public void SetStageProgress(double percentage)
		{
			throw new NotImplementedException();
		}

		public string GetLocalizedText(LocalisedMessage msg)
		{
			throw new NotImplementedException();
		}

		public CultureInfo GetCurrentCulture()
		{
			throw new NotImplementedException();
		}

		public int SendMessageInteractive(MessageSeverity severity, string text, string[] buttons)
		{
			throw new NotImplementedException();
		}

		public void CancelMessage()
		{
			throw new NotImplementedException();
		}

		protected IIdeaStaticaApp Service => base.Channel;

		public bool GetCancellationFlag()
		{
			throw new NotImplementedException();
		}

		public void InitProgressDialog()
		{
			throw new NotImplementedException();
		}

		public void SendMessage(MessageSeverity severity, LocalisedMessage msg)
		{
			throw new NotImplementedException();
		}

		public void SetStage(int stage, int stageMax, LocalisedMessage msg)
		{
			throw new NotImplementedException();
		}

		public void SendMessageLocalised(MessageSeverity severity, LocalisedMessage msg)
		{
			throw new NotImplementedException();
		}

		public void SetStageLocalised(int stage, int stageMax, LocalisedMessage msg)
		{
			throw new NotImplementedException();
		}
	}
}

#endif
