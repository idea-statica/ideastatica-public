using System.Runtime.InteropServices;

namespace CI.DataModel
{
	/// <summary>
	/// Represents copyable object
	/// </summary>
	[ComVisible(true)]
	public interface ICopyable
	{
		/// <summary>
		/// Creates shallow copy of the object
		/// </summary>
		/// <returns></returns>
		object GetShallowCopy();
	}
}
