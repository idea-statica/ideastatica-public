using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using Resx = IdeaRS.OpenModel.Properties.Resources;

namespace IdeaRS.OpenModel.Message
{
	/// <summary>
	/// Message numbers
	/// </summary>
	[Flags]
	[DataContract]
	public enum MessageNumber : int
	{
		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="Unspecified"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		Unspecified = 0,

		#region General
		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="GeneralInformation"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		Information = 0x1000000,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="GeneralWarning"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		Warning = 0x2000000,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="GeneralError"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		Error = 0x4000000,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="Reserved"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		Reserved = 0x8000000,

		#endregion

		#region Information
		#endregion

		#region Warning
		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="WarnNoPropertyInData"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		WarnNoPropertyInData = Warning | 0x01,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="WarnValueOutOfRange"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		WarnValueOutOfRange = Warning | 0x02,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="WarnCurveCount"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		WarnCurveCount = Warning | 0x03,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="WarnReinforcementBarsCollision"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		WarnReinforcementBarsCollision = Warning | 0x05,

		#endregion

		#region Error

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrNoOpenObject"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrNoOpenObject = Error | 0x01,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrDataObjectNotCreated"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrDataObjectNotCreated = Error | 0x02,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrNoObjectInOpenModel"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrNoObjectInOpenModel = Error | 0x03,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrNoReferenceObjectInOpenModel"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrNoReferenceObjectInOpenModel = Error | 0x04,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrNoEquivalentObjectInDataModel"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrNoEquivalentObjectInDataModel = Error | 0x05,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrNoCrossSectionParameter"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrNoCrossSectionParameter = Error | 0x06,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrBoltsTooClose"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrBoltsTooClose = Error | 0x07,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrBoltsTooCloseEdge"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrBoltsTooCloseEdge = Error | 0x08,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrContactsAngle"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrContactsAngle = Error | 0x09,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrIncorrentMaterialE"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrIncorrentMaterialE = Error | 0x0A,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrIncorrectMaterialEGP"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrIncorrectMaterialEGP = Error | 0x0B,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrPreloadedBoltGrade"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrPreloadedBoltGrade = Error | 0x0C,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrValueOutOfRange"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrValueOutOfRange = Error | 0x0D,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrCurveZeroPoint"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrCurveZeroPoint = Error | 0x0E,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrCurveFunction"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrCurveFunction = Error | 0x0F,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrCurveDecreaseFunction"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrCurveDecreaseFunction = Error | 0x10,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrCurveDerivation"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrCurveDerivation = Error | 0x11,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrCurveNotSet"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrCurveNotSet = Error | 0x12,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrValidPolyline"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrValidPolyline = Error | 0x13,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrWarningLoad"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrWarningLoad = Error | 0x14,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="TimeoutError"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrTimeout = Error | 0x15,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="NoInLibraryError"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrNoInLibrary = Error | 0x16,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="ErrBadWeldMaterialProperty"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrBadWeldMaterialProperty = Error | 0x17,

		/// <summary>
		/// <include file='../IdeaRS.OpenModel/Properties/Resources.resx' path='root/data[@name="OperationError"]/value/text()/*' />
		/// </summary>
		[EnumMember]
		ErrOperation = Error | 0x18,

		#endregion
	}

	/// <summary>
	/// Open message provider
	/// </summary>
	internal static class OpenMessageProvider
	{
		private static IDictionary<MessageNumber, string> messagesString = new Dictionary<MessageNumber, string>();

		/// <summary>
		/// Static constructor
		/// </summary>
		static OpenMessageProvider()
		{
			messagesString.Add(MessageNumber.Unspecified, Resx.Unspecified.ToString());

			//General
			messagesString.Add(MessageNumber.Information, Resx.GeneralInformation.ToString());
			messagesString.Add(MessageNumber.Warning, Resx.GeneralWarning.ToString());
			messagesString.Add(MessageNumber.Error, Resx.GeneralError.ToString());
			messagesString.Add(MessageNumber.Reserved, Resx.Reserved.ToString());

			//Information

			//Warning
			messagesString.Add(MessageNumber.WarnNoPropertyInData, Resx.WarnNoPropertyInData.ToString());
			messagesString.Add(MessageNumber.WarnValueOutOfRange, Resx.WarnValueOutOfRange.ToString());
			messagesString.Add(MessageNumber.WarnCurveCount, Resx.WarnCurveCount.ToString());
			messagesString.Add(MessageNumber.WarnReinforcementBarsCollision, Resx.WarnReinforcementBarsCollision.ToString());

			//Error
			messagesString.Add(MessageNumber.ErrNoOpenObject, Resx.ErrNoOpenObject.ToString());
			messagesString.Add(MessageNumber.ErrDataObjectNotCreated, Resx.ErrDataObjectNotCreated.ToString());
			messagesString.Add(MessageNumber.ErrNoObjectInOpenModel, Resx.ErrNoObjectInOpenModel.ToString());
			messagesString.Add(MessageNumber.ErrNoReferenceObjectInOpenModel, Resx.ErrNoReferenceObjectInOpenModel.ToString());
			messagesString.Add(MessageNumber.ErrNoEquivalentObjectInDataModel, Resx.ErrNoEquivalentObjectInDataModel.ToString());
			messagesString.Add(MessageNumber.ErrNoCrossSectionParameter, Resx.ErrNoCrossSectionParameter.ToString());

			messagesString.Add(MessageNumber.ErrValueOutOfRange, Resx.ErrValueOutOfRange.ToString());
			messagesString.Add(MessageNumber.ErrCurveZeroPoint, Resx.ErrCurveZeroPoint.ToString());
			messagesString.Add(MessageNumber.ErrCurveFunction, Resx.ErrCurveFunction.ToString());
			messagesString.Add(MessageNumber.ErrCurveDecreaseFunction, Resx.ErrCurveDecreaseFunction.ToString());
			messagesString.Add(MessageNumber.ErrCurveDerivation, Resx.ErrCurveDerivation.ToString());
			messagesString.Add(MessageNumber.ErrCurveNotSet, Resx.ErrCurveNotSet.ToString());
			messagesString.Add(MessageNumber.ErrValidPolyline, Resx.ErrValidPolyline.ToString());

			messagesString.Add(MessageNumber.ErrTimeout, Resx.TimeoutError.ToString());
		}

		/// <summary>
		/// Gets message text
		/// </summary>
		/// <param name="number">Message number</param>
		/// <returns>Message text or empty</returns>
		internal static string GetMessageText(MessageNumber number)
		{
			var sres = string.Empty;
			messagesString.TryGetValue(number, out sres);
			if (!string.IsNullOrEmpty(sres))
			{
				return Properties.Resources.ResourceManager.GetString(sres, Properties.Resources.Culture);
			}

			if ((number & MessageNumber.Information) != 0)
			{
				return Resx.GeneralInformation;
			}
			else if ((number & MessageNumber.Warning) != 0)
			{
				return Resx.GeneralWarning;
			}
			else if ((number & MessageNumber.Error) != 0)
			{
				return Resx.GeneralError;
			}
			else if ((number & MessageNumber.Reserved) != 0)
			{
				return Resx.Reserved;
			}
			else
			{
				Debug.Assert(false, "No text assigned for the message number", "Number {0}", number);
				return string.Empty;
			}
		}
	}
}
