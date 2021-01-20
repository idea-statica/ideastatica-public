using System;
using System.ComponentModel;
namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Property attribute 
	/// </summary>
	[BrowsableAttribute(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class OpenModelPropertyAttribute : Attribute
	{
		/// <summary>
		/// Name in the Idea model
		/// </summary>
		public String StructuralModelName = String.Empty;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="structuralModelName">Name of property in the SM</param>
		public OpenModelPropertyAttribute(string structuralModelName)
		{
			StructuralModelName = structuralModelName;
		}
	}
}

