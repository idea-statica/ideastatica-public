using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public class IntIdentifier<T> : ImmutableIdentifier<T>
		where T : IIdeaObject
	{
		public int Id { get; }

		public IntIdentifier(int id)
			: base(typeof(T).FullName + "-" + id.ToString())
		{
			Id = id;
		}

		public override object GetId() => Id;
	}
}