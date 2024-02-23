using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Extensions
{

	static public class ISIntermediateExtension
	{
		/// <summary>
		/// Find and keep
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		static public IEnumerable<ISIntermediate> GetElements(this ISIntermediate intermediateItem, string filter)
		{
			var queue = PrepareFilter(filter);

			return intermediateItem.GetElements(queue);
		}

		static public IEnumerable<ISIntermediate> GetElements(this ISIntermediate intermediateItem, Queue<string> filter)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					return sObject.GetElements(filter);
				case SList sList:
					return sList.GetElements(filter);
				case SPrimitive sPrimitive:
					return sPrimitive.GetElements(filter);
				case SAttribute sAttribute:
					return sAttribute.GetElements(filter);
				default:
					return new List<ISIntermediate>() { };
			}
		}

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

		public static string GetElementName(this ISIntermediate intermediateItem)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					return sObject.GetElementName();
				case SList sList:
					return sList.GetElementName();
				case SPrimitive sPrimitive:
					return sPrimitive.GetElementName();
				case SAttribute sAttribute:
					return sAttribute.GetElementName();
				default: throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		public static string GetElementValue(this ISIntermediate intermediateItem, string property)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					return sObject.GetElementValue(property);
				//case SList sList:
				//	return sList.GetElementValue();
				case SPrimitive sPrimitive:
					return sPrimitive.GetElementValue(property);
				case SAttribute sAttribute:
					return sAttribute.GetElementValue(property);
				default: throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		public static ISIntermediate TakeElementProperty(this ISIntermediate intermediateItem, string property)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					return sObject.TakeElementProperty(property);
				case SList sList:
					return sList.TakeElementProperty(property);
				case SPrimitive sPrimitive:
					return sPrimitive.TakeElementProperty(property);
				case SAttribute sAttribute:
					return sAttribute.TakeElementProperty(property);
				default: throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

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

		public static bool TryAddElementProperty(this ISIntermediate intermediateItem, ISIntermediate property)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					return sObject.TryAddElementProperty(property);
				case SList sList:
					return sList.TryAddElementProperty(property);
				case SPrimitive sPrimitive:
					return sPrimitive.TryAddElementProperty(property);
				case SAttribute sAttribute:
					return sAttribute.TryAddElementProperty(property);
				default: throw new InvalidOperationException($"Unsupported type of intermediateItem {intermediateItem.GetType()}");
			}
		}


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

		public static SObject CreateElementProperty(this ISIntermediate intermediateItem, string name)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					return sObject.CreateElementProperty(name);
				case SList sList:
					return sList.CreateElementProperty(name);
				default: throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

		public static SList CreateListProperty(this ISIntermediate intermediateItem, string name)
		{
			switch (intermediateItem)
			{
				case SObject sObject:
					return sObject.CreateListProperty(name);
				case SList sList:
					return sList.CreateListProperty(name);
				default: throw new InvalidOperationException($"Unknown type of intermediateItem {intermediateItem.GetType()}");
			}
		}

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
