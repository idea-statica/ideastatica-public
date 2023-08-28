using AutoMapper;
using Grpc.Core;
using IdeaRS.OpenModel;
using IdeaStatiCa.CheckbotPlugin.Common;
using IdeaStatiCa.CheckbotPlugin.Models;
using IdeaStatiCa.CheckbotPlugin.Services;
using System.IO.Pipelines;
using System.Text;
using System.Xml.Serialization;
using Protos = IdeaStatiCa.CheckbotPlugin.Protos;

namespace IdeaStatiCa.PluginRunner.Services
{
	public class ProjectService : IProjectService
	{
		private static readonly Mapper _mapper = Mapping.GetMapper();

		private readonly Protos.ProjectService.ProjectServiceClient _client;

		public ProjectService(Protos.ProjectService.ProjectServiceClient client)
		{
			_client = client;
		}

		public async Task<ProjectInfo> GetInfo()
		{
			Protos.ProjectInfoReq reg = new();
			Protos.ProjectInfoResp resp = await _client.GetInfoAsync(reg);

			return new ProjectInfo(resp.Name, CountryCode.ECEN);
		}

		public async Task<OpenModelContainer> GetModel(ModelExportOptions options)
		{
			Protos.GetModelReq reg = new()
			{
				Options = _mapper.Map<Protos.ModelExportOptions>(options)
			};

			return await GetOpenModelContainer(_client.GetModel(reg), x => x.Packet);
		}

		public async Task<OpenModelContainer> GetObjects(IEnumerable<ModelObject> objects, ModelExportOptions options)
		{
			Ensure.NotNull(objects);

			Protos.GetObjectsReq reg = new()
			{
				Options = _mapper.Map<Protos.ModelExportOptions>(options)
			};
			reg.Objects.AddRange(_mapper.Map<List<Protos.ModelObject>>(objects));

			return await GetOpenModelContainer(_client.GetObjects(reg), x => x.Packet);
		}

		public async Task<IReadOnlyCollection<ModelObject>> ListObjects()
		{
			Protos.ListObjectsRes resp = await _client.ListObjectsAsync(new());
			return resp.Objects
				.Select(x => _mapper.Map<ModelObject>(x))
				.ToList();
		}

		private static async Task<OpenModelContainer> GetOpenModelContainer<T>(AsyncServerStreamingCall<T> resp, Func<T, Protos.ModelPacket> converter, CancellationToken cancellationToken = default)
		{
			OpenModelContainer openModel = await GetOpenModelContainerFromPackets(
				resp.ResponseStream
					.ReadAllAsync(cancellationToken)
					.Select(converter), cancellationToken);

			openModel.OpenModel.ReferenceElementsReconstruction();

			return openModel;
		}

		private static async Task<OpenModelContainer> GetOpenModelContainerFromPackets(
			IAsyncEnumerable<Protos.ModelPacket> packets,
			CancellationToken cancellationToken = default)
		{
			Pipe pipe = new();
			PipeWriter pipeWriter = pipe.Writer;
			PipeReader pipeReader = pipe.Reader;

			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

			Task readTask = Task.Run(async () =>
			{
				await foreach (Protos.ModelPacket packet in packets.WithCancellation(cancellationTokenSource.Token))
				{
					Memory<byte> memory = pipeWriter.GetMemory(packet.Data.Length);
					packet.Data.Memory.CopyTo(memory);
					pipeWriter.Advance(packet.Data.Length);

					await pipeWriter.FlushAsync(cancellationTokenSource.Token);
				}

				await pipeWriter.CompleteAsync();
			}, cancellationTokenSource.Token);

			OpenModelContainer? model = null;

			Task xmlTask = Task.Run(() =>
			{
				using StreamReader reader = new(pipeReader.AsStream(), Encoding.UTF8);

				XmlSerializer serializer = new(typeof(OpenModelContainer));
				model = (OpenModelContainer?)serializer.Deserialize(reader);
			}, cancellationTokenSource.Token);

			try
			{
				await Task.WhenAll(readTask, xmlTask);

				if (model is null)
				{
					throw new InvalidOperationException("Failed to deserialize OpenModelContainer");
				}

				return model;
			}
			catch
			{
				cancellationTokenSource.Cancel();
				throw;
			}
		}
	}
}