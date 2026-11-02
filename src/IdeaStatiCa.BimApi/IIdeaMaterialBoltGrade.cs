using IdeaRS.OpenModel.Material;

namespace IdeaStatiCa.BimApi
{
	/// <summary>
	/// A boltgrade material.
	/// </summary>
	public interface IIdeaMaterialBoltGrade : IIdeaMaterial
	{
		/// <summary>
		/// Parameters of the material.
		/// </summary>
		MaterialBoltGradeAISC Material { get; }
	}
}