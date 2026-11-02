using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace IdeaStatiCa.Plugin
{
	public abstract class ConversionDictionary<TKey> : SerializableDictionary<TKey, int>
	{
		private int maxid = -1;

		public void Merge(IList<ProjectItem> source)
		{
			if (source != null)
			{
				var modified = false;
				foreach (var id in source.Select(x => x.Identifier).Except(Values))
				{
					this[GetKey(id)] = id;
					modified = true;
				}

				if (modified)
				{
					UpdateMaxId();
				}
			}
		}

		public void InitMerge(IList<ProjectItem> source)
		{
			if (source != null)
			{
				foreach (var item in source)
				{
					this[GetKey(item.Name)] = item.Identifier;
				}
				UpdateMaxId();
			}
		}

		public int MapId(TKey key)
		{
			if (TryGetValue(key, out int value))
			{
				return value;
			}

			if (maxid < 0)
			{
				UpdateMaxId();
			}

			var newId = ++maxid;
			Add(key, newId);
			return newId;
		}

		public int GetExEntityId(TKey name)//, ref bool add)
		{
			int id;
			int prevId = -1;
			//add = false;
			if (TryGetValue(name, out id))
			{
				prevId = (int)id;
			}
			return prevId;
		}

		public abstract TKey GetSaveKey(int Id);//, ref bool add)

												//{
												//	TKey key;
												//	foreach (KeyValuePair<TKey, int> obj in this)
												//	{
												//		if ((int)(obj.Value) == Id)
												//		{
												//			key = (obj.Key);
												//		}
												//	}

		//	return key;
		//}

		protected abstract TKey GetKey(int id);

		protected abstract TKey GetKey(string name);

		private void UpdateMaxId()
		{
			maxid = Values.DefaultIfEmpty(0).Max();
		}
	}

	public sealed class ConversionDictionaryInt : ConversionDictionary<int>
	{
		protected override int GetKey(int id)
		{
			return -id;
		}

		protected override int GetKey(string name)
		{
			return -1;
		}

		public override int GetSaveKey(int Id)
		{
			int key = -1;
			foreach (KeyValuePair<int, int> obj in this)
			{
				if ((int)(obj.Value) == Id)
				{
					key = (obj.Key);
				}
			}

			return key;
		}
	}

	public sealed class ConversionDictionaryString : ConversionDictionary<string>
	{
		protected override string GetKey(int id)
		{
			return $"{id}-IDEA";
		}

		protected override string GetKey(string name)
		{
			return name;
		}

		public override string GetSaveKey(int Id)
		{
			string key = "";
			foreach (KeyValuePair<string, int> obj in this)
			{
				if ((int)(obj.Value) == Id)
				{
					key = (obj.Key);
				}
			}

			return key;
		}
	}

	/// <summary>
	/// SerializableDictionary
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TVal"></typeparam>
	[Serializable]
	public class SerializableDictionary<TKey, TVal> : Dictionary<TKey, TVal>, IXmlSerializable, ISerializable
	{
		#region Constants

		private const string ItemNodeName = "Item";
		private const string KeyNodeName = "Key";
		private const string ValueNodeName = "Value";

		#endregion Constants

		#region Constructors

		public SerializableDictionary()
		{
		}

		public SerializableDictionary(IDictionary<TKey, TVal> dictionary)
			: base(dictionary)
		{
		}

		public SerializableDictionary(IEqualityComparer<TKey> comparer)
			: base(comparer)
		{
		}

		public SerializableDictionary(int capacity)
			: base(capacity)
		{
		}

		public SerializableDictionary(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer)
			: base(dictionary, comparer)
		{
		}

		public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
			: base(capacity, comparer)
		{
		}

		#endregion Constructors

		private XmlSerializer keySerializer;
		private XmlSerializer valueSerializer;

		protected SerializableDictionary(SerializationInfo info, StreamingContext context)
		{
			int itemCount = info.GetInt32("ItemCount");
			for (int i = 0; i < itemCount; i++)
			{
				var kvp =
						(KeyValuePair<TKey, TVal>)
						info.GetValue(String.Format("Item{0}", i), typeof(KeyValuePair<TKey, TVal>));
				Add(kvp.Key, kvp.Value);
			}
		}

		#region ISerializable Members

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ItemCount", Count);
			int itemIdx = 0;
			foreach (var kvp in this)
			{
				info.AddValue(String.Format("Item{0}", itemIdx), kvp, typeof(KeyValuePair<TKey, TVal>));
				itemIdx++;
			}
		}

		#endregion ISerializable Members

		#region IXmlSerializable Members

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			foreach (var kvp in this)
			{
				writer.WriteStartElement(ItemNodeName);
				writer.WriteStartElement(KeyNodeName);
				KeySerializer.Serialize(writer, kvp.Key);
				writer.WriteEndElement();
				writer.WriteStartElement(ValueNodeName);
				ValueSerializer.Serialize(writer, kvp.Value);
				writer.WriteEndElement();
				writer.WriteEndElement();
			}
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			if (reader.IsEmptyElement)
			{
				return;
			}

			// Move past container
			if (!reader.Read())
			{
				throw new XmlException("Error in Deserialization of Dictionary");
			}

			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement(ItemNodeName);
				reader.ReadStartElement(KeyNodeName);
				var key = (TKey)KeySerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadStartElement(ValueNodeName);
				var value = (TVal)ValueSerializer.Deserialize(reader);
				reader.ReadEndElement();
				reader.ReadEndElement();
				Add(key, value);
				reader.MoveToContent();
			}

			reader.ReadEndElement(); // Read End Element to close Read of containing node
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		#endregion IXmlSerializable Members

		#region Private Properties

		protected XmlSerializer ValueSerializer
		{
			get
			{
				if (valueSerializer == null)
				{
					//valueSerializer = XmlSerializer.FromTypes(new[] { typeof(TVal) })[0];
					valueSerializer = new XmlSerializer(typeof(TVal));
				}
				return valueSerializer;
			}
		}

		private XmlSerializer KeySerializer
		{
			get
			{
				if (keySerializer == null)
				{
					//keySerializer = XmlSerializer.FromTypes(new[] { typeof(TKey) })[0];
					keySerializer = new XmlSerializer(typeof(TKey));
				}
				return keySerializer;
			}
		}

		#endregion Private Properties
	}
}