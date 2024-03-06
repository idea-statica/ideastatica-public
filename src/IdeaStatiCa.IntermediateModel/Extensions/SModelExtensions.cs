using IdeaStatiCa.IntermediateModel.IRModel;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	static public class SModelExtensions
	{
		/// <summary>
		/// Change Element Value
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <param name="newPropertyValue"></param>
		static public void ChangeElementValue(this SModel model, string filter, string newPropertyValue)
		{
			var queue = new Queue<string>();
			queue.Enqueue(filter);
			model.ChangeElementValue(queue, newPropertyValue);
		}

		/// <summary>
		/// Change Element Value
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <param name="newPropertyValue"></param>
		static public void ChangeElementValue(this SModel model, Queue<string> filter, string newPropertyValue)
		{
			var foundItems = model.GetElements(filter);
			foreach (var item in foundItems)
			{
				item.ChangeElementValue(newPropertyValue);
			}
		}

		/// <summary>
		/// Change Element Name
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <param name="newPropertyName"></param>
		static public void ChangeElementName(this SModel model, string filter, string newPropertyName)
		{
			var queue = new Queue<string>();
			queue.Enqueue(filter);
			model.ChangeElementName(queue, newPropertyName);
		}

		/// <summary>
		/// Change Element Name
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <param name="newPropertyName"></param>
		static public void ChangeElementName(this SModel model, Queue<string> filter, string newPropertyName)
		{
			var foundItems = model.GetElements(filter);
			foreach (var item in foundItems)
			{
				item.ChangeElementPropertyName(filter.Last(), newPropertyName);
			}
		}

		/// <summary>
		/// GetElements
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Rename Key
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dic"></param>
		/// <param name="fromKey"></param>
		/// <param name="toKey"></param>
		public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
									  TKey fromKey, TKey toKey)
		{
			TValue value = dic[fromKey];
			dic.Remove(fromKey);
			dic[toKey] = value;
		}
	}
}
