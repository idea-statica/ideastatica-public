using RamDataAcc1 = RAMDATAACCESSLib.RamDataAccess1;
using IModel = RAMDATAACCESSLib.IModel;
using RamDataAccIDBIO = RAMDATAACCESSLib.IDBIO1;
using IMemberData = RAMDATAACCESSLib.IMemberData1;

using IdeaStatiCa.RamToIdea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea.Utilities
{
	internal static class RamSectionRetriever
	{
		internal static RamSteelSectionWrapper GetSteelSectionProp(RamDataAcc1 RamDataAccess, IMember member)
		{
			RamSteelSectionWrapper section = new RamSteelSectionWrapper();

			IMemberData MemberData = (IMemberData)RamDataAccess.GetDispInterfacePointerByEnum(RAMDATAACCESSLib.EINTERFACES.IMemberData_INT);

			MemberData.GetSteelMemberSectionDimProps(member., ref section.SteelSectionShape, ref section.SectionLabel, ref section.BfTop, ref section.BfBot,
				section.TfTop, section.TfBot, 1, 1, section.Depth, section.Tw, 1, 1, section.RolledFlag, 1, 1, 1, 1, 1, 1, 1, 1);

			return section;
		}

		//	Dim SectionProps As Props


		//	Set IMemberData1 = RamDataAcc1.GetDispInterfacePointerByEnum(IMemberData_INT)

		//	IMemberData1.GetSteelMemberSectionDimProps lMemberID, peShape, 1, pdBfTop, pdBfBot, pdTfTop, pdTfBot, 1, 1, pdDepth, pdWebT, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, pdArea

		//	If peShape = EStlWF Then
		//		SectionProps.dArea = pdArea
		//		SectionProps.dPerimeter = 2 * pdBfTop + 2 * pdDepth + 2 * pdBfBot - 2 * pdWebT

		//		SectionProps.dBfTop = pdBfTop

		//		SectionProps.strShape = "ISection"

		//	End If

		//	If peShape = EStlChannel Then
		//		SectionProps.dArea = pdArea

		//		SectionProps.dPerimeter = 2 * pdBfTop + 2 * pdDepth + 2 * pdBfBot - 2 * pdWebT

		//		SectionProps.dBfTop = pdBfTop

		//		SectionProps.strShape = "Channel"

		//	End If

		//	If peShape = EStlTube Then
		//		SectionProps.dArea = pdArea

		//		SectionProps.dPerimeter = 2 * pdBfTop + 2 * pdDepth

		//		SectionProps.dBfTop = pdBfTop

		//		SectionProps.strShape = "Tube"

		//	End If

		//	If peShape = EStlDoubleL Then
		//		SectionProps.dArea = 2 * pdArea

		//		SectionProps.dPerimeter = 4 * pdDepth + 4 * pdWebT

		//		SectionProps.dBfTop = 0

		//		SectionProps.strShape = "DoubleAngle"

		//	End If

		//	If peShape = EStlFlatBar Then
		//		SectionProps.dArea = pdArea

		//		SectionProps.dPerimeter = pdBfTop * 2 + pdTfTop * 2

		//		SectionProps.dBfTop = 0

		//		SectionProps.strShape = "Bar"

		//	End If

		//	If peShape = EStlLSection Then
		//		SectionProps.dArea = pdArea

		//		SectionProps.dPerimeter = 2 * pdDepth + 2 * pdWebT

		//		SectionProps.dBfTop = 0

		//		SectionProps.strShape = "Angle"

		//	End If

		//	If peShape = EStlNone Then
		//		SectionProps.dArea = 0

		//		SectionProps.dPerimeter = 0

		//		SectionProps.dBfTop = 0

		//		SectionProps.strShape = "NA"

		//	End If

		//	If peShape = EStlPipe Then
		//		SectionProps.dArea = pdArea

		//		SectionProps.dPerimeter = pdDepth * 3.14159

		//		SectionProps.dBfTop = 0

		//		SectionProps.strShape = "Pipe"

		//	End If

		//	If peShape = EstlRoundBar Then
		//		SectionProps.dArea = pdArea

		//		SectionProps.dPerimeter = pdDepth * 3.14159

		//		SectionProps.dBfTop = 0

		//		SectionProps.strShape = "Rod"

		//	End If

		//	If peShape = EStlTSection Then
		//		SectionProps.dArea = pdArea

		//		SectionProps.dPerimeter = 2 * pdBfTop + 2 * pdDepth

		//		SectionProps.dBfTop = 0

		//		SectionProps.strShape = "Tee"

		//	End If


		//	GetSectionProps = SectionProps

		//End Function




	}
}
