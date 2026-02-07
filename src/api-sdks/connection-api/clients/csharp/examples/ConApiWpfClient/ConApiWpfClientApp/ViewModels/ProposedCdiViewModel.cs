using IdeaStatiCa.Api.Connection.Model;
using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.Plugin;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ConApiWpfClientApp.ViewModels
{
	/// <summary>
	/// View model representing a proposed connection design item from the connection library.
	/// Lazily loads detail data (template XML and picture) on first access.
	/// </summary>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="ProposedCdiViewModel"/> class.
		/// </summary>
		/// <param name="connectionApiClient">The API client for fetching template and picture data.</param>
		/// <param name="projectId">The unique identifier of the project.</param>
		/// <param name="connectionId">The identifier of the connection.</param>
		/// <param name="cdi">The connection design item model.</param>
		/// <param name="logger">The logger for diagnostic output.</param>
		public ProposedCdiViewModel(IConnectionApiClient connectionApiClient, Guid projectId, int connectionId, IdeaStatiCa.Api.Connection.Model.ConDesignItem cdi, IPluginLogger logger)
		{
			_cdi = cdi;
			_connectionApiClient = connectionApiClient;
			_projectId = projectId;
			_connectionId = connectionId;
			_logger = logger;
		}

		/// <summary>
		/// Gets the unique identifier of the connection design item.
		/// </summary>
		public Guid ConDesignItemId
		{
			get { return _cdi.ConDesignItemId; }
		}

		/// <summary>
		/// Gets the unique identifier of the design set this item belongs to.
		/// </summary>
		public Guid ConDesignSetId
		{
			get { return _cdi.ConDesignSetId; }
		}

		/// <summary>
		/// Gets the display name of the design item.
		/// </summary>
		public string? Name
		{
			get { return _cdi.Name; }
		}

		/// <summary>
		/// Gets the design code associated with this item (e.g., "EC", "AISC").
		/// </summary>
		public string? DesignCode
		{
			get { return _cdi.DesignCode; }
		}

		/// <summary>
		/// Loads the template XML and picture data for this design item from the API.
		/// Results are cached after the first successful load.
		/// </summary>
		/// <returns><see langword="true"/> if details have been loaded (either now or previously).</returns>
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

		/// <summary>
		/// Gets the underlying connection design item model.
		/// </summary>
		public ConDesignItem Cdi => _cdi;

		/// <summary>
		/// Gets or sets the connection template XML content.
		/// </summary>
		public string? TemplateXml { get => templateXml; set => SetProperty(ref templateXml, value); }

		/// <summary>
		/// Gets or sets the raw picture data (image bytes) for this design item.
		/// </summary>
		public byte[]? PictureData { get => pictureData; set => SetProperty (ref pictureData, value); }
	}
}
