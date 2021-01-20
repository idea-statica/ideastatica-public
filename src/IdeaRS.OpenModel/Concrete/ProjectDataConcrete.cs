using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Project data concrete
	/// </summary>
	[XmlInclude(typeof(ProjectDataConcreteEc2))]
	public abstract class ProjectDataConcrete : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ProjectDataConcrete()
		{
		}

		/// <summary>
		/// Design working fife
		/// </summary>
		public int DesignWorkingLife { get; set; }
	}
}