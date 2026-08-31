using NorsokChecker.Services;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Pins the reading of the API's presentation payload — the source of the 3D preview.
	///
	/// The fixture below is the shape measured on test_cs: a top level of
	/// { groups, vertices, normals }, each group tagged `selected: { id, type: { kind } }` with the
	/// id as a STRING, `faces` as flat indices into the shared vertex array, and `text` carrying the
	/// member's label.
	///
	/// It carries all four kinds that occur in test_cs (measured 2026-08-31): member, weld, plate and
	/// boltGrid. EVERY kind is drawn — CON15 is a plated, bolted joint where keeping members only
	/// showed 17 % of the geometry — but only a MEMBER is assessable, so only a member gets a real
	/// member id.
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
		    },
		    {
		      "selected": { "id": "3", "type": { "kind": "plate" } },
		      "faces": [1,2,3]
		    },
		    {
		      "selected": { "id": "4", "type": { "kind": "boltGrid" } },
		      "faces": [2,3,4]
		    },
		    {
		      "selected": { "id": "9", "type": { "kind": "somethingNew" } },
		      "faces": [0,2,4]
		    }
		  ]
		}
		""";

		/// <summary>
		/// Every kind is returned, members and context alike. The reader used to keep members only;
		/// measured on test_cs that dropped 27–30 % of the geometry on the tubular joints and 83 % on
		/// CON15, whose plates and bolts are most of what there is to see.
		/// </summary>
		[Test]
		public void ReadMembers_ReturnsEveryKindNotJustMembers()
		{
			var meshes = JointPresentationReader.ReadMembers(Payload);

			Assert.Multiple(() =>
			{
				Assert.That(meshes.Count, Is.EqualTo(6), "all six groups are drawn");
				Assert.That(meshes.Select(m => m.Kind), Is.EquivalentTo(new[]
				{
					BodyKind.Member, BodyKind.Member, BodyKind.Weld,
					BodyKind.Plate, BodyKind.BoltGrid, BodyKind.Other,
				}));
			});
		}

		/// <summary>
		/// Only a MEMBER carries a member id; everything else is -1.
		///
		/// This is the distinction that keeps the checks honest: the §6.4 results, the utilisation
		/// colouring and the table's hover highlight are all keyed on the member id, and the plate and
		/// bolt groups carry ids in the same field ("3", "4") that collide with real member ids. Read
		/// naively, hovering member 3's row would light up a plate.
		/// </summary>
		[Test]
		public void ReadMembers_GivesAMemberIdOnlyToMembers()
		{
			var meshes = JointPresentationReader.ReadMembers(Payload);

			Assert.Multiple(() =>
			{
				Assert.That(meshes.Where(m => m.Kind == BodyKind.Member).Select(m => m.MemberId),
					Is.EquivalentTo(new[] { 1, 2 }));
				Assert.That(meshes.Where(m => m.Kind != BodyKind.Member).Select(m => m.MemberId),
					Is.All.EqualTo(-1),
					"a plate whose payload id is '3' must not answer to member 3");
			});
		}

		/// <summary>
		/// A body tagged "member" whose id is not one of the connection's members is DRAWN but not
		/// counted as a member.
		///
		/// Measured on test_cs CON15 (2026-08-31): the presentation returns NINE groups tagged
		/// "member" while GET .../members reports six — ids 7, 8 and 9 are extra bodies, unlabelled
		/// and with no member entry. Taking the tag at face value put three unassessable bodies in the
		/// members collection, so the view reported nine members and three of them could never carry a
		/// check. Verified both ways on CON1 too, where all six are real and none is demoted.
		/// </summary>
		[Test]
		public void ReadMembers_DemotesAMemberTagThatIsNotInTheMemberList()
		{
			// the fixture's members are 1 and 2; pretend the connection only has member 1
			var meshes = JointPresentationReader.ReadMembers(Payload, null, new[] { 1 });

			Assert.Multiple(() =>
			{
				Assert.That(meshes.Count, Is.EqualTo(6), "every body is still drawn");
				Assert.That(meshes.Where(m => m.Kind == BodyKind.Member).Select(m => m.MemberId),
					Is.EquivalentTo(new[] { 1 }), "only the known member stays assessable");
				Assert.That(meshes.Count(m => m.Kind == BodyKind.Other), Is.EqualTo(2),
					"member 2 joins the unknown kind as context");
			});
		}

		/// <summary>
		/// The known-id filter must not demote anything when every tagged member IS a real one — the
		/// control for the test above, and the CON1 case. Without it, a filter that demoted
		/// everything would pass the test above just as well.
		/// </summary>
		[Test]
		public void ReadMembers_DemotesNothingWhenEveryMemberIsKnown()
		{
			var meshes = JointPresentationReader.ReadMembers(Payload, null, new[] { 1, 2 });

			Assert.That(meshes.Where(m => m.Kind == BodyKind.Member).Select(m => m.MemberId),
				Is.EquivalentTo(new[] { 1, 2 }));
		}

		/// <summary>An unrecognised kind is still drawn, as Other — a payload that grows a kind
		/// should show up in the picture rather than silently vanish from it.</summary>
		[Test]
		public void ReadMembers_DrawsAnUnknownKindRatherThanDroppingIt()
		{
			var meshes = JointPresentationReader.ReadMembers(Payload);

			Assert.That(meshes.Count(m => m.Kind == BodyKind.Other), Is.EqualTo(1));
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
			var meshes = JointPresentationReader.ReadMembers(doubled);

			// the members, by id — the same check as on the plain payload, so the double encoding is
			// the only variable
			Assert.That(meshes.Where(m => m.Kind == BodyKind.Member).Select(m => m.MemberId),
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
