﻿namespace IdeaStatiCa.IntermediateModel.IRModel
{
	// List intermediate object
	public class SList : ISIntermediate
	{
		public ICollection<ISIntermediate> Items { get; set; }
		public int Count { get { return Items.Count; } }

		public SList()
		{
			Items = new List<ISIntermediate>();
		}

		public SList(ISIntermediate item)
		{
			if (item == null)
			{
				Items = new List<ISIntermediate>();
			}
			else
			{
				Items = new List<ISIntermediate>() { item };
			}
		}

		public SList(params ISIntermediate[] items)
		{
			if (items == null)
			{
				Items = new List<ISIntermediate>();
			}
			else
			{
				Items = new List<ISIntermediate>(items);
			}
		}


		public void Add(ISIntermediate sIntermediateItem)
		{
			Items.Add(sIntermediateItem);
		}

		public ISIntermediate First()
		{
			return Items.First();
		}

		public IEnumerable<ISIntermediate> AsEnumerable()
		{
			return Items.AsEnumerable();
		}
	}
}
