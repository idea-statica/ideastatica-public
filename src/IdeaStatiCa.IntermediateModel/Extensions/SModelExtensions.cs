using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	static public class SModelExtensions
	{
		static public void ChangeElementValue(this SModel model, string filter, string newPropertyValue)
		{
			var queue = new Queue<string>();
			queue.Enqueue(filter);
			model.ChangeElementValue(queue, newPropertyValue);
		}

		static public void ChangeElementValue(this SModel model, Queue<string> filter, string newPropertyValue)
		{
			var foundItems = model.GetElements(filter);
			foreach (var item in foundItems)
			{
				item.ChangeElementValue(newPropertyValue);
			}
		}

		static public void ChangeElementName(this SModel model, string filter, string newPropertyName)
		{
			var queue = new Queue<string>();
			queue.Enqueue(filter);
			model.ChangeElementName(queue, newPropertyName);
		}

		static public void ChangeElementName(this SModel model, Queue<string> filter, string newPropertyName)
		{
			var foundItems = model.GetElements(filter);
			foreach (var item in foundItems)
			{
				item.ChangeElementPropertyName(filter.Last(), newPropertyName);
			}
		}

		static IEnumerable<ISIntermediate> GetElements(this SModel model, Queue<string> filter)
		{
			return model.RootItem.GetElements(filter);
		}

		/// <summary>
		/// Find and keep
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		static public IEnumerable<ISIntermediate> GetElements(this SModel model, string filter)
		{
			return model.RootItem.GetElements(filter);
		}


		public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
									  TKey fromKey, TKey toKey)
		{
			TValue value = dic[fromKey];
			dic.Remove(fromKey);
			dic[toKey] = value;
		}
	}
}
