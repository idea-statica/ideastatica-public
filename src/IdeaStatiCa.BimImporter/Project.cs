using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	/// <inheritdoc cref="IProject"/>
	/// <remarks>This class is not thread-safe.</remarks>
	public class Project : ProjectNoCache
	{

		/// <summary>
		/// Creates an instance of Project.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="persistence">Instance of IPersistence for storing of id mapping.</param>
		/// <param name="objectRestorer">Object restorer</param>
		/// <exception cref="ArgumentNullException">Throws if any argument is null.</exception>
		public Project(IPluginLogger logger, IPersistence persistence, IObjectRestorer objectRestorer) : base(logger, persistence, objectRestorer)
		{
		}

		/// <summary>
		/// Creates an instance of Project.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="persistence">Instance of IPersistence for storing of id mapping.</param>
		/// <exception cref="ArgumentNullException">Throws if any argument is null.</exception>
		public Project(IPluginLogger logger, IPersistence persistence) : base(logger, persistence)
		{
		}


		/// <inheritdoc cref="IProject.GetBimObject(int)"/>
		public override IIdeaObject GetBimObject(int id)
		{
			if (_objectMapping.TryGetValue(id, out IIdeaObject obj))
			{
				return obj;
			}

			if (!(_objectRestorer is null))
			{
				if (_persistenceTokens.TryGetValue(id, out IIdeaPersistenceToken token))
				{
					obj = _objectRestorer.Restore(token);
					_objectMapping.Add(id, obj);
					return obj;
				}
			}

			throw new KeyNotFoundException();
		}
	}

}