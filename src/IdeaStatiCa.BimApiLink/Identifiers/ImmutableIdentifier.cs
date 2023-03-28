﻿using IdeaStatiCa.BimApi;

namespace IdeaStatiCa.BimApiLink.Identifiers
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

		public override object GetId() => _stringId;
	}
}