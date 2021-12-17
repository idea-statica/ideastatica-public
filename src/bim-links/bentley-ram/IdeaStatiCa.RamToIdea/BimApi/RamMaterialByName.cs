using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamMaterialByName : IIdeaMaterialByName
	{
		public MaterialType MaterialType { get; set; }

		public string Id { get; }

		public string Name { get; set; }

		public RamMaterialByName(int uid)
		{
			Id = $"material-named-{uid}";
		}
	}
}