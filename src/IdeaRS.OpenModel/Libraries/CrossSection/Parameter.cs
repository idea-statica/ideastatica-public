using System.Xml.Serialization;

namespace IdeaRS.OpenModel.CrossSection
{
	/// <summary>
	/// Common parameter of cross-section
	/// </summary>
	[XmlInclude(typeof(ParameterDouble))]
	[XmlInclude(typeof(ParameterInt))]
	[XmlInclude(typeof(ParameterBool))]
	[XmlInclude(typeof(ParameterString))]
	public abstract class Parameter : OpenObject
	{
		/// <summary>
		/// Name of property
		/// </summary>
		public string Name { get; set; }
	}

	/// <summary>
	/// Double parameter of cross-section
	/// </summary>
	public class ParameterDouble : Parameter
	{
		/// <summary>
		/// Value
		/// </summary>
		public double Value { get; set; }
	}

	/// <summary>
	/// Integer parameter of cross-section
	/// </summary>
	public class ParameterInt : Parameter
	{
		/// <summary>
		/// Value
		/// </summary>
		public int Value { get; set; }
	}

	/// <summary>
	/// Boolean parameter of cross-section
	/// </summary>
	public class ParameterBool : Parameter
	{
		/// <summary>
		/// Value
		/// </summary>
		public bool Value { get; set; }
	}

	/// <summary>
	/// String parameter of cross-section
	/// </summary>
	public class ParameterString : Parameter
	{
		/// <summary>
		/// Value
		/// </summary>
		public string Value { get; set; }
	}

	/// <summary>
	/// Reference element parameter of cross-section
	/// </summary>
	public class ParameterReferenceElement : Parameter
	{
		/// <summary>
		/// Value
		/// </summary>
		public ReferenceElement Value { get; set; }
	}
}