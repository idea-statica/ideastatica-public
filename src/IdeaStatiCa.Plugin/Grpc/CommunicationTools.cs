using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace IdeaStatiCa.Plugin.Grpc
{
	internal class CommunicationTools
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bufferSize"></param>
		/// <returns></returns>
		internal static List<ChannelOption> GetChannelOptions(int bufferSize = Constants.GRPC_MAX_MSG_SIZE)
		{
			var res = new List<ChannelOption>() {
						new ChannelOption(ChannelOptions.MaxReceiveMessageLength, bufferSize),
						new ChannelOption(ChannelOptions.MaxSendMessageLength, bufferSize)
				};
			return res;
		}

		/// <summary>
		/// Compress <paramref name="stringToCompress"/> end return encoded base-64 digits
		/// </summary>
		/// <param name="stringToCompress">String to compress</param>
		/// <returns>Base-64 encoded digits including compressed <paramref name="stringToCompress"/></returns>
		public static string Zip(string stringToCompress)
		{
			var bytes = Encoding.Unicode.GetBytes(stringToCompress);

			using (var msi = new MemoryStream(bytes))
			{
				using (var mso = new MemoryStream())
				{
					using (var gs = new GZipStream(mso, CompressionMode.Compress))
					{
						msi.CopyTo(gs);
					}

					var buffer = mso.ToArray();
					return Convert.ToBase64String(buffer);
				}
			}
		}

		/// <summary>
		/// Decode <paramref name="base64Data"/> and decompress them
		/// </summary>
		/// <param name="base64Data">Data to be decompressed</param>
		/// <returns>Decompressed string</returns>
		public static string Unzip(string base64Data)
		{
			byte[] compressedData = Convert.FromBase64String(base64Data);

			using (var msi = new MemoryStream(compressedData))
			{
				using (var mso = new MemoryStream())
				{
					using (var gs = new GZipStream(msi, CompressionMode.Decompress))
					{
						gs.CopyTo(mso);
					}

					return Encoding.Unicode.GetString(mso.ToArray());
				}
			}
		}
	}
}