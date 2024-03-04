using IdeaStatiCa.IntermediateModel.IRModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	public static class SListExtension
	{
		/// <summary>
		/// Get Elements by filter
		/// </summary>
		/// <param name="sList"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static IEnumerable<ISIntermediate> GetElements(this SList sList, Queue<string> filter)
		{
			if (filter == null || filter.Count == 0 || sList == null || sList.Items == null || sList.Items.Count() == 0)
			{
				return new List<ISIntermediate>();
			}

			var res = sList.Items.SelectMany(p => p.GetElements(new Queue<string>(filter)));
			return res;
		}

		/// <summary>
		/// Change element property name for all items
		/// </summary>
		/// <param name="sList"></param>
		/// <param name="name"></param>
		/// <param name="newName"></param>
		public static void ChangeElementPropertyName(this SList sList, string name, string newName)
		{
			foreach (var item in sList.Items)
			{
				item.ChangeElementPropertyName(name, newName);
			}
		}

		/// <summary>
		/// Change element value for all items
		/// </summary>
		/// <param name="sList"></param>
		/// <param name="newValue"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public static void ChangeElementValue(this SList sList, string newValue)
		{
			if (sList.Items.ToList().TrueForAll(x => x is SPrimitive))
			{
				foreach (var item in sList.Items)
				{
					item.ChangeElementValue(newValue);
				}
			}
			else
			{
				throw new InvalidOperationException($"List not contains only primitive value.");
			}
		}

		/// <summary>
		/// Take element property on index
		/// </summary>
		/// <param name="sList"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static ISIntermediate TakeElementProperty(this SList sList, int index)
		{
			if (sList.Items.Count() > index && index >= 0)
			{
				var element = sList.Items.ToArray()[index];
				sList.Items.Remove(element);
				return element;
			}
			else
			{
				throw new InvalidOperationException($"List not item on index {index}.");
			}
		}

		/// <summary>
		/// Add same element property
		/// </summary>
		/// <param name="sList"></param>
		/// <param name="property"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public static void AddElementProperty(this SList sList, ISIntermediate property)
		{
			if (!sList.TryAddElementProperty(property))
			{
				throw new InvalidOperationException($"List not contains same item {sList.GetElementName()} as new property {property.GetElementName()}.");
			}
		}

		/// <summary>
		/// Try Add Element Property
		/// </summary>
		/// <param name="sList"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static bool TryAddElementProperty(this SList sList, ISIntermediate property)
		{
			if (sList.Items.Count() > 0 && sList.GetElementName() != property.GetElementName())
			{
				return false;
			}
			sList.Add(property);
			return true;
		}

		/// <summary>
		/// Get first element name
		/// </summary>
		/// <param name="sList"></param>
		/// <returns></returns>
		public static string GetElementName(this SList sList)
		{
			return sList.Items.First().GetElementName();
		}
	}
}
