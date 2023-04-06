using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.Identifiers
{
	public class StringIdentifier<T> : ImmutableIdentifier<T>
		where T : IIdeaObject
	{
		public string Id { get; }

		public StringIdentifier(string id)
			: this(id, typeof(T).FullName)
		{
		}

		protected StringIdentifier(string id, string typeName)
			: base($"{typeName}-{id}")
		{
			Id = id;
		}

		public override object GetId() => Id;
	}
}