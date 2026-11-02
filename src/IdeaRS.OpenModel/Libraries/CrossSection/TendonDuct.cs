using IdeaRS.OpenModel.Geometry2D;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Tendon ducts material type
	/// </summary>
	[DataContract]
	public enum MaterialDuct
	{
		/// <summary>
		/// Metal
		/// </summary>
		Metal,

		/// <summary>
		/// Plastic
		/// </summary>
		Plastic
	}

	/// <summary>
	/// Tendon duct
	/// </summary>
	[OpenModelClass("CI.Services.Concrete.ReinforcedSection.TendonDuct,CI.ReinforcedSection")]
	[DataContract]
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	public class TendonDuct : OpenObject
	{
		/// <summary>
		/// Tendon duct Id
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Position of duct
		/// </summary>
		[OpenModelProperty("CentrePoint")]
		public Point2D Point { get; set; }

		/// <summary>
		/// Tendon duct material
		/// </summary>
		public MaterialDuct MaterialDuct { get; set; }

		/// <summary>
		/// rue for debonding tubes, false for tendon ducts
		/// </summary>
		public bool IsDebondingTube { get; set; }

		/// <summary>
		/// Diameter
		/// </summary>
		public double Diameter { get; set; }
	}
}