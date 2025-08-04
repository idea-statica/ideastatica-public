using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Resx = IdeaRS.OpenModel.Properties.Resources;

namespace IdeaRS.OpenModel.Message
{
	/// <summary>
	/// Open Warning message
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[DataContract]
	public class WarningMessage : OpenMessage
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public WarningMessage()
			: base()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="number">Number of the message</param>
		/// <param name="description">Description of the message</param>
		/// <param name="openObject">Affected object</param>
		/// <param name="propertyName">Affected property</param>
		/// <param name="innerMessage">Inner message</param>
		public WarningMessage(MessageNumber number, OpenObject openObject, string propertyName, string description, OpenMessage innerMessage = null)
			: base(number, openObject, propertyName, description, innerMessage)
		{
			Debug.Assert((number & MessageNumber.Warning) != 0, "No warning Message number in Warning message");
		}

		/// <summary>
		/// Gets the Message
		/// </summary>
		[XmlIgnore]
		public override string Message
		{
			get
			{
				return string.Format("{0},\n{1}", Resx.Warning, base.Message);
			}
		}
	}
}
