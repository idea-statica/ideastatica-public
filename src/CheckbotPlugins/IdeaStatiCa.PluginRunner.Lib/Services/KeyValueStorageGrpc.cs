using Google.Protobuf;
using IdeaStatiCa.CheckbotPlugin.Protos;
using IdeaStatiCa.CheckbotPlugin.Services;
using IdeaStatiCa.PluginRunner.Utils;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class KeyValueStorageGrpc : IKeyValueStorage
	{
		private readonly StorageService.StorageServiceClient _client;

		public KeyValueStorageGrpc(StorageService.StorageServiceClient client)
		{
			_client = client;
		}

		public async Task<bool> Delete(string key)
		{
			Ensure.ArgNotEmpty(key);

			DeleteReq reg = new()
			{
				Key = key
			};
			DeleteResp resp = await _client.DeleteAsync(reg);

			return resp.Success;
		}

		public async Task<bool> Exists(string key)
		{
			Ensure.ArgNotEmpty(key);

			ExistsReq reg = new()
			{
				Key = key
			};
			ExistsResp resp = await _client.ExistsAsync(reg);

			return resp.Exists;
		}

		public async Task<ReadOnlyMemory<byte>?> Get(string key)
		{
			Ensure.ArgNotEmpty(key);

			GetReq reg = new()
			{
				Key = key
			};
			GetResp resp = await _client.GetAsync(reg);

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

			SetReq reg = new()
			{
				Key = key,
				Value = ByteString.CopyFrom(value.Span)
			};
			SetResp resp = await _client.SetAsync(reg);

			if (!resp.Success)
			{
				throw new Exception();
			}
		}
	}
}