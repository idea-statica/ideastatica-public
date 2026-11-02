using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Stirrup
	/// </summary>
	[OpenModelClass("CI.Services.Concrete.ReinforcedSection.OneStirrup,CI.ReinforcedSection")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class Stirrup : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Stirrup()
		{
			IsClosed = false;
			ShearCheck = false;
			TorsionCheck = false;
			Distance = 0.2;
			DiameterOfMandrel = 1;
			Diameter = 0.06;
		}

		/// <summary>
		/// Geometry
		/// </summary>
		public PolyLine2D Geometry { get; set; }

		/// <summary>
		/// Diameter
		/// </summary>
		public double Diameter { get; set; }

		/// <summary>
		/// Material
		/// </summary>
		public ReferenceElement Material { get; set; }

		/// <summary>
		/// Anchorage Lenght
		/// </summary>
		public double AnchorageLenght { get; set; }

		/// <summary>
		/// Radius of stirrup mandrel - refering to stirrup axis
		/// </summary>
		public double DiameterOfMandrel { get; set; }

		/// <summary>
		/// Open / Closed stirrup
		/// </summary>
		public bool IsClosed { get; set; }

		/// <summary>
		/// Longitudinal distance between stirrups
		/// </summary>
		public double Distance { get; set; }

		/// <summary>
		/// Status of shear check, not possible for detailing stirrup
		/// </summary>
		public bool ShearCheck { get; set; }

		/// <summary>
		/// Status of torsion check, not possible for detailing stirrup
		/// </summary>
		public bool TorsionCheck { get; set; }
	}
}