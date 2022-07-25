﻿using Google.Protobuf;
using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.CheckbotPlugin.Services;

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
			Ensure.NotEmpty(key);

			Protos.DeleteReq reg = new()
			{
				Key = key
			};
			Protos.DeleteResp resp = await _client.DeleteAsync(reg)
				.ConfigureAwait(false);

			return resp.Success;
		}

		public async Task<bool> Exists(string key)
		{
			Ensure.NotEmpty(key);

			Protos.ExistsReq reg = new()
			{
				Key = key
			};
			Protos.ExistsResp resp = await _client.ExistsAsync(reg)
				.ConfigureAwait(false);

			return resp.Exists;
		}

		public async Task<ReadOnlyMemory<byte>> Get(string key)
		{
			Ensure.NotEmpty(key);

			Protos.GetReq reg = new()
			{
				Key = key
			};
			Protos.GetResp resp = await _client.GetAsync(reg)
				.ConfigureAwait(false);

			if (!resp.Success)
			{
				throw new KeyNotFoundException($"Key '{key}' does not exist.");
			}

			return resp.Value.ToByteArray();
		}

		public async Task Set(string key, ReadOnlyMemory<byte> value)
		{
			Ensure.NotEmpty(key);

			if (value.Length == 0)
			{
				throw new ArgumentException("Value cannot be zero-length.");
			}

			Protos.SetReq reg = new()
			{
				Key = key,
				Value = ByteString.CopyFrom(value.Span)
			};

			await _client.SetAsync(reg)
				.ConfigureAwait(false);
		}
	}
}