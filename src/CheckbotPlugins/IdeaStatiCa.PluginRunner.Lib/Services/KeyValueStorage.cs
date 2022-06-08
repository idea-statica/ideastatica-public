using Google.Protobuf;
using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner.Utils;

using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class KeyValueStorage : IKeyValueStorage
	{
		private readonly Protos.StorageService.StorageServiceClient _client;

		public KeyValueStorage(Protos.StorageService.StorageServiceClient client)
		{
			_client = client;
		}

		public async Task<bool> Delete(string key)
		{
			Ensure.ArgNotEmpty(key);

			Protos.DeleteReq reg = new()
			{
				Key = key
			};
			Protos.DeleteResp resp = await _client.DeleteAsync(reg);

			return resp.Success;
		}

		public async Task<bool> Exists(string key)
		{
			Ensure.ArgNotEmpty(key);

			Protos.ExistsReq reg = new()
			{
				Key = key
			};
			Protos.ExistsResp resp = await _client.ExistsAsync(reg);

			return resp.Exists;
		}

		public async Task<ReadOnlyMemory<byte>?> Get(string key)
		{
			Ensure.ArgNotEmpty(key);

			Protos.GetReq reg = new()
			{
				Key = key
			};
			Protos.GetResp resp = await _client.GetAsync(reg);

			if (!resp.Success)
			{
				return null;
			}

			return resp.Value.ToByteArray();
		}

		public async Task Set(string key, ReadOnlyMemory<byte> value)
		{
			Ensure.ArgNotEmpty(key);

			if (value.Length == 0)
			{
				throw new ArgumentException("Value cannot be zero-length.");
			}

			Protos.SetReq reg = new()
			{
				Key = key,
				Value = ByteString.CopyFrom(value.Span)
			};
			Protos.SetResp resp = await _client.SetAsync(reg);

			if (!resp.Success)
			{
				throw new Exception();
			}
		}
	}
}