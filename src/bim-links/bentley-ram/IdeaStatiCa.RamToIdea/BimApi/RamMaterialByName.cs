using IdeaStatiCa.BimApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMaterialByName : IIdeaMaterialByName
	{
		public MaterialType MaterialType { get; }

		public string Id { get; }

		public string Name { get; }

		public RamMaterialByName(MaterialType materialType, int no, string name)
		{
			MaterialType = materialType;
			Id = $"material-by-name-{no}";
			Name = name;
		}
	}
}
