using IdeaRS.OpenModel.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Additional data for Ec project
	/// </summary>
	[OpenModelClass("CI.ProjectData.ProjectDataEc,CI.ProjectData")]
	public class ProjectDataEc : OpenObject
	{
		/// <summary>
		/// Gets or sets the actual national annex
		/// </summary>
		public NationalAnnexCode AnnexCode { get; set; }

		/// <summary>
		/// To allow fatigue check
		/// </summary>
		public bool FatigueCheck { get; set; }

		/// <summary>
		/// True for check fatigue accorting 1992-2 Annex NN
		/// </summary>
		public bool FatigueAnnexNN { get; set; }
	}
}
