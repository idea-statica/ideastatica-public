using IdeaStatiCa.BimApi;
using System.Reflection;
using System.Runtime.Serialization;

namespace IdeaStatiCa.BimImporter.Persistence
{
	public abstract class AbstractPersistenceToken : IIdeaPersistenceToken
	{
		public enum ObjectType
		{
			Member,
			Node
		};

		public ObjectType Type { get; set; }

		protected AbstractPersistenceToken(SerializationInfo info, StreamingContext context)
		{
			foreach (PropertyInfo propertyInfo in GetType().GetProperties())
			{
				object value = info.GetValue(propertyInfo.Name, propertyInfo.PropertyType);
				propertyInfo.SetValue(this, value);
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (PropertyInfo propertyInfo in GetType().GetProperties())
			{
				info.AddValue(propertyInfo.Name, propertyInfo.GetValue(this));
			}
		}
	}
}