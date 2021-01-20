namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Project data concrete Ec2
	/// </summary>
	[OpenModelClass("CI.Services.Concrete.Ec2.ProjectDataConcreteEc2,CI.ProjectData", "CI.Services.Concrete.IProjectDataConcrete,CI.BasicTypes", typeof(ProjectDataConcrete))]
	public class ProjectDataConcreteEc2 : ProjectDataConcrete
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ProjectDataConcreteEc2()
			: base()
		{
			CodeEN1992_2 = false;
			CodeEN1992_3 = false;
			DesignWorkingLife = 50;
			//ExposureClassesData = new ExposureClassesDataEc2();
		}

		/// <summary>
		/// Flag if code EN 1992-2 is included
		/// </summary>
		public bool CodeEN1992_2 { get; set; }

		/// <summary>
		/// Flag if code EN 1992-2 is included
		/// </summary>
		public bool CodeEN1992_3 { get; set; }

		///// <summary>
		///// Exposure classes
		///// </summary>
		//[XmlIgnore]
		//public IExposureClassesDataEc2 ExposureClassesData { get; set; }
	}
}