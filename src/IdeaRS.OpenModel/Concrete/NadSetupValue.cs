using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Setup value base
	/// </summary>
	[XmlInclude(typeof(NadSetupValueBool))]
	[XmlInclude(typeof(NadSetupValueShort))]
	[XmlInclude(typeof(NadSetupValueInt))]
	[XmlInclude(typeof(NadSetupValueLong))]
	[XmlInclude(typeof(NadSetupValueFloat))]
	[XmlInclude(typeof(NadSetupValueDouble))]
	public abstract class NadSetupValue
	{
		/// <summary>
		/// Gets or sets the value id.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Is active
		/// </summary>
		public bool IsActive { get; set; }
	}

	/// <summary>
	/// Setup value bool
	/// </summary>
	public class NadSetupValueBool : NadSetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public bool Value { get; set; }
	}

	/// <summary>
	/// Setup value bool
	/// </summary>
	public class NadSetupValueShort : NadSetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public short Value { get; set; }
	}

	/// <summary>
	/// Setup value int
	/// </summary>
	public class NadSetupValueInt : NadSetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public int Value { get; set; }
	}

	/// <summary>
	/// Setup value long
	/// </summary>
	public class NadSetupValueLong : NadSetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public long Value { get; set; }
	}

	/// <summary>
	/// Setup value float
	/// </summary>
	public class NadSetupValueFloat : NadSetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public float Value { get; set; }
	}

	/// <summary>
	/// Setup value double
	/// </summary>
	public class NadSetupValueDouble : NadSetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public double Value { get; set; }
	}
}