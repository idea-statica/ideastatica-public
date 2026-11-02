using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Concrete setup base class
	/// </summary>
	[XmlInclude(typeof(ConcreteSetupEc2))]
	public abstract class ConcreteSetup : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected ConcreteSetup()
		{
			SetupValues = new System.Collections.Generic.List<SetupValue>();
		}

		/// <summary>
		/// Setup values
		/// </summary>
		public System.Collections.Generic.List<SetupValue> SetupValues { get; set; }
	}
}