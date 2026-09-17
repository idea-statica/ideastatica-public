using IdeaStatiCa.ConnectionApi;
using IdeaStatiCa.ConnectionApi.Client;
using System.Net;

namespace ST_ConRestApiClient
{
	/// <summary>
	/// The exception factory every API of <see cref="ConnectionApiClient"/> is wired to. Offline: it
	/// decides from a response object alone, so no service is needed to pin what it does and - more
	/// importantly - what it leaves alone.
	/// </summary>
	[TestFixture]
	public class IdeaApiExceptionFactoryTests
	{
		private const string SomeJson = "{\"beams\":[{\"plates\":[{\"material\":{\"id\":1}}]}]}";

		/// <summary>A response whose body did not become an object - the case under test.</summary>
		private static ApiResponse<T> Unread<T>(HttpStatusCode status, string rawContent,
			string? errorText = null) where T : class
			=> new ApiResponse<T>(status, new Multimap<string, string>(), default!, rawContent)
			{
				ErrorText = errorText,
			};

		/// <summary>A response that read into a value - nothing for the factory to report.</summary>
		private static ApiResponse<T> Read<T>(HttpStatusCode status, T data, string rawContent)
			=> new ApiResponse<T>(status, new Multimap<string, string>(), data, rawContent);

		[Test]
		public void SuccessWithABodyThatDidNotDeserialise_Throws()
		{
			// what a 26.0 client gets from a 26.1 service on export-iom-connection-data
			var response = Unread<object>(HttpStatusCode.OK, SomeJson,
				"Unexpected character encountered while parsing value: {. Path 'beams[0].plates[0].material'");

			var exception = IdeaApiExceptionFactory.Instance("ExportIomConnectionData", response);

			Assert.That(exception, Is.InstanceOf<ApiException>());
			Assert.Multiple(() =>
			{
				// the method, the reason the deserializer gave, and the direction to look in
				Assert.That(exception!.Message, Does.Contain("ExportIomConnectionData"));
				Assert.That(exception.Message, Does.Contain("beams[0].plates[0].material"));
				Assert.That(exception.Message, Does.Contain("version mismatch"));
				// the body is kept, so a caller that logs the exception can see what arrived
				Assert.That(((ApiException)exception).ErrorContent, Is.EqualTo(SomeJson));
			});
		}

		[Test]
		public void SuccessWithABodyThatDidNotDeserialise_AndNoReasonReported_StillThrows()
		{
			var response = Unread<object>(HttpStatusCode.OK, SomeJson);

			var exception = IdeaApiExceptionFactory.Instance("ExportIomConnectionData", response);

			Assert.That(exception, Is.InstanceOf<ApiException>());
			// the length is what separates this from an empty answer
			Assert.That(exception!.Message, Does.Contain($"{SomeJson.Length} characters"));
		}

		/// <summary>
		/// The legitimate nulls. Each of these is an answer, not a failure, and turning any of them
		/// into an exception would break working code - which is the risk this hook carries.
		/// </summary>
		[Test]
		public void SuccessWithNothingToRead_DoesNotThrow()
		{
			Assert.Multiple(() =>
			{
				Assert.That(IdeaApiExceptionFactory.Instance("GetTemplate",
					Unread<object>(HttpStatusCode.OK, "null")), Is.Null, "the JSON literal null");

				Assert.That(IdeaApiExceptionFactory.Instance("GetTemplate",
					Unread<object>(HttpStatusCode.OK, "")), Is.Null, "an empty body");

				Assert.That(IdeaApiExceptionFactory.Instance("GetTemplate",
					Unread<object>(HttpStatusCode.OK, "   ")), Is.Null, "whitespace only");

				Assert.That(IdeaApiExceptionFactory.Instance("DeleteMember",
					Unread<object>(HttpStatusCode.NoContent, "")), Is.Null, "204 No Content");

				Assert.That(IdeaApiExceptionFactory.Instance("GetConnection",
					Read(HttpStatusCode.OK, "read fine", SomeJson)), Is.Null, "a body that read");
			});
		}

		/// <summary>
		/// A response typed as a non-nullable value type cannot be judged: a body that fails to convert
		/// leaves 0 or false, which is indistinguishable from a real answer. Documented here so the
		/// limitation is a decision rather than an oversight.
		/// </summary>
		[Test]
		public void SuccessWithAValueTypeResponse_IsOutOfScope()
		{
			Assert.That(IdeaApiExceptionFactory.Instance("GetCount",
				Read(HttpStatusCode.OK, 0, SomeJson)), Is.Null);
		}

		[Test]
		public void FailedRequests_KeepTheGeneratedBehaviour()
		{
			var notFound = Unread<object>(HttpStatusCode.NotFound, "{\"title\":\"Not Found\"}");
			var transportFailure = Unread<object>((HttpStatusCode)0, "", "no connection");

			var fromNotFound = IdeaApiExceptionFactory.Instance("GetConnection", notFound);
			var fromTransport = IdeaApiExceptionFactory.Instance("GetConnection", transportFailure);

			Assert.Multiple(() =>
			{
				// the generated factory's own message, unchanged: callers already handle these
				Assert.That(fromNotFound, Is.InstanceOf<ApiException>());
				Assert.That(((ApiException)fromNotFound!).ErrorCode, Is.EqualTo(404));
				Assert.That(fromNotFound.Message, Does.Contain("Not Found"));

				Assert.That(fromTransport, Is.InstanceOf<ApiException>());
				Assert.That(((ApiException)fromTransport!).ErrorCode, Is.EqualTo(0));
				Assert.That(fromTransport.Message, Does.Contain("no connection"));
			});
		}
	}
}
