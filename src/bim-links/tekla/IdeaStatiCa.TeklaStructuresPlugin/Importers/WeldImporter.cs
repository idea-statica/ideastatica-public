using IdeaRS.OpenModel.Connection;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using System;
using System.Collections.Generic;
using TSM = Tekla.Structures.Model;
using TSV = Tekla.Structures.TeklaStructuresSettings;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class WeldImporter : BaseImporter<IIdeaWeld>
	{
		internal const double vectorCompareTollerance = 1E-04;

		public WeldImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaWeld Create(string id)
		{
			PlugInLogger.LogInformation($"WeldImporter create {id}");
			var item = Model.GetItemByHandler(id);
			if (item == null)
			{
				PlugInLogger.LogInformation($"WeldImporter not found weld {id}");
				return null;
			}

			if (item is TSM.BaseWeld teklaWeld)
			{
				var weld = new BimApi.Weld(teklaWeld.Identifier.GUID.ToString());

				bool aiscWeldMark = false;
				bool IsoLegLengthAsWeldsize = false;
				TSV.GetAdvancedOption("XS_AISC_WELD_MARK", ref aiscWeldMark);
				TSV.GetAdvancedOption("XS_ISO_LEG_LENGTH_AS_WELDSIZE", ref IsoLegLengthAsWeldsize);

				var cs = teklaWeld.GetCoordinateSystem();

				weld.StartNo = Model.GetPointId(cs.Origin);
				weld.EndNo = Model.GetPointId(cs.Origin);
				//This stupid test due to plate as member and we are not sure if its imported as plate or member

				CheckAndAddConnectedObject<IIdeaPlate>(teklaWeld.MainObject, weld);
				CheckAndAddConnectedObject<IIdeaMember1D>(teklaWeld.MainObject, weld);

				CheckAndAddConnectedObject<IIdeaPlate>(teklaWeld.SecondaryObject, weld);
				CheckAndAddConnectedObject<IIdeaMember1D>(teklaWeld.SecondaryObject, weld);

				// WELD TYPE
				WeldType weldTypeCode;
				if (teklaWeld.AroundWeld || (teklaWeld.TypeAbove != TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_NONE && teklaWeld.TypeBelow != TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_NONE))
				{
					weldTypeCode = WeldType.DoubleFillet;
				}
				else
				{
					TSM.BaseWeld.WeldTypeEnum teklaWeldType;
					if (teklaWeld.TypeAbove != TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_NONE)
					{
						teklaWeldType = teklaWeld.TypeAbove;
					}
					else
					{
						teklaWeldType = teklaWeld.TypeBelow;
					}

					if (teklaWeldType == TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_NONE)
					{
						return null;
					}

					switch (teklaWeldType)
					{
						case TSM.BaseWeld.WeldTypeEnum.STEEP_FLANKED_BEVEL_GROOVE_SINGLE_BEVEL_BUTT:
						case TSM.BaseWeld.WeldTypeEnum.STEEP_FLANKED_BEVEL_GROOVE_SINGLE_V_BUTT:
						case TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_BEVEL_BACKING:
						case TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_BEVEL_GROOVE_SINGLE_BEVEL_BUTT:
						case TSM.BaseWeld.WeldTypeEnum.WELD_TYPE_BEVEL_GROOVE_SINGLE_V_BUTT:
							weldTypeCode = WeldType.Bevel;
							break;

						default:
							weldTypeCode = WeldType.Fillet;
							break;
					}
				}

				double weldSize = Math.Max((double)teklaWeld.SizeAbove, (double)teklaWeld.SizeBelow);
				if (aiscWeldMark || ((!aiscWeldMark) && IsoLegLengthAsWeldsize))
				{
					weldSize /= Math.Sqrt(2);
				}

				weld.Thickness = weldSize.MilimetersToMeters();
				weld.WeldType = weldTypeCode;

				return weld;
			}
			else
			{
				PlugInLogger.LogInformation($"WeldImporter unknown weld {item.GetType()}");
				return null;
			}
		}

		private void CheckAndAddConnectedObject<T>(TSM.ModelObject part, BimApi.Weld weld)
			where T : IIdeaObjectConnectable
		{
			IIdeaObject ideaObject = CheckMaybe<T>(part.Identifier.GUID.ToString());
			if (ideaObject != null)
			{

				IIdeaObjectConnectable mainObject = GetMaybe<T>(part.Identifier.GUID.ToString());
				if (mainObject != null)
				{
					(weld.ConnectedParts as List<IIdeaObjectConnectable>)?.Add(mainObject);
				}
			}
		}
	}
}
