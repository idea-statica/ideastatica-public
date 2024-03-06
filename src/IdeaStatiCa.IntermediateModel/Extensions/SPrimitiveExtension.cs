using IdeaStatiCa.IntermediateModel.IRModel;
using System;
using System.Collections.Generic;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	public static class SPrimitiveExtension
	{
		private static readonly string DefaultName = "Value";
		public static void AddElementProperty(this SPrimitive sPrimitive, ISIntermediate property)
		{
			throw new NotImplementedException();
		}

		public static bool TryAddElementProperty(this SPrimitive sPrimitive, ISIntermediate property)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get elements by filter
		/// </summary>
		/// <param name="sPrimitive"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static IEnumerable<ISIntermediate> GetElements(this SPrimitive sPrimitive, Queue<string> filter)
		{
			return new List<ISIntermediate>() { };
		}

		/// <summary>
		/// Get Element Name
		/// </summary>
		/// <param name="sPrimitive"></param>
		/// <returns></returns>
		public static string GetElementName(this SPrimitive sPrimitive)
		{
			return DefaultName;
		}

		/// <summary>
		/// Get Element Value
		/// </summary>
		/// <param name="sPrimitive"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static string GetElementValue(this SPrimitive sPrimitive, string property = null)
		{
			return sPrimitive.Value;
		}

		public static void ChangeElementPropertyName(this SPrimitive sPrimitive, string name, string newName)
		{
			throw new InvalidOperationException("SPrimitive can not be renamed");
		}

		/// <summary>
		/// Change Element Value
		/// </summary>
		/// <param name="sPrimitive"></param>
		/// <param name="newValue"></param>
		public static void ChangeElementValue(this SPrimitive sPrimitive, string newValue)
		{
			sPrimitive.Value = newValue;
		}
	}
}
