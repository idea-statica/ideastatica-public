using System.Runtime.InteropServices;

namespace CI.Geometry3D
{
	[Guid("6ff22179-ea1d-37ce-ab30-d38aeb556f04")]
	public interface IArcSegment3D : ISegment3D
	{
		/// <summary>
		/// gets or sets the intermediate point of the arc segment
		/// </summary>
		IPoint3D IntermedPoint
		{
			get;
			set;
		}
	}
}
