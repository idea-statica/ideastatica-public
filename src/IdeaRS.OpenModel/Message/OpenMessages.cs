using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Message
{
	/// <summary>
	/// Open messages collection
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	[DataContract]
	public class OpenMessages
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public OpenMessages()
		{
			Messages = new List<OpenMessage>();
		}

		/// <summary>
		/// List of messages
		/// </summary>
		[DataMember]
		public List<OpenMessage> Messages { get; set; }

		/// <summary>
		/// Add a message
		/// </summary>
		/// <param name="message">Message to be added</param>
		public void Add(OpenMessage message)
		{
			if (Messages == null)
			{
				Messages = new List<OpenMessage>();
			}

			Messages.Add(message);
		}

		/// <summary>
		/// Remove a message
		/// </summary>
		/// <param name="message">Message will be removed</param>
		public void Remove(OpenMessage message)
		{
			if (Messages == null)
			{
				return;
			}

			Messages.Remove(message);
		}

		/// <summary>
		/// List Error messages only
		/// </summary>
		[XmlIgnore]
		public List<ErrorMessage> ErrorMessages
		{
			get
			{
				return Messages.OfType<ErrorMessage>().ToList();
			}
		}

		/// <summary>
		/// List Worning messages only
		/// </summary>
		[XmlIgnore]
		public List<WarningMessage> WarningMessages
		{
			get
			{
				return Messages.OfType<WarningMessage>().ToList();
			}
		}

		/// <summary>
		/// List Information messages only
		/// </summary>
		[XmlIgnore]
		public List<InformationMessage> InformationMessages
		{
			get
			{
				return Messages.OfType<InformationMessage>().ToList();
			}
		}
	}
}
