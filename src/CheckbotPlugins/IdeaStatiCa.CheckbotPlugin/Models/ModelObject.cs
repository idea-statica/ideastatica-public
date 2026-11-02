namespace IdeaStatiCa.CheckbotPlugin.Models
{
	public class ModelObject
	{
		public ModelObjectType Type { get; }

		public int Id { get; }

		public ModelObject(ModelObjectType type, int id)
		{
			Type = type;
			Id = id;
		}
	}
}