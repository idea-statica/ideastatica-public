using IdeaStatiCa.IntermediateModel.IRModel;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	public static class SModelExtensions
	{
		/// <summary>
		/// Change Element Value
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <param name="newPropertyValue"></param>
		public static void ChangeElementValue(this SModel model, string filter, string newPropertyValue)
		{
			Queue<string> queue = new Queue<string>();
			queue.Enqueue(filter);
			model.ChangeElementValue(queue, newPropertyValue);
		}

		/// <summary>
		/// Change Element Value
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <param name="newPropertyValue"></param>
		public static void ChangeElementValue(this SModel model, Queue<string> filter, string newPropertyValue)
		{
			IEnumerable<ISIntermediate> foundItems = model.GetElements(filter);
			foreach (ISIntermediate item in foundItems)
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
		public static void ChangeElementName(this SModel model, string filter, string newPropertyName)
		{
			Queue<string> queue = new Queue<string>();
			queue.Enqueue(filter);
			model.ChangeElementName(queue, newPropertyName);
		}

		/// <summary>
		/// Change Element Name
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <param name="newPropertyName"></param>
		public static void ChangeElementName(this SModel model, Queue<string> filter, string newPropertyName)
		{
			IEnumerable<ISIntermediate> foundItems = model.GetElements(filter);
			foreach (ISIntermediate item in foundItems)
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
		private static IEnumerable<ISIntermediate> GetElements(this SModel model, Queue<string> filter)
		{
			return model.RootItem.GetElements(filter);
		}

		/// <summary>
		/// Find and keep
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static IEnumerable<ISIntermediate> GetElements(this SModel model, string filter)
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

		/// <summary>
		/// Find and keep
		/// </summary>
		/// <param name="model"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static bool TryGetFirstElement(this SModel model, string filter, out ISIntermediate element)
		{
			element = model.RootItem.GetElements(filter).FirstOrDefault();

			if (element is null)
			{
				return false;
			}

			return true;
		}
	}
}
