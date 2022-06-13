using MessagePack;
using System.Buffers;

namespace IdeaStatiCa.CheckbotPlugin.Services
{
	public static class KeyValueStorageExtensions
	{
		public static Task Set<T>(this IKeyValueStorage storage, string key, T value, bool compressed = false)
		{
			MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard;

			if (compressed)
			{
				options = options.WithCompression(MessagePackCompression.Lz4BlockArray);
			}

			ArrayBufferWriter<byte> writer = new();
			MessagePackSerializer.Serialize(writer, value, options);

			return storage.Set(key, writer.WrittenMemory);
		}

		public static async Task<T?> Get<T>(this IKeyValueStorage storage, string key, bool compressed = false)
		{
			ReadOnlyMemory<byte>? value = await storage.Get(key).ConfigureAwait(false);
			if (!value.HasValue)
			{
				return default;
			}

			MessagePackSerializerOptions options = MessagePackSerializerOptions.Standard;

			if (compressed)
			{
				options = options.WithCompression(MessagePackCompression.Lz4BlockArray);
			}

			return MessagePackSerializer.Deserialize<T>(value.Value, options);
		}
	}
}