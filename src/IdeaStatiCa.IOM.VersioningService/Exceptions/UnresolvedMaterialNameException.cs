using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IOM.VersioningService.Exceptions
{
	/// <summary>
	/// Thrown by an upgrade step that turns a material name into a reference when the model does not list the name: the
	/// reference would leave the element without its material.
	/// </summary>
	public class UnresolvedMaterialNameException : Exception
	{
		public UnresolvedMaterialNameException(IReadOnlyList<UnresolvedMaterialName> items)
			: base(Describe(items))
		{
			Items = items;
		}

		/// <summary>
		/// Every element of the model whose material name is not listed.
		/// </summary>
		public IReadOnlyList<UnresolvedMaterialName> Items { get; }

		private static string Describe(IReadOnlyList<UnresolvedMaterialName> items)
		{
			return "The model does not contain the material named by "
				+ string.Join("; ", items.Select(DescribeItem));
		}

		private static string DescribeItem(UnresolvedMaterialName item)
		{
			var origin = string.IsNullOrEmpty(item.OriginalModelId) ? string.Empty : $", OriginalModelId {item.OriginalModelId}";
			return $"{item.ElementKind} '{item.ElementName}' (Id {item.ElementId}{origin}): '{item.MaterialName}'";
		}
	}
}
