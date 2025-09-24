﻿using IdeaRS.OpenModel.Model;
using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApi.Results;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.RamToIdea.BimApi
{
	internal class RamElement1D : IIdeaElement1D
	{
		public IIdeaSegment3D Segment { get; set; }

		public string Id => $"element-{Segment.Id}";

		public string Name { get; }

		public IIdeaCrossSection StartCrossSection { get; set; }

		public IIdeaCrossSection EndCrossSection { get; set; }

		public IdeaVector3D EccentricityBegin { get; set; } = new IdeaVector3D(0, 0, 0);

		public IdeaVector3D EccentricityEnd { get; set; } = new IdeaVector3D(0, 0, 0);

		public CardinalPoints CardinalPoint { get; set; }

		public EccentricityReference EccentricityReference { get; set; }

		public double RotationRx { get; set; }

		public IEnumerable<IIdeaResult> GetResults()
		{
			return Enumerable.Empty<IIdeaResult>();
		}
	}
}