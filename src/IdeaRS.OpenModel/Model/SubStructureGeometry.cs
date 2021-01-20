using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel.Model
{

	/// <summary>
	/// SubStructure
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.SubStructure,CI.StructuralElements", "CI.StructModel.Structure.ISubStructure,CI.BasicTypes")]
	public class SubStructure : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SubStructure()
		{
			Geometry = new SubStructureGeometry();
			Loading = null;
		}

		/// <summary>
		/// Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Geometry
		/// </summary>
		public SubStructureGeometry Geometry { get; set; }

		/// <summary>
		/// Loading
		/// </summary>
		public SubStructureLoading Loading { get; set; }
	}


	/// <summary>
	/// SubStructureLoading
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.SubStructureLoading, CI.StructuralElements", "CI.StructModel.Structure.ISubStructureLoading,CI.BasicTypes")]
	public class SubStructureLoading : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SubStructureLoading()
		{
			LoadCases = new List<ReferenceElement>();
		}
		/// <summary>
		/// List of LoadCases
		/// </summary>
		public List<ReferenceElement> LoadCases
		{
			get;
			set;
		}
	}

	/// <summary>
	/// SubStructureGeometry
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.SubStructureGeometry, CI.StructuralElements", "CI.StructModel.Structure.ISubStructureGeometry,CI.BasicTypes")]
	public class SubStructureGeometry : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public SubStructureGeometry()
		{
			ConnectionPoints = new List<ReferenceElement>();
			RealMembers = new List<DesignMemberInSubStructure>();
			TheoreticalMembers = new List<DesignMemberInSubStructure>();
		}

		/// <summary>
		/// Array of connection points
		/// </summary>
		/// <returns>List of ConnectionPoints</returns>
		public List<ReferenceElement> ConnectionPoints { get; set; }

		/// <summary>
		/// Array of DesignMemberInSubStructure
		/// </summary>
		/// <returns>List of DesignMemberInSubStructure</returns>
		public List<DesignMemberInSubStructure> RealMembers { get; set; }

		/// <summary>
		/// Array of DesignMemberInSubStructure
		/// </summary>
		/// <returns>List of DesignMemberInSubStructure</returns>
		public List<DesignMemberInSubStructure> TheoreticalMembers { get; set; }

	}


	/// <summary>
	/// DesignMemberInSubStructure
	/// </summary>
	[OpenModelClass("CI.StructModel.Structure.DesignMemberInSubStructure, CI.StructuralElements", "CI.StructModel.Structure.IDesignMemberInSubStructure,CI.BasicTypes")]
	public class DesignMemberInSubStructure : OpenObject
	{
		/// <summary>
		/// Reference to DesignMember
		/// </summary>
		public ReferenceElement DesignMember { get; set; }
	}
}
