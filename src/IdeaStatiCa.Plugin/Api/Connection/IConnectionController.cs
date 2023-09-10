﻿using IdeaStatiCa.Public;
using System;

namespace IdeaStatiCa.Plugin
{
	/// <summary>
	/// Automation of IdeaConnection app
	/// </summary>
	public interface IConnectionController
	{
		/// <summary>
		/// Fires when IdeaConnection app exited.
		/// </summary>
		event EventHandler ConnectionAppExited;

		/// <summary>
		/// Indicates whether communication with IdeaConnection.exe is alive.
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Open idea con project in IdeaConnection
		/// </summary>
		/// <param name="fileName">The file path to the ideaCon.</param>
		/// <returns></returns>
		int OpenProject(string fileName);

		/// <summary>
		/// Close the open project
		/// </summary>
		/// <returns></returns>
		int CloseProject();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="conId"></param>
		/// <returns></returns>
		IBlobStorage GenerateReport(int conId);
	}
}
