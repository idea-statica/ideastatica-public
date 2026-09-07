using System.Text;

namespace IdeaStatiCa.ConnectionApi
{
	/// <summary>
	/// The name an application gives itself towards the Connection REST API service.
	///
	/// Why it exists: the service reports every endpoint call it serves, and without this every call
	/// looks the same. It cannot tell a python script from a Grasshopper component from a partner's
	/// desktop tool from one of our own examples, because the only thing a client is identified by is
	/// the ClientId it was handed, which is a fresh GUID per session. So we cannot answer "which
	/// integrations are actually used, and how" - and an application that wanted to be counted had to
	/// report its own events through IdeaStatiCa.Diagnostics, which is not published outside the
	/// company.
	///
	/// One string in a request header answers it for every client, whatever it is written in. Sending
	/// nothing keeps the previous behaviour: the calls are counted, just not attributed.
	///
	/// It identifies the APPLICATION, not the user: it is meant to be a constant of the build, and
	/// nothing about the machine, the project or the person belongs in it.
	/// </summary>
	public static class ClientApplicationIdentity
	{
		/// <summary>
		/// The request header the service reads the identification from. A header rather than a
		/// parameter of the connect call, so that identifying an application needs no change to any
		/// endpoint, no regenerated client, and no version of the service newer than the one the
		/// customer has - an older service simply ignores it.
		/// </summary>
		public const string HeaderName = "X-Idea-Client-App";

		/// <summary>
		/// The longest value that is sent. It is a name, not a payload, and it travels on every
		/// request; anything longer is cut.
		/// </summary>
		public const int MaxLength = 64;

		/// <summary>
		/// The header value for an application name and an optional version, or null when there is
		/// nothing to send.
		///
		/// The result is restricted to printable ASCII: a header value with a control character in it
		/// is rejected outright by some HTTP stacks and quietly mangled by others, and a name is not
		/// worth a failed request. Anything outside that range becomes '_', so the value stays
		/// recognisable rather than disappearing.
		/// </summary>
		/// <param name="application">For example "NorsokChecker". Optional.</param>
		/// <param name="version">For example "1.4.2". Optional.</param>
		public static string Format(string application, string version = null)
		{
			string name = Sanitize(application);
			if (name.Length == 0)
			{
				return null;
			}

			string suffix = Sanitize(version);
			string value = suffix.Length == 0 ? name : $"{name}/{suffix}";

			return value.Length <= MaxLength ? value : value.Substring(0, MaxLength);
		}

		private static string Sanitize(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return string.Empty;
			}

			var sanitized = new StringBuilder(text.Length);
			foreach (char c in text.Trim())
			{
				// 0x20-0x7E is printable ASCII; the separator is reserved for joining name and version
				sanitized.Append(c >= ' ' && c <= '~' && c != '/' ? c : '_');
			}

			return sanitized.ToString();
		}
	}
}
