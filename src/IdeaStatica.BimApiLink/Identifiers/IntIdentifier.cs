using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public class IntIdentifier<T> : Identifier<T>
		where T : IIdeaObject
	{
		public int Id { get; }

		public IntIdentifier(int id)
		{
			Id = id;
		}

		public override string GetStringId() => typeof(T).FullName + "-" + Id.ToString();
	}
}