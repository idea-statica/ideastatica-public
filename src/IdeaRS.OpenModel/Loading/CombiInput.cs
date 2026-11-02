using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Loading
{
	/// <summary>
	/// Basic class combination
	/// </summary>
	[XmlInclude(typeof(CombiInputEC))]
	[XmlInclude(typeof(CombiInputSIA))]
	public abstract class CombiInput : OpenElementId
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public CombiInput()
		{
			Items = new System.Collections.Generic.List<CombiItem>();
		}

		/// <summary>
		/// Name of combination
		/// </summary>
		public System.String Name { get; set; }

		/// <summary>
		/// Combi Items in combination
		/// </summary>
		public System.Collections.Generic.List<CombiItem> Items { get; set; }

		/// <summary>
		/// Additional info
		/// </summary>
		public System.String Description { get; set; }
	}
}