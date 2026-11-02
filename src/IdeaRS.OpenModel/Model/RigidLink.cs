using System.Collections.Generic;

namespace IdeaRS.OpenModel.Model
{
	/// <summary>
	/// Rigid link between nodes
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.RigidLink,CI.StructuralElements", "CI.StructModel.Structure.IRigidLink,CI.BasicTypes")]
	public class RigidLink : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RigidLink()
		{
			SlaveNodes = new List<ReferenceElement>();
		}

		/// <summary>
		/// Node to which all six independent
		/// degrees of freedom for the element are assigned
		/// </summary>
		public ReferenceElement MasterNode { get; set; }

		/// <summary>
		/// Dependent nodes
		/// </summary>
		public List<ReferenceElement> SlaveNodes { get; set; }

		/// <summary>
		/// DOF rigid in RX dirrection
		/// </summary>
		public bool RigidRX { get; set; }

		/// <summary>
		/// DOF rigid in RY dirrection
		/// </summary>
		public bool RigidRY { get; set; }

		/// <summary>
		/// DOF rigid in RZ dirrection
		/// </summary>
		public bool RigidRZ { get; set; }

		/// <summary>
		/// DOF rigid in X dirrection
		/// </summary>
		public bool RigidX { get; set; }

		/// <summary>
		/// DOF rigid in Y dirrection
		/// </summary>
		private bool RigidY { get; set; }

		/// <summary>
		/// DOF rigid in Z dirrection
		/// </summary>
		public bool RigidZ { get; set; }
	}
}