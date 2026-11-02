using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CI.Common
{
	/// <summary>
	/// Implementation of INotifyPropertyChanged
	/// </summary>
	public class NotifyPropertyChangedImpl : INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises on PropertyChanged event
		/// </summary>
		/// <param name="name">The name of the property that changed.</param>
		protected void OnPropertyChanged([CallerMemberName]string name = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		protected bool IsHandler { get { return (PropertyChanged != null); } }

		/// <summary>
		/// Wraps <paramref name="origArg"/> into <see cref="ForwardedPropertyChangedEventArgs"/> and raises <see cref="PropertyChangedEventHandler"/>.
		/// </summary>
		/// <param name="propertyName">Name of the property in this object.</param>
		/// <param name="origSender">Original sender.</param>
		/// <param name="origArg">Original argument.</param>
		protected void ForwardPropertyChanged(string propertyName, object origSender, PropertyChangedEventArgs origArg)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				if (origArg is ForwardedPropertyChangedEventArgs)
				{
					origSender = (origArg as ForwardedPropertyChangedEventArgs).OriginalSender;
				}
				handler(this, new ForwardedPropertyChangedEventArgs(propertyName, origSender, origArg));
			}
		}
	}
}
