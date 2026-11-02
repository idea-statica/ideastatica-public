namespace IdeaRS.OpenModel.Detail
{
	/// <summary>
	/// IDEA StatiCa Detail setting
	/// </summary>
	public class ISDSetting
	{
		/// <summary>
		/// ctor
		/// </summary>
		public ISDSetting()
		{

		}

		/// <summary>
		/// Concrete cover
		/// </summary>
		public double Cover { get; set; }

		/// <summary>
		/// 3D solution
		/// </summary>
		public bool Is3D { get; set; }
	}
}
