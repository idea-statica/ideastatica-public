using System;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConDesignItem
	{
		public string Version { get; set; }

		public string Name { get; set; }

		public string DesignCode { get; set; }

		public Guid ConDesignSetId { get; set; }

		public Guid ConDesignItemId { get; set; }
	}
}