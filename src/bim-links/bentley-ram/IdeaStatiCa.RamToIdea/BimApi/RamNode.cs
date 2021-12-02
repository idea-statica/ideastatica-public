//using Dlubal.RSTAB8;
//using IdeaRstabPlugin.Providers;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimImporter.Persistence;
using IdeaStatiCa.Plugin;
using System;

namespace IdeaStatiCa.RamToIdea
{
	internal class RamNode : IIdeaNode
	{
		public IdeaVector3D Vector => throw new NotImplementedException();

		public IIdeaPersistenceToken Token => throw new NotImplementedException();

		public string Id => throw new NotImplementedException();

		public string Name => throw new NotImplementedException();
	}
}
