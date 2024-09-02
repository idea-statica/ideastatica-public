using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// ISteelSetup
	/// </summary>
	[XmlInclude(typeof(SteelSetupECEN))]
	[XmlInclude(typeof(SteelSetupAISC))]
	[XmlInclude(typeof(SteelSetupCISC))]
	[XmlInclude(typeof(SteelSetupAUS))]
	[XmlInclude(typeof(SteelSetupRUS))]
	[XmlInclude(typeof(SteelSetupIND))]
	[XmlInclude(typeof(SteelSetupHKG))]
	[XmlInclude(typeof(SteelSetupCHN))]
	[KnownType(typeof(SteelSetupECEN))]
	[KnownType(typeof(SteelSetupAISC))]
	[KnownType(typeof(SteelSetupCISC))]
	[KnownType(typeof(SteelSetupAUS))]
	[KnownType(typeof(SteelSetupRUS))]
	[KnownType(typeof(SteelSetupIND))]
	[KnownType(typeof(SteelSetupHKG))]
	[KnownType(typeof(SteelSetupCHN))]
	public abstract class SteelSetup
	{
		/// <summary>
		/// Friction Coefficient Pbolt Default
		/// </summary>
		/// <returns></returns>
		public virtual double FrictionCoefficientPboltDefault()
		{
			return 0.30;
		}
	}
}
