using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	static public class SObjectExtension
	{
		public static void AddElementProperty(this SObject sObject, ISIntermediate property)
		{
			if (!sObject.TryAddElementProperty(property))
			{
				throw new InvalidOperationException($"Object {sObject.TypeName} already contains property {property.GetElementName()}");
			}
		}

		public static bool TryAddElementProperty(this SObject sObject, ISIntermediate property)
		{
			if (sObject.Properties.ContainsKey(property.GetElementName()))
			{
				return false;
				throw new InvalidOperationException($"Object {sObject.TypeName} already contains property {property.GetElementName()}");
			}
			else
			{
				sObject.Properties.Add(property.GetElementName(), property);
			}
			return true;
		}

		static public IEnumerable<ISIntermediate> GetElements(this SObject sObject, Queue<string> filter)
		{

			if (filter == null || filter.Count == 0)
			{
				return new List<ISIntermediate>();
			}

			if (filter == null)
			{
				return Enumerable.Empty<ISIntermediate>();
			}

			///var filterPart = filter.Dequeue();
			if (!filter.TryPeek(out string filterPart))
			{
				return Enumerable.Empty<ISIntermediate>();
			}

			//potentially here allow regex
			//we found item
			if (sObject.TypeName == filterPart)
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

		public static void ChangeElementPropertyName(this SObject sObject, string name, string newName)
		{
			if (sObject.TypeName == name)
			{
				sObject.TypeName = newName;
			}
			else
			{
				if (sObject.Properties.ContainsKey(name))
				{
					sObject.Properties[name].ChangeElementPropertyName(name, newName);
					sObject.Properties.RenameKey(name, newName);
				}
			}
		}

		public static void ChangeElementValue(this SObject sObject, string newValue)
		{
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


		public static SObject CreateElementProperty(this SObject sObject, string name)
		{
			var element = new SObject() { TypeName = name };
			sObject.Properties.Add(sObject.GetElementName(), element);

			return element;
		}

		public static SList CreateListProperty(this SObject sObject, string name)
		{
			var sList = new SList();
			sObject.Properties.Add(name, sList);
			return sList;
		}

		public static void RemoveElementProperty(this SObject sObject, ISIntermediate property)
		{
			sObject.RemoveElementProperty(property.GetElementName());
		}

		public static void RemoveElementProperty(this SObject sObject, string property)
		{
			if (sObject.Properties.ContainsKey(property))
			{
				sObject.Properties.Remove(property);
			}
		}
	}
}
