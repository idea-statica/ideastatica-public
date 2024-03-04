using IdeaStatiCa.IntermediateModel.IRModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	static public class SObjectExtension
	{
		/// <summary>
		/// Add Element Property
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="property"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public static void AddElementProperty(this SObject sObject, ISIntermediate property)
		{
			if (!sObject.TryAddElementProperty(property))
			{
				throw new InvalidOperationException($"Object {sObject.TypeName} already contains property {property.GetElementName()}");
			}
		}

		/// <summary>
		/// Try Add Element Property
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static bool TryAddElementProperty(this SObject sObject, ISIntermediate property)
		{
			if (sObject.Properties.ContainsKey(property.GetElementName()))
			{
				return false;
			}
			else
			{
				sObject.Properties.Add(property.GetElementName(), property);
			}
			return true;
		}

		/// <summary>
		/// Get Elements
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		static public IEnumerable<ISIntermediate> GetElements(this SObject sObject, Queue<string> filter)
		{

			if (filter == null || filter.Count == 0)
			{
				return Enumerable.Empty<ISIntermediate>();
			}
			string filterPart = filter.Peek();

			//potentially here allow regex
			//we found item
			if (string.Equals(sObject.TypeName, filterPart, StringComparison.OrdinalIgnoreCase))
			{
				filter.Dequeue();
				if (filter.Count > 0)
				{
					//we found step in chain of filter go deeper
					//pass queue as copy
					var res = sObject.Properties.Values.SelectMany(p => p.GetElements(new Queue<string>(filter)));
					return res;
				}
				else
				{
					//we found item
					return new List<ISIntermediate>() { sObject };
				}
			}
			else
			{
				var res = sObject.Properties.Values.SelectMany(
					p => p.GetElements(new Queue<string>(filter)));
				return res;
			}
		}

		public static string GetElementName(this SObject sObject)
		{
			return sObject.TypeName;
		}

		/// <summary>
		/// Take Element Property
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		public static ISIntermediate TakeElementProperty(this SObject sObject, string filter)
		{
			if (sObject.Properties.TryGetValue(filter, out var res))
			{
				sObject.Properties.Remove(filter);
				return res;
			}
			else
			{
				throw new KeyNotFoundException($"Object {sObject.TypeName} not contains property {filter}. ");
			}
		}

		/// <summary>
		/// Get Element Value
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static string GetElementValue(this SObject sObject, string property = null)
		{
			if (string.IsNullOrEmpty(property))
			{
				if (sObject.Properties.Count == 1 && sObject.Properties.Values.First() is SPrimitive sPrimitive)
				{
					return sPrimitive.GetElementValue();
				}

				throw new InvalidOperationException($"Object {sObject.TypeName} not contains primitive value. ");
			}
			else
			{
				if (sObject.Properties.TryGetValue(property, out ISIntermediate elementProperty))
				{
					return elementProperty.GetElementValue(string.Empty);
				}
				else
				{
					throw new InvalidOperationException($"Object {sObject.TypeName} not contains property {property} with primitive value. ");
				}

			}


		}

		/// <summary>
		/// ChangeElement Property Name
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="name"></param>
		/// <param name="newName"></param>
		public static void ChangeElementPropertyName(this SObject sObject, string name, string newName)
		{
			if (sObject.TypeName == name)
			{
				sObject.TypeName = newName;
			}
			else if (sObject.Properties.ContainsKey(name))
			{
				sObject.Properties[name].ChangeElementPropertyName(name, newName);
				sObject.Properties.RenameKey(name, newName);
			}

		}

		/// <summary>
		/// Change Element Value
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="newValue"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public static void ChangeElementValue(this SObject sObject, string newValue)
		{
			if (sObject.Properties.Count == 0)
			{
				var primitive = new SPrimitive { Value = newValue };
				sObject.Properties[primitive.GetElementName()] = primitive;
				return;
			}

			if (sObject.Properties.Count != 1)
			{
				throw new InvalidOperationException("Object should contain only one property with value");
			}

			if (sObject.Properties.First().Value is SPrimitive sPrimitive)
			{
				sPrimitive.ChangeElementValue(newValue);
			}
			else
			{
				throw new InvalidOperationException("Object should contain only one SPrimitive property with value");
			}
		}

		/// <summary>
		/// Create Element Property
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static SObject CreateElementProperty(this SObject sObject, string name)
		{
			var element = new SObject() { TypeName = name };
			sObject.Properties.Add(sObject.GetElementName(), element);

			return element;
		}

		/// <summary>
		/// Create List Property
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static SList CreateListProperty(this SObject sObject, string name)
		{
			var sList = new SList();
			sObject.Properties.Add(name, sList);
			return sList;
		}

		/// <summary>
		/// Remove Element Property
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="property"></param>
		public static void RemoveElementProperty(this SObject sObject, ISIntermediate property)
		{
			sObject.RemoveElementProperty(property.GetElementName());
		}

		/// <summary>
		/// Remove Element Property
		/// </summary>
		/// <param name="sObject"></param>
		/// <param name="property"></param>
		public static void RemoveElementProperty(this SObject sObject, string property)
		{
			if (sObject.Properties.ContainsKey(property))
			{
				sObject.Properties.Remove(property);
			}
		}
	}
}
