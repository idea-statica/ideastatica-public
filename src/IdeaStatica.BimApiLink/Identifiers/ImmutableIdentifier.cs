using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.Identifiers
{
	public abstract class ImmutableIdentifier<T> : Identifier<T>
		where T : IIdeaObject
	{
		private readonly string _stringId;

		protected ImmutableIdentifier(string stringId)
		{
			_stringId = stringId;
		}

		public override string GetStringId() => _stringId;
	}
}