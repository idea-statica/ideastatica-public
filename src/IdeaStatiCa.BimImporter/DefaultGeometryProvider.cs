using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.BimImporter
{
	/// <summary>
	/// Default <see cref="IGeometry"/> provider. It uses <see cref="Geometry"/>.
	/// </summary>
	public class DefaultGeometryProvider : IGeometryProvider
	{
		private readonly IPluginLogger _logger;
		private readonly IIdeaModel _model;

		/// <summary>
		/// Creates instance of DefaultGeometryProvider.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="model">Model</param>
		/// <exception cref="ArgumentNullException">Throws if any argument is null.</exception>
		public DefaultGeometryProvider(IPluginLogger logger, IIdeaModel model)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_model = model ?? throw new ArgumentNullException(nameof(model));
		}

		///<inheritdoc cref="IGeometryProvider.GetGeometry"/>
		public IGeometry GetGeometry()
		{
			return new Geometry(_logger, _model);
		}
	}
}