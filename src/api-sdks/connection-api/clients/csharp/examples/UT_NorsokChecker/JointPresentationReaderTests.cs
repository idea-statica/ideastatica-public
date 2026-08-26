using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Pins the reading of the API's presentation payload — the source of the 3D preview.
	///
	/// The fixture below is the shape measured on test_cs CON1: a top level of
	/// { groups, vertices, normals }, each group tagged `selected: { id, type: { kind } }` with the
	/// id as a STRING, `faces` as flat indices into the shared vertex array, and `text` carrying the
	/// member's label. Welds appear as groups too and must be left out.
	/// </summary>
	[TestFixture]
	public class JointPresentationReaderTests
	{
		private const string Payload = """
		{
		  "vertices": [0,0,0, 1,0,0, 0,1,0, 0,0,1, 2,0,0, 0,2,0],
		  "normals":  [0,0,1, 0,0,1, 0,0,1, 0,1,0, 0,1,0, 0,1,0],
		  "groups": [
		    {
		      "selected": { "id": "1", "type": { "kind": "member" } },
		      "faces": [0,1,2],
		      "color": [150,190,240,255],
		      "text": [ { "value": "M1" } ]
		    },
		    {
		      "selected": { "id": "2", "type": { "kind": "member" } },
		      "faces": [3,4,5, 0,4,5],
		      "text": [ { "value": "M2" } ]
		    },
		    {
		      "selected": { "id": "7", "type": { "kind": "weld" } },
		      "faces": [0,1,3]
		    }
		  ]
		}
		""";

		[Test]
		public void ReadMembers_KeepsMembersAndDropsWelds()
		{
			var meshes = JointPresentationReader.ReadMembers(Payload);

			Assert.That(meshes.Select(m => m.MemberId), Is.EquivalentTo(new[] { 1, 2 }),
				"the weld group must not be returned");
		}

		[Test]
		public void ReadMembers_ReadsTheIdAsANumberAndTheLabel()
		{
			var m1 = JointPresentationReader.ReadMembers(Payload).Single(m => m.MemberId == 1);

			Assert.That(m1.Name, Is.EqualTo("M1"));
		}

		[Test]
		public void ReadMembers_SharesTheVertexArrayAndIndexesIntoIt()
		{
			var meshes = JointPresentationReader.ReadMembers(Payload);
			var m1 = meshes.Single(m => m.MemberId == 1);
			var m2 = meshes.Single(m => m.MemberId == 2);

			Assert.Multiple(() =>
			{
				// 6 points, so 18 floats — the same array for every member
				Assert.That(m1.Positions, Has.Length.EqualTo(18));
				Assert.That(m2.Positions, Is.SameAs(m1.Positions), "the vertex array is shared");
				Assert.That(m1.Normals, Has.Length.EqualTo(18));

				Assert.That(m1.Indices, Is.EqualTo(new[] { 0, 1, 2 }));
				Assert.That(m2.Indices, Is.EqualTo(new[] { 3, 4, 5, 0, 4, 5 }));
			});
		}

		/// <summary>A group with too few indices to make a triangle is not a body.</summary>
		[Test]
		public void ReadMembers_SkipsAGroupWithoutAWholeTriangle()
		{
			const string thin = """
			{
			  "vertices": [0,0,0, 1,0,0],
			  "normals": [0,0,1, 0,0,1],
			  "groups": [ { "selected": { "id": "1", "type": { "kind": "member" } }, "faces": [0,1] } ]
			}
			""";

			Assert.That(JointPresentationReader.ReadMembers(thin), Is.Empty);
		}

		/// <summary>The payload is sometimes delivered as a JSON string containing JSON.</summary>
		[Test]
		public void ReadMembers_HandlesADoubleEncodedPayload()
		{
			string doubled = System.Text.Json.JsonSerializer.Serialize(Payload);

			Assert.That(JointPresentationReader.ReadMembers(doubled).Select(m => m.MemberId),
				Is.EquivalentTo(new[] { 1, 2 }));
		}

		[TestCase("")]
		[TestCase("not json")]
		[TestCase("{}")]
		[TestCase("""{ "groups": [] }""")]
		public void ReadMembers_ReturnsEmptyRatherThanThrowing(string input)
		{
			Assert.That(JointPresentationReader.ReadMembers(input), Is.Empty);
		}
	}
}
