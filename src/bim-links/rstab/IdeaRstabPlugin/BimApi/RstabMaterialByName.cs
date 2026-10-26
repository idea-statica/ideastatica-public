using IdeaStatiCa.BimApi;

namespace IdeaRstabPlugin.BimApi
{
	/// <inheritdoc cref="IIdeaMaterialByName"/>
	internal class RstabMaterialByName : IIdeaMaterialByName
	{
		public MaterialType MaterialType { get; }

		public string Id { get; }

		public string Name { get; }

		public RstabMaterialByName(MaterialType materialType, int no, string name)
		{
			MaterialType = materialType;
			Id = $"material-by-name-{no}";
			Name = name;
		}
	}
}