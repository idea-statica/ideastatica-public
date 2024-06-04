#include "pch.h"
#include "MaterialImporter.h"

namespace CppFeaApiWrapper
{
	namespace Importers
	{
		MatrialImporter::MatrialImporter(ImporterContext^ context)
		{
			this->context = context;
		}

		IIdeaMaterial^ MatrialImporter::Create(int id)
		{
			IdeaMaterialByName^ mat = gcnew IdeaMaterialByName(id);
			mat->Name = "S 355";
			mat->MaterialType = MaterialType::Steel;

			return mat;
		}
	}
}