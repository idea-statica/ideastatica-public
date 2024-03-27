using System;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public static class KeyValueStorageExtensions
	{
		public static Task Set(this IKeyValueStorage storage, string key, short value)
		{
			return storage.Set(key, BitConverter.GetBytes(value));
		}

		public static Task Set(this IKeyValueStorage storage, string key, int value)
		{
			return storage.Set(key, BitConverter.GetBytes(value));
		}

		public static Task Set(this IKeyValueStorage storage, string key, long value)
		{
			return storage.Set(key, BitConverter.GetBytes(value));
		}

		public static Task Set(this IKeyValueStorage storage, string key, float value)
		{
			return storage.Set(key, BitConverter.GetBytes(value));
		}

		public static Task Set(this IKeyValueStorage storage, string key, double value)
		{
			return storage.Set(key, BitConverter.GetBytes(value));
		}

		public static Task Set(this IKeyValueStorage storage, string key, bool value)
		{
			return storage.Set(key, BitConverter.GetBytes(value));
		}

		public static Task Set(this IKeyValueStorage storage, string key, char value)
		{
			return storage.Set(key, BitConverter.GetBytes(value));
		}

		public static Task Set(this IKeyValueStorage storage, string key, string value)
		{
			return storage.Set(key, Encoding.UTF8.GetBytes(value));
		}
#if NET5_0_OR_GREATER
		public static async Task<short> GetShort(this IKeyValueStorage storage, string key)
		{
			ReadOnlyMemory<byte> data = await storage.Get(key);
			return BitConverter.ToInt16(data.Span);
		}

		public static async Task<int> GetInt(this IKeyValueStorage storage, string key)
		{
			ReadOnlyMemory<byte> data = await storage.Get(key);
			return BitConverter.ToInt32(data.Span);
		}

		public static async Task<long> GetLong(this IKeyValueStorage storage, string key)
		{
			ReadOnlyMemory<byte> data = await storage.Get(key);
			return BitConverter.ToInt64(data.Span);
		}

		public static async Task<float> GetFloat(this IKeyValueStorage storage, string key)
		{
			ReadOnlyMemory<byte> data = await storage.Get(key);
			return BitConverter.ToSingle(data.Span);
		}

		public static async Task<double> GetDouble(this IKeyValueStorage storage, string key)
		{
			ReadOnlyMemory<byte> data = await storage.Get(key);
			return BitConverter.ToDouble(data.Span);
		}

		public static async Task<bool> GetBool(this IKeyValueStorage storage, string key)
		{
			ReadOnlyMemory<byte> data = await storage.Get(key);
			return BitConverter.ToBoolean(data.Span);
		}

		public static async Task<char> GetChar(this IKeyValueStorage storage, string key)
		{
			ReadOnlyMemory<byte> data = await storage.Get(key);
			return BitConverter.ToChar(data.Span);
		}

		public static async Task<string> GetString(this IKeyValueStorage storage, string key)
		{
			ReadOnlyMemory<byte> data = await storage.Get(key);
			return Encoding.UTF8.GetString(data.Span);
		}
#endif
	}
}