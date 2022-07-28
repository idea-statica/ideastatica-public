using IdeaStatica.BimApiLink.Identifiers;
using IdeaStatiCa.BimApi;

namespace IdeaStatica.BimApiLink.BimApi
{
	public abstract class AbstractIdeaObject<T> : BimLinkObject, IIdeaObject
		where T : IIdeaObject
	{
		public string Id => Identifier.GetStringId();

		public virtual string Name { get; set; } = string.Empty;

		public Identifier<T> Identifier { get; }

		protected AbstractIdeaObject(Identifier<T> identifier)
		{
			Identifier = identifier;
		}
	}
}