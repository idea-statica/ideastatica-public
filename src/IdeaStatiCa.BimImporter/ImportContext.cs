using IdeaRS.OpenModel;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.BimImporter
{
	internal class ImportContext
	{
		public Dictionary<string, ReferenceElement> ReferenceElements { get; } = new Dictionary<string, ReferenceElement>();

		public OpenModel OpenModel { get; } = new OpenModel();

		public void Add(OpenElementId openElementId)
		{
			int result = OpenModel.AddObject(openElementId);
			if (result != 0)
			{
				throw new Exception();
			}
		}
	}
}