using System.Xml.Linq;
using System.Xml.Serialization;

namespace CI
{
	/// <summary>
	/// Provides custom formatting for XML-Linq serialization and deserialization (using XElement).
	/// </summary>
	public interface IXSerializable
	{
		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="source">The <c>System.Xml.Linq.XElement</c> from which the object is deserialized.</param>
		/// <param name="customName">The custom name of <c>System.Xml.Linq.XElement</c>.
		/// If it is null, than is taken from typeof(ValueTT).Name.</param>
		void ReadXElement(XElement source, string customName = null);

		/// <summary>
		/// Converts an object into its XML representation by <c>System.Xml.Linq.XElement</c>.
		/// </summary>
		/// <param name="customName">The custom name of generated <c>System.Xml.Linq.XElement</c>.
		/// If it is null, than is taken from typeof(ValueTT).Name.</param>
		/// <returns>The <c>System.Xml.Linq.XElement</c> to which the object is serialized.</returns>
		XElement WriteXElement(string customName = null);

		/// <summary>
		/// Returns the default name of Xml element, if parameter name id not null.
		/// </summary>
		/// <param name="name">The custom name, if null, than returns default name.</param>
		/// <returns>The name of Xml element.</returns>
		string GetXmlName(string name);
	}

	/// <summary>
	/// Provides custom formatting for XML-Linq serialization and deserialization (using XElement).
	/// </summary>
	public interface IXSerializableContainer
	{
		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <typeparam name="TObject">The deserialized object class.</typeparam>
		/// <param name="source">The <c>System.Xml.Linq.XElement</c> from which the object is deserialized.</param>
		/// <param name="ser">The XmlSerializer...if null, will be created based on the TObject.</param>
		/// <param name="customName">The custom name of <c>System.Xml.Linq.XElement</c>.
		/// <param name="clear">indicator to clear container - default is true</param>
		/// If it is null, than is taken from typeof(ValueTT).Name.</param>
		void ReadXElement<TObject>(XElement source, XmlSerializer ser = null, string customName = null, bool clear = true) where TObject : class;

		/// <summary>
		/// Converts an object into its XML representation by <c>System.Xml.Linq.XElement</c>.
		/// </summary>
		/// <typeparam name="TObject">The serialized object class.</typeparam>
		/// <param name="ser">The XmlSerializer...if null, will be created based on the TObject.</param>
		/// <param name="customName">The custom name of generated <c>System.Xml.Linq.XElement</c>.
		/// If it is null, than is taken from typeof(ValueTT).Name.</param>
		/// <returns>The <c>System.Xml.Linq.XElement</c> to which the object is serialized.</returns>
		XElement WriteXElement<TObject>(XmlSerializer ser = null, string customName = null) where TObject : class;

		/// <summary>
		/// Returns the default name of Xml element, if parameter name id not null.
		/// </summary>
		/// <typeparam name="TObject">The serialized object class.</typeparam>
		/// <param name="name">The custom name, if null, than returns default name.</param>
		/// <returns>The name of Xml element.</returns>
		string GetXmlName<TObject>(string name) where TObject : class;
	}
}
