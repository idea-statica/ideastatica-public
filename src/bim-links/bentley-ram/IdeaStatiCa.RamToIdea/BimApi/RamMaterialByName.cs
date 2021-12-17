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
		public MaterialType MaterialType { get; set; }

		public string Id { get; set; }

		public string Name { get; set; }
	}
}
