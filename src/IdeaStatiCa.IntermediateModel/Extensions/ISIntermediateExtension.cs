using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Extensions
{

	static public class ISIntermediateExtension
	{
		/// <summary>
		/// Find and get element
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		static public IEnumerable<ISIntermediate> GetElements(this ISIntermediate intermediateItem, string filter)
		{
			var queue = PrepareFilter(filter);

			return intermediateItem.GetElements(queue);
		}

		/// <summary>
		/// Find and get element
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		static public IEnumerable<ISIntermediate> GetElements(this ISIntermediate intermediateItem, Queue<string> filter) => intermediateItem switch
		{
			SObject sObject => sObject.GetElements(filter),
			SList sList => sList.GetElements(filter),
			SPrimitive sPrimitive => sPrimitive.GetElements(filter),
			SAttribute sAttribute => sAttribute.GetElements(filter),
			_ => new List<ISIntermediate>() { },
		};

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
		public static string GetElementName(this ISIntermediate intermediateItem) => intermediateItem switch
		{
			SObject sObject => sObject.GetElementName(),
			SList sList => sList.GetElementName(),
			SPrimitive sPrimitive => sPrimitive.GetElementName(),
			SAttribute sAttribute => sAttribute.GetElementName(),
			_ => throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}"),
		};


		/// <summary>
		/// Get Element Value
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static string GetElementValue(this ISIntermediate intermediateItem, string property) => intermediateItem switch
		{
			SObject sObject => sObject.GetElementValue(property),
			SPrimitive sPrimitive => sPrimitive.GetElementValue(property),
			SAttribute sAttribute => sAttribute.GetElementValue(property),
			_ => throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}"),
		};

		/// <summary>
		/// Take (Get and Remove) Element Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static ISIntermediate TakeElementProperty(this ISIntermediate intermediateItem, string property) => intermediateItem switch
		{
			SObject sObject => sObject.TakeElementProperty(property),
			SList sList => sList.TakeElementProperty(property),
			SPrimitive sPrimitive => sPrimitive.TakeElementProperty(property),
			SAttribute sAttribute => sAttribute.TakeElementProperty(property),
			_ => throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}"),
		};



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
				default: throw new InvalidOperationException($"Unsupported type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		/// <summary>
		/// Try Add Element Property
		/// </summary>
		/// <param name="intermediateItem"></param>
		/// <param name="property"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static bool TryAddElementProperty(this ISIntermediate intermediateItem, ISIntermediate property) => intermediateItem switch
		{
			SObject sObject => sObject.TryAddElementProperty(property),
			SList sList => sList.TryAddElementProperty(property),
			SPrimitive sPrimitive => sPrimitive.TryAddElementProperty(property),
			SAttribute sAttribute => sAttribute.TryAddElementProperty(property),
			_ => throw new InvalidOperationException($"Unsupported type of intermediateItem {intermediateItem.GetType()}"),
		};


		private static Queue<string> PrepareFilter(string filter)
		{
			var filterParts = ExplodeFilter(filter);
			var queue = new Queue<string>();
			filterParts.ForEach(fp => queue.Enqueue(fp));

			return queue;
		}

		private static List<string> ExplodeFilter(string filterParh)
		{
			return filterParh.Split(";").ToList();
		}

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
				default: throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
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
				default: throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}
	}
}
