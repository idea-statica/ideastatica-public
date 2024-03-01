using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.OpenModel.VersioningService
{
	public interface IVersioningService
	{
		/// <summary>
		/// Load model - prepare versioning service for modifying model 
		/// </summary>
		/// <param name="model"></param>
		public void LoadModel(SModel model);

		/// <summary>
		/// Is Model conversed to latest version
		/// </summary>
		/// <returns></returns>
		public bool IsModelActual();
	}
}
