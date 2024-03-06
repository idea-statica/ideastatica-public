using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IOM.VersioningService
{
	public interface IVersioningService
	{
		/// <summary>
		/// Load model - prepare versioning service for modifying model 
		/// </summary>
		/// <param name="model"></param>
		void LoadModel(SModel model);

		/// <summary>
		/// Is Model conversed to latest version
		/// </summary>
		/// <returns></returns>
		bool IsModelActual();
	}
}
