using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public record StringIdentifier<T> : Identifier<T>
		where T : IIdeaObject
	{
		public string Id { get; }

		public StringIdentifier(string id)
		{
			Id = id;
		}

		public override string GetStringId() => typeof(T).FullName + "-" + Id;
	}
}