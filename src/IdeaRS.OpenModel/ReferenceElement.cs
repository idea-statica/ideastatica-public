using System.Linq;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Reference element class
	/// </summary>
	public class ReferenceElement
	{
		private OpenElementId element = null;
		private System.Int32 id = 0;
		private string typeName = string.Empty;

		/// <summary>
		/// Constructor
		/// </summary>
		public ReferenceElement()
		{
			element = null;
			id = 0;
			typeName = string.Empty;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="element">Referenced element</param>
		public ReferenceElement(OpenElementId element)
		{
			this.element = element;
		}

		/// <summary>
		/// Element type name
		/// </summary>
		public string TypeName
		{
			get
			{
				if (element != null)
				{
					var t = element.GetType();
					var atr = t.GetCustomAttributes(typeof(IdeaRS.OpenModel.OpenModelClassAttribute), true).FirstOrDefault();
					if (atr != null)
					{
						var t1 = (atr as IdeaRS.OpenModel.OpenModelClassAttribute).OpenModelListType;
						if (t1 != null)
						{
							return t1.Name;
						}
					}

					return t.Name;
				}

				return typeName;
			}

			set
			{
				typeName = value;
			}
		}

		/// <summary>
		/// Element Id
		/// </summary>
		public System.Int32 Id
		{
			get
			{
				if (element != null)
				{
					return element.Id;
				}

				return id;
			}

			set
			{
				id = value;
			}
		}

		/// <summary>
		/// Referenced element
		/// </summary>
		[XmlIgnore]
		public OpenElementId Element
		{
			get
			{
				return element;
			}

			set
			{
				element = value;
			}
		}
	}
}