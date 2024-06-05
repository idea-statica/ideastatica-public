#include "pch.h"
#include "CrossSectionImporter.h"
#include "CrossSectionByName.h"


namespace CppFeaApiWrapper
{
	namespace Importers
	{
		CrossSectionImporter::CrossSectionImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IIdeaCrossSection^ CrossSectionImporter::Create(int id)
		{
			CppFeaApiWrapper::BimApi::CrossSectionByName^ css = gcnew CppFeaApiWrapper::BimApi::CrossSectionByName(id);
			css->MaterialNo = 1;
			css->Name = "IPE200";

			return css;
		}
	}
}