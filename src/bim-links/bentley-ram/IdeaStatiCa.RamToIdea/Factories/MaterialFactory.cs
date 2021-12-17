//using RAMDATAACCESSLib;
//using IdeaStatiCa.RamToIdea.BimApi;
//using IdeaStatiCa.BimApi;
//using IdeaStatiCa.Plugin;
//using System;
//using System.Diagnostics;

//namespace IdeaStatiCa.RamToIdea.Factories
//{
//	/// <summary>
//	/// Factory class for <see cref="IIdeaMaterial"/>.
//	/// </summary>
//	internal class MaterialFactory
//	{
//		//TODO
//		//private readonly static IPluginLogger _logger = LoggerProvider.GetLogger("bim.rstab.factories");

//		/// <summary>
//		/// Creates an instance of <see cref="IIdeaMaterial"/> from a given <see cref="IMaterial"/>.
//		/// </summary>
//		/// <param name="objectFactory">IObjectFactory instance</param>
//		/// <param name="importSession"></param>
//		/// <returns>IIdeaMaterial instance</returns>
//		/// <param name="source">RSTAB material</param>

//		public IIdeaMaterial Create(IObjectFactory objectFactory, IImportSession importSession, RamMaterialWrapper source)
//		{
//			Debug.Assert(importSession.CountryCode != IdeaRS.OpenModel.CountryCode.None);

//			//Material materialData = source.GetData();
//			//Utils.ParseMaterialTextID(materialData.TextID, out _, out string type, out _);

//			EMATERIALTYPES type = source.Type;

//			try
//			{
//				switch (type)
//				{
//					case EMATERIALTYPES.ESteelMat:
//					case EMATERIALTYPES.ESteelJoistMat:
//						return new RamMaterialSteel(importSession.CountryCode, (RamSteelMaterialWrapper)source);

//					//case "CONCRETE":
//					//	return new RstabMaterialConcrete(importSession.CountryCode, source);

//					default:
//						throw new NotImplementedException($"Unsupported material type {type}");
//				}
//			}
//			catch (NotImplementedException e)
//			{
//				//_logger.LogError($"Unable to import material {materialData.No}", e);
//				// TODO: maybe send mesage to CCM/Checkbot or something
//				return new RamMaterialByName(MaterialType.Steel, source.Id, "S 235");
//			}
//		}
//	}
//}
