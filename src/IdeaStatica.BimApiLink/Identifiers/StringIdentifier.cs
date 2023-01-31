using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public class StringIdentifier<T> : ImmutableIdentifier<T>
		where T : IIdeaObject
	{
		public string Id { get; }

		public StringIdentifier(string id)
			: base(typeof(T).FullName + "-" + id)
		{
			Id = id;
		}

		public override object GetId() => Id;
	}
}