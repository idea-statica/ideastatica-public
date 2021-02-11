using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Grpc
{
    /// <summary>
    /// Handles communication over Grpc.
    /// </summary>
    public interface IGrpcMessageHandler<T> : IGrpcMessageHandler where T:class
    {
        /// <summary>
        /// Handles request incoming from the client.
        /// </summary>
        /// <param name="message">Message sent by client.</param>
        /// <returns></returns>
        new Task<T> HandleServerMessage(GrpcMessage message, GrpcServer server);

        /// <summary>
        /// Handles response incoming from the server.
        /// </summary>
        /// <param name="message">Message sent by server.</param>
        /// <returns></returns>
        new Task<T> HandleClientMessage(GrpcMessage message, GrpcClient client);
    }

    /// <summary>
    /// Non generic implementation of <see cref="IGrpcMessageHandler{T}"/>
    /// </summary>
    public interface IGrpcMessageHandler
    {
        /// <summary>
        /// Handles request incoming from the client.
        /// </summary>
        /// <param name="message">Message sent by client.</param>
        /// <returns></returns>
        Task<object> HandleServerMessage(GrpcMessage message, GrpcServer server);

        /// <summary>
        /// Handles response incoming from the server.
        /// </summary>
        /// <param name="message">Message sent by server.</param>
        /// <returns></returns>
        Task<object> HandleClientMessage(GrpcMessage message, GrpcClient client);
    }
}
