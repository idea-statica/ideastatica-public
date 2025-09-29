using IdeaStatiCa.IntermediateModel.IRModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IntermediateModel.Extensions
{

	public static class ISIntermediateExtension
	{
		/// <summary>
		/// Find and get element
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static IEnumerable<ISIntermediate> GetElements(this ISIntermediate intermediateItem, string filter)
		{
			Queue<string> queue = PrepareFilter(filter);

			return intermediateItem.GetElements(queue);
		}

		/// <summary>
		/// Find and get element
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static IEnumerable<ISIntermediate> GetElements(this ISIntermediate intermediateItem, Queue<string> filter)
		{
			return intermediateItem switch
			{
				SObject sObject => sObject.GetElements(filter),
				SList sList => sList.GetElements(filter),
				SPrimitive sPrimitive => sPrimitive.GetElements(filter),
				SAttribute sAttribute => sAttribute.GetElements(filter),
				_ => [],
			};
		}

		/// <summary>
		/// Change Element name or Element Property Name
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="name"></param>
		/// <param name="newName"></param>
		public static void ChangeElementPropertyName(this ISIntermediate intermediateItem, string name, string newName)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					sObject.ChangeElementPropertyName(name, newName);
					break;
				case SList sList:
					sList.ChangeElementPropertyName(name, newName);
					break;
				case SPrimitive sPrimitive:
					sPrimitive.ChangeElementPropertyName(name, newName);
					break;
				case SAttribute sAttribute:
					sAttribute.ChangeElementPropertyName(name, newName);
					break;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="newName"></param>
		public static void ChangeElementValue(this ISIntermediate intermediateItem, string newName)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					sObject.ChangeElementValue(newName);
					break;
				case SList sList:
					sList.ChangeElementValue(newName);
					break;
				case SPrimitive sPrimitive:
					sPrimitive.ChangeElementValue(newName);
					break;
				case SAttribute sAttribute:
					sAttribute.ChangeElementValue(newName);
					break;
			}
		}

		/// <summary>
		/// Get Element Name
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static string GetElementName(this ISIntermediate intermediateItem)
		{
			return intermediateItem switch
			{
				SObject sObject => sObject.GetElementName(),
				SList sList => sList.GetElementName(),
				SPrimitive sPrimitive => sPrimitive.GetElementName(),
				SAttribute sAttribute => sAttribute.GetElementName(),
				_ => throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}"),
			};
		}


		/// <summary>
		/// Get Element Value
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static string GetElementValue(this ISIntermediate intermediateItem, string property)
		{
			return intermediateItem switch
			{
				SObject sObject => sObject.GetElementValue(property),
				SList sList => sList.GetElementValue(property),
				SPrimitive sPrimitive => sPrimitive.GetElementValue(property),
				SAttribute sAttribute => sAttribute.GetElementValue(property),
				_ => throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}"),
			};
		}

		/// <summary>
		/// Try Get Element Value
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static string TryGetElementValue(this ISIntermediate intermediateItem, string property)
		{
			try
			{
				return intermediateItem.GetElementValue(property);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Take (Get and Remove) Element Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static ISIntermediate TakeElementProperty(this ISIntermediate intermediateItem, string property)
		{
			return intermediateItem switch
			{
				SObject sObject => sObject.TakeElementProperty(property),
				SList sList => sList.TakeElementProperty(property),
				SPrimitive sPrimitive => sPrimitive.TakeElementProperty(property),
				SAttribute sAttribute => sAttribute.TakeElementProperty(property),
				_ => throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}"),
			};
		}

		/// <summary>
		/// Try Take (Get and Remove) Element Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		public static ISIntermediate TryTakeElementProperty(this ISIntermediate intermediateItem, string property)
		{
			try
			{
				return TakeElementProperty(intermediateItem, property);
			}
			catch
			{
				return null;
			}
		}


		/// <summary>
		/// Add Element Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public static void AddElementProperty(this ISIntermediate intermediateItem, ISIntermediate property)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					sObject.AddElementProperty(property);
					break;
				case SList sList:
					sList.AddElementProperty(property);
					break;
				case SPrimitive sPrimitive:
					sPrimitive.AddElementProperty(property);
					break;
				case SAttribute sAttribute:
					sAttribute.AddElementProperty(property);
					break;
				default:
					throw new InvalidOperationException($"Unsupported type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		/// <summary>
		/// Try Add Element Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static bool TryAddElementProperty(this ISIntermediate intermediateItem, ISIntermediate property)
		{
			return intermediateItem switch
			{
				SObject sObject => sObject.TryAddElementProperty(property),
				SList sList => sList.TryAddElementProperty(property),
				SPrimitive sPrimitive => sPrimitive.TryAddElementProperty(property),
				SAttribute sAttribute => sAttribute.TryAddElementProperty(property),
				_ => throw new InvalidOperationException($"Unsupported type of intermediateItem {intermediateItem.GetType()}"),
			};
		}


		private static Queue<string> PrepareFilter(string filter)
		{
			List<string> filterParts = ExplodeFilter(filter);
			Queue<string> queue = new();
			filterParts.ForEach(queue.Enqueue);

			return queue;
		}

		private static List<string> ExplodeFilter(string filterParh) => filterParh.Split(';').ToList();

		/// <summary>
		/// Create Element Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static SObject CreateElementProperty(this ISIntermediate intermediateItem, string name)
		{
			if (intermediateItem is SObject sObject)
			{
				return sObject.CreateElementProperty(name);
			}
			else
			{
				throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		/// <summary>
		/// Create Element Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static void CreateElementProperty(this ISIntermediate intermediateItem, string name, string value)
		{
			if (intermediateItem is SObject sObject)
			{
				sObject.CreateElementProperty(name)
					.ChangeElementValue(value);
			}
			else
			{
				throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		/// <summary>
		/// Create List Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static SList CreateListProperty(this ISIntermediate intermediateItem, string name)
		{
			if (intermediateItem is SObject sObject)
			{
				return sObject.CreateListProperty(name);
			}
			else
			{
				throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		/// <summary>
		/// Remove Element Property from SObject or SList
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public static void RemoveElementProperty(this ISIntermediate intermediateItem, ISIntermediate property)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					sObject.RemoveElementProperty(property);
					break;
				case SList sList:
					sList.RemoveElementProperty(property);
					break;
				default:
					throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		/// <summary>
		/// Remove Element Property from SObject or SList
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public static void RemoveElementProperty(this ISIntermediate intermediateItem, string property)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					sObject.RemoveElementProperty(property);
					break;
				case SList sList:
					sList.RemoveElementProperty(property);
					break;
				default:
					throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		public static bool IsEmpty(this ISIntermediate intermediateItem)
		{
			if (intermediateItem is null)
			{
				return true;
			}

			return intermediateItem switch
			{
				SObject sObject => sObject.Properties.Count == 0,
				SList sList => sList.Count == 0,
				SPrimitive => false,
				SAttribute => false,
				_ => throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}"),
			};
		}

		public static SObject ResolveReferenceElement(this ISIntermediate intermediateItem, SModel model)
		{
			if (intermediateItem is not SObject sObject)
			{
				throw new InvalidOperationException($"ResolveReferenceElement can be called only on SObject, current type is {intermediateItem.GetType()}");
			}

			string typeName = sObject.GetElementValue("TypeName");
			string id = sObject.GetElementValue("Id");

			IEnumerable<ISIntermediate> elements = model.GetElements(typeName + ";" + typeName);
			if (elements is null)
			{
				return null;
			}

			foreach (SObject element in elements.OfType<SObject>())
			{
				string elementId = element.GetElementValue("Id");
				if (string.Equals(id, elementId, StringComparison.OrdinalIgnoreCase))
				{
					return element;
				}
			}

			return null;
		}
	}
}
