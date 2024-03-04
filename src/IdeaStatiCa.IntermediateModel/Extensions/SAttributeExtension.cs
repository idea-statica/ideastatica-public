using IdeaStatiCa.IntermediateModel.IRModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	public static class SAttributeExtension
	{
		public static void AddElementProperty(this SAttribute sAttribute, ISIntermediate property)
		{
			throw new NotImplementedException();
		}

		public static bool TryAddElementProperty(this SAttribute sAttribute, ISIntermediate property)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get Elements 
		/// </summary>
		/// <param name="sAttribute"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static IEnumerable<ISIntermediate> GetElements(this SAttribute sAttribute, Queue<string> filter)
		{
			if (filter == null || filter.Count == 0)
			{
				return Enumerable.Empty<ISIntermediate>();
			}

			var filterPart = filter.Dequeue();
			if (sAttribute.LocalName == filterPart)
			{
				return new List<ISIntermediate>() { sAttribute };
			}
			else
			{
				return new List<ISIntermediate>() { };
			}
		}

		public static string GetElementName(this SAttribute sAttribute)
		{
			throw new NotImplementedException();
		}

		public static ISIntermediate TakeElementProperty(this SAttribute sAttribute, string filter)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get Attribute Value
		/// </summary>
		/// <param name="sAttribute"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static string GetElementValue(this SAttribute sAttribute, string property = null)
		{
			return sAttribute.Value;
		}

		public static void ChangeElementPropertyName(this SAttribute sAttribute, string name, string newName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Change Attribute Value
		/// </summary>
		/// <param name="sAttribute"></param>
		/// <param name="newValue"></param>
		public static void ChangeElementValue(this SAttribute sAttribute, string newValue)
		{
			sAttribute.Value = newValue;
		}
	}
}
