using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.ViewModels
{
	public  class ProposedCdiViewModel : ViewModelBase
	{
		private readonly IdeaStatiCa.Api.Connection.Model.ConDesignItem _cdi;
		private readonly IConnectionApiClient _connectionApiClient;
		private readonly IPluginLogger _logger;
		private bool hasDetails = false;
		private Guid _projectId;
		private int _connectionId;
		private string? templateXml;
		private byte[]? pictureData;

		public ProposedCdiViewModel(IConnectionApiClient connectionApiClient, Guid projectId, int connectionId, IdeaStatiCa.Api.Connection.Model.ConDesignItem cdi, IPluginLogger logger)
		{
			_cdi = cdi;
			_connectionApiClient = connectionApiClient;
			_projectId = projectId;
			_connectionId = connectionId;
			_logger = logger;
		}

		public Guid ConDesignItemId
		{
			get { return _cdi.ConDesignItemId; }
		}

		public Guid ConDesignSetId
		{
			get { return _cdi.ConDesignSetId; }
		}

		public string? Name
		{
			get { return _cdi.Name; }
		}

		public string? DesignCode
		{
			get { return _cdi.DesignCode; }
		}

		public async Task<bool> InitDetailsAsync()
		{
			if(hasDetails)
			{
				return hasDetails;
			}

			var templateBase64 = await _connectionApiClient.ConnectionLibrary.GetTemplateAsync(_cdi.ConDesignSetId, _cdi.ConDesignItemId, 0);
			TemplateXml = Encoding.UTF8.GetString(Convert.FromBase64String(templateBase64));

			try
			{
				PictureData = await _connectionApiClient.ConnectionLibrary.GetDesignItemPictureDataAsync(_cdi.ConDesignSetId, _cdi.ConDesignItemId);

			}
			catch (Exception e)
			{
				_logger.LogInformation("ProposedCdiViewModel.InitDetailsAsync  : GetDesignItemPictureDataAsync FAILED", e);
			}

			hasDetails = true;
			return hasDetails;
		}


		public ConDesignItem Cdi => _cdi;

		public string? TemplateXml { get => templateXml; set => SetProperty(ref templateXml, value); }

		public byte[]? PictureData { get => pictureData; set => SetProperty (ref pictureData, value); }
	}
}
