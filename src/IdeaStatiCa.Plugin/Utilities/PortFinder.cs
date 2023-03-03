using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IdeaStatiCa.Plugin.Utilities
{
    /// <summary>
    /// Tool used to find a free port for gRPC communication.
    /// </summary>
    public static class PortFinder
    {
        /// <summary>
        /// Searches for the next available port.
        /// <paramref name="startPort"/> Initial port to start the search from.
        /// </summary>
		/// <param name="usedPort">Port that was already generated for use but not assigned</param>
        /// <returns></returns>
        public static int FindPort(int minPort, int maxPort, int? usedPort = null)
        {
            if (maxPort < minPort)
                throw new ArgumentException("Max cannot be less than min.");

            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            var usedPorts =
                ipProperties.GetActiveTcpConnections()
                    .Where(connection => connection.State != TcpState.Closed)
                    .Select(connection => connection.LocalEndPoint)
                    .Concat(ipProperties.GetActiveTcpListeners())
                    .Concat(ipProperties.GetActiveUdpListeners())
                    .Select(endpoint => endpoint.Port)
                    .ToArray();

			var allUnused =
				Enumerable.Range(minPort, maxPort - minPort)
					.Where(port => !usedPorts.Contains(port))
					.Select(port => new int?(port));

			var firstUnused =allUnused
				.Where(port => port != usedPort)
				.FirstOrDefault();

			if (!firstUnused.HasValue)
                throw new Exception($"All local TCP ports between {minPort} and {maxPort} are currently in use.");

            return firstUnused.Value;
        }
    }
}
