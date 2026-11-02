
using System.Runtime.InteropServices;
namespace CI.DataModel
{
	/// <summary>
	/// Support for explicit notification in data model. 
	/// </summary>
	[ComVisible(true)]
	public interface IPropertyDataChanged
	{
		/// <summary>
		/// Explicitly invokes notification in the data model.
		/// </summary>
		/// <param name="property">Name of the property which was changed.</param>
		void NotifyPropertyChanged(string property);
	}
}
