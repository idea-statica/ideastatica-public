using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.BimImporter
{
	/// <inheritdoc cref="IProject"/>
	/// <remarks>This class is not thread-safe.</remarks>
	public class Project : IProject
	{
		/// <inheritdoc cref="IProject.IdMapping"/>
		public ConversionDictionaryString IdMapping { get; private set; } = new ConversionDictionaryString();

		private Dictionary<int, IIdeaObject> _iomIdToBimObject = new Dictionary<int, IIdeaObject>();
		private readonly IPluginLogger _logger;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="logger"/> is null.</exception>
		public Project(IPluginLogger logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc cref="IProject.GetIomId(string)"/>
		public int GetIomId(string bimId)
		{
			return IdMapping[bimId];
		}

		/// <inheritdoc cref="IProject.GetIomId(IIdeaObject)"/>
		/// <exception cref="ArgumentNullException">Throws if <paramref name="obj"/> is null.</exception>
		public int GetIomId(IIdeaObject obj)
		{
			if (obj is null)
			{
				throw new ArgumentNullException(nameof(obj));
			}

			string bimId = obj.Id;

			if (IdMapping.TryGetValue(bimId, out int iomId))
			{
				return iomId;
			}

			iomId = IdMapping.MapId(bimId);
			_iomIdToBimObject.Add(iomId, obj);

			_logger.LogDebug($"Creating new id mapping: BimApi id {bimId}, IOM id {iomId}");

			return iomId;
		}

		/// <inheritdoc cref="IProject.GetBimObject(int)"/>
		public IIdeaObject GetBimObject(int id)
		{
			return _iomIdToBimObject[id];
		}

		/// <inheritdoc cref="IProject.Load(IGeometry, ConversionDictionaryString)"/>
		/// <exception cref="ArgumentNullException">If some argument is null.</exception>
		public void Load(IGeometry geometry, ConversionDictionaryString conversionTable)
		{
			if (geometry is null)
			{
				throw new ArgumentNullException(nameof(geometry));
			}

			if (conversionTable is null)
			{
				throw new ArgumentNullException(nameof(conversionTable));
			}

			IEnumerable<IIdeaObject> objects = geometry.GetMembers().OfType<IIdeaObject>().Concat(geometry.GetNodes());
			Dictionary<int, IIdeaObject> iom2BimObj = new Dictionary<int, IIdeaObject>();

			foreach (IIdeaObject obj in objects)
			{
				string bimId = obj.Id;
				if (conversionTable.TryGetValue(bimId, out int iomId))
				{
					iom2BimObj.Add(iomId, obj);
					_logger.LogDebug($"Loaded id mapping: BimApi id {bimId}, IOM id {iomId}");
				}
			}

			IdMapping = conversionTable;
			_iomIdToBimObject = iom2BimObj; // ensures that _iomIdToBimObject is in a valid state
		}
	}
}