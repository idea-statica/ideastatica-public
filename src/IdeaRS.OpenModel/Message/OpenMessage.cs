using IdeaRS.OpenModel.CrossSection;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Resx = IdeaRS.OpenModel.Properties.Resources;

namespace IdeaRS.OpenModel.Message
{
	/// <summary>
	/// Open message base class
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[XmlInclude(typeof(ErrorMessage))]
	[XmlInclude(typeof(WarningMessage))]
	[XmlInclude(typeof(InformationMessage))]
//	[XmlInclude(typeof(CrossSectionParameter))]
	[KnownType(typeof(ErrorMessage))]
	[KnownType(typeof(WarningMessage))]
	[KnownType(typeof(InformationMessage))]
	[DataContract]
	public class OpenMessage : OpenObject
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OpenMessage()
		{
			Number = MessageNumber.Unspecified;
			InnerMessage = null;
			Description = string.Empty;
			Object = null;
			PropertyName = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="number">Number of the message</param>
		/// <param name="description">Description of the message</param>
		/// <param name="openObject">Affected object</param>
		/// <param name="propertyName">Affected property</param>
		/// <param name="innerMessage">Inner message</param>
		internal OpenMessage(MessageNumber number, OpenObject openObject, string propertyName, string description, OpenMessage innerMessage = null)
		{
			Number = number;
			InnerMessage = innerMessage;
			Description = description;
			Object = openObject;
			PropertyName = propertyName;
		}

		/// <summary>
		/// Creating message
		/// </summary>
		/// <param name="number">Number of the message</param>
		/// <param name="description">Description of the message</param>
		/// <param name="openObject">Affected object</param>
		/// <param name="propertyName">Affected property</param>
		/// <param name="innerMessage">Inner message</param>
		/// <returns>The created message or null if the number is not supported</returns>
		public static OpenMessage Create(MessageNumber number, OpenObject openObject, string propertyName, string description, OpenMessage innerMessage = null)
		{
			OpenMessage msg = null;

			if ((number & MessageNumber.Information) != 0)
			{
				msg = new InformationMessage(number, openObject, propertyName, description, innerMessage);
			}
			else if ((number & MessageNumber.Warning) != 0)
			{
				msg = new WarningMessage(number, openObject, propertyName, description, innerMessage);
			}
			else if ((number & MessageNumber.Error) != 0)
			{
				msg = new ErrorMessage(number, openObject, propertyName, description, innerMessage);
			}
			else if ((number & MessageNumber.Reserved) != 0)
			{
				Debug.Assert(false, "Not supported message", "Number {0}", number);
			}
			else
			{
				Debug.Assert(false, "Not supported message", "Number {0}", number);
			}

			return msg;
		}

		/// <summary>
		/// Number of message
		/// </summary>
		[DataMember]
		public MessageNumber Number { get; set; }

		/// <summary>
		/// Inner message
		/// </summary>
		public OpenMessage InnerMessage { get; set; }

		/// <summary>
		/// Description of message
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Affected object
		/// </summary>
		public OpenObject Object { get; set; }

		/// <summary>
		/// Affected property name
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// Gets the value of affected property
		/// </summary>
		[XmlIgnore]
		public object PropertyValue
		{
			get
			{
				if ((Object == null) || (string.IsNullOrEmpty(PropertyName)))
				{
					return null;
				}

				var properties = TypeDescriptor.GetProperties(Object);
				if (properties == null)
				{
					return null;
				}

				var propDescriptor = properties.Find(PropertyName, false);
				if (propDescriptor == null)
				{
					return null;
				}

				return propDescriptor.GetValue(Object);
			}
		}

		/// <summary>
		/// Gets the Message
		/// </summary>
		[XmlIgnore]
		public virtual string Message
		{
			get
			{
				var text = string.Empty;
				if (Object != null)
				{
					var type = Object.GetType();
					if (Object is OpenElementId)
					{
						text = string.Format("{0}: {1}, Id: {2}", Resx.Object, type.Name, (Object as OpenElementId).Id);
					}
					else
					{
						text = string.Format("{0}: {1}", Resx.Object, type.Name);
					}

					if (!string.IsNullOrEmpty(PropertyName))
					{
						text += ",\n";
						var val = PropertyValue;
						if (val != null)
						{
							text += string.Format("{0}: {1}, Value: {2}", Resx.Property, PropertyName, val.ToString());
						}
						else
						{
							text += string.Format("{0}: {1}", Resx.Property, PropertyName);
						}
					}
				}
				else if (!string.IsNullOrEmpty(PropertyName))
				{
					text = string.Format("{0}: {1}", Resx.Property, PropertyName);
				}

				var messageText = OpenMessageProvider.GetMessageText(Number);
				if (!string.IsNullOrEmpty(messageText))
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += ",\n";
					}

					text += messageText;
				}

				if (!string.IsNullOrEmpty(Description))
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += ",\n";
					}

					text += Description;
				}

				return text;
			}
		}
	}
}