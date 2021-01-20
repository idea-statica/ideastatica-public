using System;
using System.ComponentModel;

namespace IdeaRS.OpenModel
{
	/// <summary>
	/// Base attribute of the Open model
	/// </summary>
	[BrowsableAttribute(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class OpenModelClassAttribute : Attribute
	{
		/// <summary>
		/// 
		/// </summary>
		public String StructuralModelClassType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public String StructuralModelContainerType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Type OpenModelListType { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="structuralModelClassType">Equivalent SM type</param>
		/// <param name="structuralModelContainerType">SM container type</param>
		/// <param name="openModelListType">List type in Open Model</param>
		public OpenModelClassAttribute(string structuralModelClassType, String structuralModelContainerType, Type openModelListType)
		{
			StructuralModelClassType = structuralModelClassType;
			StructuralModelContainerType = structuralModelContainerType;
			OpenModelListType = openModelListType;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="structuralModelClassType">Equivalent SM type</param>
		/// <param name="openModelListType">List type in Open Model</param>
		public OpenModelClassAttribute(string structuralModelClassType, Type openModelListType)
		{
			StructuralModelClassType = structuralModelClassType;
			StructuralModelContainerType = String.Empty;
			OpenModelListType = openModelListType;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="structuralModelClassType">Equivalent SM type</param>
		/// <param name="structuralModelContainerType">SM container type</param>
		public OpenModelClassAttribute(string structuralModelClassType, String structuralModelContainerType)
		{
			StructuralModelClassType = structuralModelClassType;
			StructuralModelContainerType = structuralModelContainerType;
			OpenModelListType = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="structuralModelClassType">Equivalent SM type</param>
		public OpenModelClassAttribute(string structuralModelClassType)
		{
			StructuralModelClassType = structuralModelClassType;
			StructuralModelContainerType = String.Empty;
			OpenModelListType = null;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="openModelListType">List type in Open Model</param>
		public OpenModelClassAttribute(Type openModelListType)
		{
			StructuralModelClassType = String.Empty;
			StructuralModelContainerType = String.Empty;
			OpenModelListType = openModelListType;
		}
	}
}
