#include "pch.h"
#include "MaterialImporter.h"

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		MaterialImporter::MaterialImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IIdeaMaterial^ MaterialImporter::Create(int id)
		{
			IdeaMaterialByName^ mat = gcnew IdeaMaterialByName(id);
			mat->Name = "S 355";
			mat->MaterialType = MaterialType::Steel;

			return mat;
		}
	}
}