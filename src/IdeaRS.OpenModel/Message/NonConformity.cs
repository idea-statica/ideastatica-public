using System;

namespace IdeaRS.OpenModel.Message
{
	/// <summary>
	/// Non-conformity
	/// </summary>
	public class NonConformity
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public NonConformity()
		{
			Guid = Guid.Empty;
			Severity = NonConformitySeverity.Info;
		}

		/// <summary>
		/// Description of the nonconformity
		/// </summary>
		public Guid Guid { get; set; }

		/// <summary>
		/// severity of this nonconformity
		/// </summary>
		public NonConformitySeverity Severity { get; set; }
	}
}