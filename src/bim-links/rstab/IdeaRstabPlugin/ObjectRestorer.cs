using IdeaRstabPlugin.Factories;
using IdeaRstabPlugin.Geometry;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using System;
using System.Collections.Generic;

namespace IdeaRstabPlugin
{
	/// <summary>
	/// Creates an object based on <see cref="PersistenceToken"/>.
	/// </summary>
	internal class ObjectRestorer : IObjectRestorer
	{
		public readonly IObjectFactory _objectFactory;
		private readonly ILinesAndNodes _linesAndNodes;

		public ObjectRestorer(IObjectFactory objectFactory, ILinesAndNodes linesAndNodes)
		{
			_objectFactory = objectFactory;
			_linesAndNodes = linesAndNodes;
		}

		/// <inheritdoc cref="IObjectRestorer.Restore(IIdeaPersistenceToken)"/>
		/// <exception cref="ArgumentException">Throws if <paramref name="token"/> is not
		/// instance of <see cref="PersistenceToken"/>.</exception>
		/// <exception cref="InvalidOperationException">Throws if token type is not Member or Node.</exception>
		public IIdeaPersistentObject Restore(IIdeaPersistenceToken token)
		{
			if (!(token is PersistenceToken persistenceToken))
			{
				throw new ArgumentException("Argument must be instance of " + nameof(PersistenceToken), nameof(token));
			}

			switch (persistenceToken.Type)
			{
				case TokenObjectType.Member:
					_linesAndNodes.UpdateMembers(new List<int>() { persistenceToken.No });
					return _objectFactory.GetMember(persistenceToken.No);

				case TokenObjectType.Node:
					return _objectFactory.GetNode(persistenceToken.No);

				default:
					throw new InvalidOperationException($"Unknown token type '{persistenceToken.Type}'");
			}
		}
	}
}