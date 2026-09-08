using IdeaStatiCa.ConnectionApi.Client;
using System;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// The <see cref="ExceptionFactory"/> every API of <see cref="ConnectionApiClient"/> is wired to.
	///
	/// It keeps the generated behaviour for a failed request - <see cref="Configuration.DefaultExceptionFactory"/>
	/// raises for HTTP status 400 and above, and for a transport failure reported as status 0 - and adds
	/// the case that behaviour misses: a SUCCESSFUL status whose body this client cannot read.
	///
	/// How that happens: the response body is converted by the client's own deserializer, and RestSharp
	/// catches whatever the deserializer throws instead of letting it out - it lands in the response's
	/// error message. The generated code copies that into <see cref="IApiResponse.ErrorText"/>, but the
	/// default factory only looks at the status code, so the call returns <c>null</c> and throws nothing.
	/// The caller then cannot tell "the service has no data for this" from "this client cannot read what
	/// the service sent", which is the difference between an empty model and a version mismatch.
	///
	/// Measured against a 26.1 service with a 26.0 client:
	/// <c>export-iom-connection-data</c> answers HTTP 200 with 296 218 characters of JSON that the client
	/// cannot deserialise (<c>beams[0].plates[0].material</c> is an object where this client's IdeaRS.OpenModel
	/// expects a scalar). The call returned <c>null</c>, the example built on it concluded the joint's brace
	/// feet overlapped - a statement about geometry drawn from the absence of geometry - and no diagnostic
	/// anywhere named the real cause.
	/// </summary>
	public static class IdeaApiExceptionFactory
	{
		/// <summary>
		/// Assign to <see cref="IApiAccessor.ExceptionFactory"/>. Stateless and safe to share.
		/// </summary>
		public static readonly ExceptionFactory Instance = (methodName, response) =>
		{
			// A failed request is the generated factory's business, and its messages are the ones
			// callers already handle. Only what it lets through is reconsidered below.
			Exception generated = Configuration.DefaultExceptionFactory(methodName, response);
			if (generated != null)
			{
				return generated;
			}

			return UnreadableBody(methodName, response);
		};

		/// <summary>
		/// An <see cref="ApiException"/> when a 2xx response carries a body that did not become an
		/// object, or null when there is nothing wrong.
		///
		/// Deliberately narrow - this must never turn a working call into an exception:
		///
		///   - only 2xx, because everything else is already handled;
		///   - only when the deserialised value is null AND the body is not itself empty or the JSON
		///     literal <c>null</c>, which are legitimate answers ("no template applied", "no results yet");
		///   - only for a response type that CAN be null, so an endpoint returning a number or a bool is
		///     out of scope. Those cannot be told apart: a body that fails to convert leaves the default
		///     value (0, false), which is indistinguishable from a real one. Callers of those endpoints
		///     still depend on the service answering sensibly - fixing that needs the generated
		///     conversion to stop swallowing, which is not something this hook can reach.
		/// </summary>
		private static ApiException UnreadableBody(string methodName, IApiResponse response)
		{
			int status = (int)response.StatusCode;
			if (status < 200 || status > 299)
			{
				return null;
			}

			if (response.Content != null)
			{
				return null;
			}

			Type type = response.ResponseType;
			if (type == null || (type.IsValueType && Nullable.GetUnderlyingType(type) == null))
			{
				return null;
			}

			string raw = response.RawContent;
			if (string.IsNullOrWhiteSpace(raw) || raw.Trim() == "null")
			{
				return null;
			}

			// The reason, when the deserializer left one. Without it the length of the body is still
			// worth reporting: it is what distinguishes this from an empty answer.
			string reason = string.IsNullOrWhiteSpace(response.ErrorText)
				? $"the body is {raw.Length} characters long and no reason was reported"
				: response.ErrorText.Trim();

			return new ApiException(500,
				$"Error calling {methodName}: the service answered {status} but this client could not read"
				+ $" the response into {type.Name} ({reason}). This is usually a version mismatch - a"
				+ " service newer than the client package. Check the service version against the"
				+ " IdeaStatiCa.ConnectionApi version in use.",
				raw, response.Headers);
		}
	}
}
