using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public class IntIdentifier<T> : ImmutableIdentifier<T>
		where T : IIdeaObject
	{
		public int Id { get; }

		public IntIdentifier(int id)
			: this(id, typeof(T).FullName)
		{
		}

		protected IntIdentifier(int id, string typeName)
			: base($"{typeName}-{id}")
		{
			Id = id;
		}
	}
}