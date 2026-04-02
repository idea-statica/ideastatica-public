using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IdeaStatiCa.Api.Connection.Model
{
	/// <summary>
	/// Position along a member at which internal forces are reported in a load effect.
	/// <para>
	/// For continuous members (e.g., a chord passing through a connection node),
	/// load effects contain entries at both <c>Begin</c> and <c>End</c>, representing
	/// the internal forces on either side of the connection. For ended members (e.g., braces),
	/// only a single position entry is returned.
	/// </para>
	/// <para>
	/// The sign convention for <see cref="ConLoadEffectSectionLoad"/> differs between positions.
	/// See <see cref="ConLoadEffectSectionLoad"/> for details.
	/// </para>
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ConLoadEffectPositionEnum
	{
		/// <summary>
		/// The End position of the member at the connection.
		/// Internal forces use the standard structural sign convention (positive N = tension).
		/// This is the default value and is used for ended members that have a single entry.
		/// </summary>
		End = 0,

		/// <summary>
		/// The Begin position of the member at the connection.
		/// At this position, the axial force (N) and bending moment (My) values use an inverted
		/// sign convention — they must be negated to obtain the standard convention (positive N = tension).
		/// </summary>
		Begin = 1,
	}
}