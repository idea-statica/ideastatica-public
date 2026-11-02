using System.ComponentModel;

namespace CI.Common
{
	/// <summary>
	/// Forwarded <see cref="PropertyChangedEventArgs"/>. It allows to bubble notifications in the chain.
	/// </summary>
	public class ForwardedPropertyChangedEventArgs : PropertyChangedEventArgs
	{
		#region Fields
		private PropertyChangedEventArgs originalArgs;
		private object originalSender;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyName">New property name</param>
		/// <param name="originalSender">Reference to changed object</param>
		/// <param name="originalArgs">Name arguments</param>
		public ForwardedPropertyChangedEventArgs(string propertyName, object originalSender, PropertyChangedEventArgs originalArgs)
			: base(propertyName)
		{
			this.originalArgs = originalArgs;
			this.originalSender = originalSender;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets changed object.
		/// </summary>
		public PropertyChangedEventArgs OriginalArgs
		{
			get { return originalArgs; }
		}

		/// <summary>
		/// Gets name of the changed property.
		/// </summary>
		public object OriginalSender
		{
			get { return originalSender; }
		}
		#endregion
	}
}
