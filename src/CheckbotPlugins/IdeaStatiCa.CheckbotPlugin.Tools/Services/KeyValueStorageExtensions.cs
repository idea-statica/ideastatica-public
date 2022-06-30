using IdeaStatiCa.CheckbotPlugin.Common;
using System.Text;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public static class KeyValueStorageExtensions
	{
		public static Task Set(this IKeyValueStorage storage, string key, short value)
			=> storage.Set(key, BitConverter.GetBytes(value));

		public static Task Set(this IKeyValueStorage storage, string key, int value)
			=> storage.Set(key, BitConverter.GetBytes(value));

		public static Task Set(this IKeyValueStorage storage, string key, long value)
			=> storage.Set(key, BitConverter.GetBytes(value));

		public static Task Set(this IKeyValueStorage storage, string key, float value)
			=> storage.Set(key, BitConverter.GetBytes(value));

		public static Task Set(this IKeyValueStorage storage, string key, double value)
			=> storage.Set(key, BitConverter.GetBytes(value));

		public static Task Set(this IKeyValueStorage storage, string key, bool value)
			=> storage.Set(key, BitConverter.GetBytes(value));

		public static Task Set(this IKeyValueStorage storage, string key, char value)
			=> storage.Set(key, BitConverter.GetBytes(value));

		public static Task Set(this IKeyValueStorage storage, string key, string value)
			=> storage.Set(key, Encoding.UTF8.GetBytes(value));

		public static Task<short> GetShort(this IKeyValueStorage storage, string key)
			=> storage.Get(key).Then(x => BitConverter.ToInt16(x.Span));

		public static Task<int> GetInt(this IKeyValueStorage storage, string key)
			=> storage.Get(key).Then(x => BitConverter.ToInt32(x.Span));

		public static Task<long> GetLong(this IKeyValueStorage storage, string key)
			=> storage.Get(key).Then(x => BitConverter.ToInt64(x.Span));

		public static Task<float> GetFloat(this IKeyValueStorage storage, string key)
			=> storage.Get(key).Then(x => BitConverter.ToSingle(x.Span));

		public static Task<double> GetDouble(this IKeyValueStorage storage, string key)
			=> storage.Get(key).Then(x => BitConverter.ToDouble(x.Span));

		public static Task<bool> GetBool(this IKeyValueStorage storage, string key)
			=> storage.Get(key).Then(x => BitConverter.ToBoolean(x.Span));

		public static Task<char> GetChar(this IKeyValueStorage storage, string key)
			=> storage.Get(key).Then(x => BitConverter.ToChar(x.Span));

		public static Task<string> GetString(this IKeyValueStorage storage, string key)
			=> storage.Get(key).Then(x => Encoding.UTF8.GetString(x.Span));
	}
}