using Dlubal.RSTAB8;
using IdeaRstabPlugin.BimApi;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.PluginLogger;
using System;
using System.Diagnostics;

namespace IdeaRstabPlugin.Factories
{
	/// <summary>
	/// Factory class for <see cref="IIdeaMaterial"/>.
	/// </summary>
	internal class MaterialFactory : IFactory<IMaterial, IIdeaMaterial>
	{
		private static readonly IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.factories");

		/// <summary>
		/// Creates an instance of <see cref="IIdeaMaterial"/> from a given <see cref="IMaterial"/>.
		/// </summary>
		/// <param name="objectFactory">IObjectFactory instance</param>
		/// <param name="importSession"></param>
		/// <returns>IIdeaMaterial instance</returns>
		/// <param name="source">RSTAB material</param>

		public IIdeaMaterial Create(IObjectFactory objectFactory, IImportSession importSession, IMaterial source)
		{
			Debug.Assert(importSession.CountryCode != IdeaRS.OpenModel.CountryCode.None);

			Material materialData = source.GetData();
			Utils.ParseMaterialTextID(materialData.TextID, out _, out string type, out _);

			bool isConcrete = type == "CONCRETE";

			// if we can't get the material database there's no point in trying to import the
			// material by parameters
			if (source.GetDatabaseMaterial() is null)
			{
				return CreateNamedMaterial(isConcrete, materialData);
			}

			try
			{
				if (isConcrete)
				{
					return new RstabMaterialConcrete(importSession.CountryCode, source);
				}
				else
				{
					return new RstabMaterialSteel(importSession.CountryCode, source);
				}
			}
			catch (Exception e)
			{
				_logger.LogError($"Unable to import material {materialData.No}", e);
				return CreateNamedMaterial(isConcrete, materialData);
			}
		}

		private IIdeaMaterial CreateNamedMaterial(bool isConcrete, Material material)
		{
			return new RstabMaterialByName(isConcrete ? MaterialType.Concrete : MaterialType.Steel, material.No, material.Description);
		}
	}
}