using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IdeaRS.GeometricItems
{
	[DataContract]
	public class Region
	{
		[DataMember]
		public List<Polyline> Openings { get; set; }

		[DataMember]
		public Polyline Outline { get; set; }

		public Region()
		{
			this.Outline = new Polyline();
		}

		public Region(Polyline outline)
		{
			this.Outline = outline;
		}

		public Region Clone()
		{
			var clone = new Region { Outline = this.Outline.Clone(), };
			if (this.Openings != null)
			{
				clone.Openings = new List<Polyline>(this.Openings.Count);
				foreach (var item in this.Openings)
				{
					clone.Openings.Add(item.Clone());
				}
			}

			return clone;
		}
	}
}