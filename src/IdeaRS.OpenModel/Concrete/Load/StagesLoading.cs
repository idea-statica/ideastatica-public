using System.Collections.Generic;

namespace IdeaRS.OpenModel.Concrete.Load
{
	/// <summary>
	/// Type of effects in cross-section component
	/// </summary>
	public enum CssEffectType
	{
		/// <summary>
		/// internal forces
		/// </summary>
		InternalForce = 0,

		/// <summary>
		/// deformation plane
		/// </summary>
		DeformationPlane,
	}

	/// <summary>
	/// Type of input of prestressing
	/// </summary>
	public enum PrestressInputType
	{
		///// <summary>
		///// user input - stress, losses by percentage
		///// </summary>
		//UserInput = 0,

		/// <summary>
		/// stress after transfer - long term losses will be calculated
		/// </summary>
		AfterTransfer = 1,

		///// <summary>
		///// stress before transfer - part short-term losses and long-term losses will be calculated
		///// </summary>
		//BeforeTransfer,

		/// <summary>
		/// stress after long term losses is added to tendon
		/// </summary>
		AfterLongTermLosses = 3,
	}

	/// <summary>
	/// Stages loading
	/// </summary>
	public class StagesLoading : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public StagesLoading()
		{
			CssEffectType = CssEffectType.InternalForce;
			PrestressInputType = PrestressInputType.AfterTransfer;
			PermanentLoad = new List<StageLoading>();
			PrestressLoad = new List<PrestressLoading>();
			CssComponentLoad = new List<CssComponentLoading>();
			TendonComponentLoad = new List<TendonComponentLoading>();
		}

		/// <summary>
		/// Type of effects in cross-section component
		/// </summary>
		public CssEffectType CssEffectType { get; set; }

		/// <summary>
		/// Type of input of prestressing
		/// </summary>
		public PrestressInputType PrestressInputType { get; set; }

		/// <summary>
		/// Permanent loading
		/// </summary>
		public List<StageLoading> PermanentLoad { get; set; }

		/// <summary>
		/// Prestress loading
		/// </summary>
		public List<PrestressLoading> PrestressLoad { get; set; }

		/// <summary>
		/// Cross-section component loading
		/// </summary>
		public List<CssComponentLoading> CssComponentLoad { get; set; }

		/// <summary>
		/// Tendon component loading
		/// </summary>
		public List<TendonComponentLoading> TendonComponentLoad { get; set; }
	}
}