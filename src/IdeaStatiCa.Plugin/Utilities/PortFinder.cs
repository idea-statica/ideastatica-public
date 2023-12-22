using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace IdeaStatiCa.Plugin.Utilities
{
	/// <summary>
	/// Tool used to find a free port for gRPC communication.
	/// </summary>
	public static class PortFinder
	{

		/// <summary>
		/// Searches for the next available tcp port on the localhost.
		/// </summary>
		/// <param name="minPort">Initial port to start the search from.</param>
		/// <param name="maxPort">The last available tcp port</param>
		/// <param name="pluginLogger">Optional logger</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="Exception"></exception>
		public static int FindPort(int minPort, int maxPort, IPluginLogger pluginLogger = null)
		{
			if(pluginLogger != null)
			{
				pluginLogger.LogDebug($"PortFinder.FindPort minport = {minPort} maxport = {maxPort}");
			}

			if (maxPort < minPort)
			{
				throw new ArgumentException("Max cannot be less than min.");
			}

			var ipProperties = IPGlobalProperties.GetIPGlobalProperties();

			var activeTcpConnections = ipProperties.GetActiveTcpConnections();

			var usedPorts =
				activeTcpConnections
					.Where(connection => connection.State != TcpState.Closed)
					.Select(connection => connection.LocalEndPoint)
					.Concat(ipProperties.GetActiveTcpListeners())
					.Concat(ipProperties.GetActiveUdpListeners())
					.Select(endpoint => endpoint.Port)
					.ToArray();

			if (pluginLogger != null)
			{
				pluginLogger.LogDebug($"PortFinder.FindPort {usedPorts.Length} ports are currently used");
			}

			var allUnused =
				Enumerable.Range(minPort, maxPort - minPort)
					.Where(port => !usedPorts.Contains(port))
					.Select(port => new int?(port));

			var firstUnused = allUnused
				.FirstOrDefault();

			if (!firstUnused.HasValue)
			{
				if (pluginLogger != null)
				{
					foreach (var connection in activeTcpConnections)
					{
						pluginLogger.LogTrace($"Local {connection.LocalEndPoint.Address}:{connection.LocalEndPoint.Port} Remote {connection.RemoteEndPoint.Address}:{connection.RemoteEndPoint.Port}");
					}
				}

				throw new Exception($"All local TCP ports between {minPort} and {maxPort} are currently in use.");
			}

			return firstUnused.Value;
		}
	}
}
