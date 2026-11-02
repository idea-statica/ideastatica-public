using System;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConDesignSet
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string OwnerId { get; set; }
		public string Type { get; set; }
	}
}
