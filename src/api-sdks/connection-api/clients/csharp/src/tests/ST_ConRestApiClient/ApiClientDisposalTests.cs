using FluentAssertions;
using IdeaStatiCa.ConnectionApi.Client;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace ST_ConRestApiClient
{
	/// <summary>
	/// Tests that ApiClient properly reuses a single RestClient instance
	/// instead of creating one per request (which causes TCP port exhaustion).
	/// Bug #32846: TCP port exhaustion from disposed HttpClient/RestClient instances
	/// leaving sockets in TIME_WAIT state.
	/// </summary>
	[TestFixture]
	public class ApiClientDisposalTests
	{
		[Test]
		public void ApiClient_Should_Implement_IDisposable()
		{
			// ApiClient must implement IDisposable so that callers can dispose
			// the underlying RestClient when they are done, preventing socket leaks.
			typeof(ApiClient).Should().Implement<IDisposable>(
				"ApiClient owns an HttpClient via RestClient and must be disposable to prevent TCP port exhaustion");
		}

		[Test]
		public void ApiClient_Should_Have_RestClient_Field_For_Reuse()
		{
			// ApiClient should hold a RestClient as a field (not create one per request).
			// This ensures socket reuse across HTTP calls.
			var restClientFields = typeof(ApiClient)
				.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(f => f.FieldType.Name == "RestClient")
				.ToList();

			restClientFields.Should().NotBeEmpty(
				"ApiClient should hold a RestClient field for reuse across requests, " +
				"instead of creating a new RestClient per request (causes TCP port exhaustion)");
		}

		[Test]
		public void ApiClient_Dispose_Should_Dispose_RestClient()
		{
			// When ApiClient is disposed, the underlying RestClient should also be disposed.
			var client = new ApiClient("http://localhost:5000");

			var disposable = client as IDisposable;
			disposable.Should().NotBeNull("ApiClient must implement IDisposable");

			// Should not throw when disposing
			Action dispose = () => disposable!.Dispose();
			dispose.Should().NotThrow("disposing ApiClient should cleanly dispose the underlying RestClient");
		}
	}
}
