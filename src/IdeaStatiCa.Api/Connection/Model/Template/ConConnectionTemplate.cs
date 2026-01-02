using IdeaStatiCa.Api.Connection.Model.Project;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConConnectionTemplate
	{
		/// <summary>
		/// Represents the unique identifier of the template
		/// If the same template is used in multiple times, the same Guid is used.
		/// </summary>
		public Guid LibraryTemplateId { get; set; }

		/// <summary>
		/// Unique identifier of template within the connection instance
		/// </summary>
		public int TemplateId { get; set; }

		/// <summary>
		/// Members used in template
		/// </summary>
		public List<ConItem> Members { get; set; } = new List<ConItem>();

		/// <summary>
		/// Operation Ids used in template
		/// </summary>
		public List<ConItem> Operations { get; set; } = new List<ConItem>();

		/// <summary>
		/// Operation parameter Ids used in template
		/// </summary>
		public List<string> ParameterKeys { get; set; } = new List<string>();

		/// <summary>
		/// Common properties for all operations in template
		/// Consists of common material id, bolt assembly id, plate material id
		/// </summary>
		public ConOperationCommonProperties CommonProperties { get; set; } = new ConOperationCommonProperties();

		public ConConnectionTemplate()
		{
			
		}

		public ConConnectionTemplate(Guid tempalteId, 
			int instanceId,
			List<ConItem> members, 
			List<ConItem> operations, 
			List<string> parameterKeys, 
			ConOperationCommonProperties commonProperties)
		{
			LibraryTemplateId = tempalteId;
			TemplateId = instanceId;
			Members = members;
			Operations = operations;
			ParameterKeys = parameterKeys;
			CommonProperties = commonProperties;
		}
	}
}
