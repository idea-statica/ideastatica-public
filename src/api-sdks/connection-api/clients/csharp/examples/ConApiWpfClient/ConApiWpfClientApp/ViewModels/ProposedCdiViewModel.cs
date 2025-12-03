using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.ViewModels
{
	public  class ProposedCdiViewModel : ViewModelBase
	{
		private readonly IdeaStatiCa.Api.Connection.Model.ConDesignItem _cdi;
		private readonly IConnectionApiClient _connectionApiClient;
		private bool hasDetails = false;
		private Guid _projectId;
		private int _connectionId;
		private string templateXml;

		public ProposedCdiViewModel(IConnectionApiClient connectionApiClient, Guid projectId, int connectionId, IdeaStatiCa.Api.Connection.Model.ConDesignItem cdi)
		{
			_cdi = cdi;
			_connectionApiClient = connectionApiClient;
			_projectId = projectId;
			_connectionId = connectionId;
		}

		public Guid ConDesignItemId
		{
			get { return _cdi.ConDesignItemId; }
		}

		public string? Name
		{
			get { return _cdi.Name; }
		}

		public async Task<bool> InitDetailsAsync()
		{
			if(hasDetails)
			{
				return hasDetails;
			}

			var templateBase64 = await _connectionApiClient.ConnectionLibrary.GetTemplateAsync(_cdi.ConDesignSetId, _cdi.ConDesignItemId, 0);
			TemplateXml = Encoding.UTF8.GetString(Convert.FromBase64String(templateBase64));
			//try
			//{
			//	PictureData = await _connectionApiClient.ConnectionLibrary.GetDesignItemPictureDataAsync(_cdi.PictureId);
				
			//}
			//catch(Exception e)
			//{

			//}

			hasDetails = true;
			return hasDetails;
		}


		public ConDesignItem Cdi => _cdi;

		public string TemplateXml { get => templateXml; set => templateXml = value; }

		public byte[] PictureData { get; set; }
	}
}
