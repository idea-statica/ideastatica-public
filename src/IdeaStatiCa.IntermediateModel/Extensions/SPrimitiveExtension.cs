using IdeaStatiCa.IntermediateModel.IRModel;

namespace IdeaStatiCa.IntermediateModel.Extensions
{
	public static class SPrimitiveExtension
	{
		public static void AddElementProperty(this SPrimitive sPrimitive, ISIntermediate property)
		{
			throw new NotImplementedException();
		}

		public static bool TryAddElementProperty(this SPrimitive sPrimitive, ISIntermediate property)
		{
			throw new NotImplementedException();
		}

		public static IEnumerable<ISIntermediate> GetElements(this SPrimitive sPrimitive, Queue<string> filter)
		{
			return new List<ISIntermediate>() { };
		}

		public static string GetElementName(this SPrimitive sPrimitive)
		{
			return "Value";
		}

		public static string GetElementValue(this SPrimitive sPrimitive, string property = null)
		{
			return sPrimitive.Value;
		}

		public static void ChangeElementPropertyName(this SPrimitive sPrimitive, string name, string newName)
		{
			throw new InvalidOperationException("SPrimitive can not be renamed");
		}

		public static void ChangeElementValue(this SPrimitive sPrimitive, string newValue)
		{
			sPrimitive.Value = newValue;
		}
	}
}
