using System;
using System.Collections.Generic;

namespace IdeaStatiCa.Api.Connection.Model
{
	public class ConConnectionTemplateModel
	{
		/// <summary>
		/// Represents the unique identifier of the template
		/// If the same template is used in multiple times, the same Guid is used.
		/// </summary>
		public Guid TempalteId { get; set; }

		/// <summary>
		/// Unique identifier of template within the connection instance
		/// </summary>
		public int InstanceId { get; set; }

		/// <summary>
		/// Ids of members used in template
		/// </summary>
		public List<int> MemberIds { get; set; } = new List<int>();

		/// <summary>
		/// Operation Ids used in template
		/// </summary>
		public List<int> OperationIds { get; set; } = new List<int>();

		/// <summary>
		/// Operation parameter Ids used in template
		/// </summary>
		public List<int> ParameterIds { get; set; } = new List<int>();

		/// <summary>
		/// Id of common plate material used in all operations of the template
		/// If template do not use common plate material, the value is null
		/// </summary>
		public int? PlateMaterialId = null;

		/// <summary>
		/// Id of common weld material used in all operations of the template
		/// If template do not use common weld material, the value is null
		/// </summary>
		public int? WeldMaterialId = null;

		/// <summary>
		/// Id of common bolt assembly used in all operations of the template
		/// If template do not use common bolt assembly, the value is null
		/// </summary>
		public int? BoltAssemblyId = null;

		public ConConnectionTemplateModel()
		{
			
		}

		public ConConnectionTemplateModel(Guid tempalteId, 
			int instanceId,
			List<int> memberIds, 
			List<int> operationIds, 
			List<int> parameterIds, 
			int? plateMaterialId, 
			int? weldMaterialId, 
			int? boltAssemblyId)
		{
			TempalteId = tempalteId;
			InstanceId = instanceId;
			MemberIds = memberIds;
			OperationIds = operationIds;
			ParameterIds = parameterIds;
			PlateMaterialId = plateMaterialId;
			WeldMaterialId = weldMaterialId;
			BoltAssemblyId = boltAssemblyId;
		}
	}
}
