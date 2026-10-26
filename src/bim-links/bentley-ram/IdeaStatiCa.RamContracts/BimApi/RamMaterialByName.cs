using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	public class RamMaterialByName : IIdeaMaterialByName
	{
		public MaterialType MaterialType { get; set; }

		///fix of duplicity materials PAV
		public string Id { get => $"material-named-{Name}"; }

		public string Name { get; set; }

	}
}