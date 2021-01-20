using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Concrete
{
	/// <summary>
	/// Setup value base
	/// </summary>
	[XmlInclude(typeof(SetupValueBool))]
	[XmlInclude(typeof(SetupValueShort))]
	[XmlInclude(typeof(SetupValueInt))]
	[XmlInclude(typeof(SetupValueLong))]
	[XmlInclude(typeof(SetupValueFloat))]
	[XmlInclude(typeof(SetupValueDouble))]
	public abstract class SetupValue
	{
		/// <summary>
		/// Gets or sets the value id.
		/// </summary>
		public int Id { get; set; }
	}

	/// <summary>
	/// Setup value bool
	/// </summary>
	public class SetupValueBool : SetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public bool Value { get; set; }
	}

	/// <summary>
	/// Setup value bool
	/// </summary>
	public class SetupValueShort : SetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public short Value { get; set; }
	}

	/// <summary>
	/// Setup value int
	/// </summary>
	public class SetupValueInt : SetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public int Value { get; set; }
	}

	/// <summary>
	/// Setup value long
	/// </summary>
	public class SetupValueLong : SetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public long Value { get; set; }
	}

	/// <summary>
	/// Setup value float
	/// </summary>
	public class SetupValueFloat : SetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public float Value { get; set; }
	}

	/// <summary>
	/// Setup value double
	/// </summary>
	public class SetupValueDouble : SetupValue
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public double Value { get; set; }
	}
}