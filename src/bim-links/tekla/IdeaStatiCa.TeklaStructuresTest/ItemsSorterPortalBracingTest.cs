using CI.Geometry3D;
using FluentAssertions;
using IdeaStatiCa.BIM.Common;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace IdeaStatiCa.TeklaStructuresTest
{
	/// <summary>
	/// The sorter input captured from a portal frame whose three brace ends carry the same connection detail: a gusset
	/// welded to the beam, a tab bolted to the gusset, and a cap welded to the tab and to the brace. Mirror images must
	/// sort alike, whichever of the equal beams happens to master a joint.
	/// </summary>
	public class ItemsSorterPortalBracingTest
	{
		[SetUp]
		public void CompareItemsByTheirParent() => Item.CustomComparer = new ParentIdentityComparer();

		[TestCase("y=0")]
		[TestCase("y=2.5")]
		[TestCase("y=5")]
		public void EveryBraceEnd_ArrivesWithItsWholeDetail(string end)
		{
			var detail = BraceEnds[end];
			var (data, settings) = CapturedPortalFrame();

			var joint = new ItemsSorter().Sort(data, settings).Joints.Single(j => j.Members.Any(m => (string)m.Parent == detail.Brace));

			joint.Plates.Select(p => (string)p.Parent).Should().Contain(new[] { detail.Gusset, detail.Tab, detail.Cap });
			joint.Fasteners.Select(f => (string)f.Parent).Should().Contain(detail.GussetBolts);
			joint.Welds.Select(w => (string)w.Parent).Should().Contain(new[] { detail.BraceToCap, detail.CapToTab });
			joint.Plates.Select(p => (string)p.Parent).Should().NotContain(BraceEnds.Where(other => other.Key != end).SelectMany(other => new[] { other.Value.Gusset, other.Value.Tab, other.Value.Cap }));
		}

		/// <summary>
		/// The weld to the brace is enough on its own. Without the one to the tab the cap has nothing else to arrive by,
		/// so it arrives only if weld recovery reaches it from the node it sits beside, whichever node masters the joint.
		/// </summary>
		[TestCase("y=0")]
		[TestCase("y=2.5")]
		[TestCase("y=5")]
		public void CapWeldedOnlyToItsBrace_StillArrives(string end)
		{
			var detail = BraceEnds[end];
			var (data, settings) = CapturedPortalFrame();
			data.Welds = data.Welds.Where(w => (string)w.Parent != detail.CapToTab).ToList();

			var joint = new ItemsSorter().Sort(data, settings).Joints.Single(j => j.Members.Any(m => (string)m.Parent == detail.Brace));

			joint.Plates.Select(p => (string)p.Parent).Should().Contain(detail.Cap);
			joint.Welds.Select(w => (string)w.Parent).Should().Contain(detail.BraceToCap);
		}

		private sealed class BraceEnd
		{
			public string Brace { get; set; }
			public string Gusset { get; set; }
			public string Tab { get; set; }
			public string Cap { get; set; }
			public string GussetBolts { get; set; }
			public string BraceToCap { get; set; }
			public string CapToTab { get; set; }
		}

		private static readonly Dictionary<string, BraceEnd> BraceEnds = new Dictionary<string, BraceEnd>
		{
			["y=0"] = new BraceEnd
			{
				Brace = "e71d1006-d215-45a3-a0cc-4e7c047ad20a",
				Gusset = "62a0f34e-9bfd-4d7a-96e7-eebea7bc17ae",
				Tab = "9287cd5c-44cd-4f5d-8b1a-21bd8205f718",
				Cap = "7870ff24-6bb1-4cf1-9b78-a6606d03fe21",
				GussetBolts = "8657237b-e1d9-4211-8c85-3be727f80763",
				BraceToCap = "368e866a-78b4-4e5e-96c6-892a13c145b1",
				CapToTab = "05927c99-e6cd-4417-b362-db0ae8b419d6",
			},
			["y=2.5"] = new BraceEnd
			{
				Brace = "c380ab0f-48ab-4fc3-91f2-4a60b03eaad7",
				Gusset = "03472dfa-b53b-409c-8bfa-3dab0b88ac01",
				Tab = "e1a62e5d-d565-4e94-a57d-ebbe1d6f37d9",
				Cap = "9d9850d0-9cfe-413b-b8b5-a15a7f228213",
				GussetBolts = "e36b3a00-b5fd-42f4-b856-b9467116422c",
				BraceToCap = "e22ffc3c-167f-4e22-9b98-f0e9d0492e45",
				CapToTab = "ef627f23-e7f8-4abd-af14-27bba7085cb7",
			},
			["y=5"] = new BraceEnd
			{
				Brace = "357cff22-9058-48a6-a1c5-59a206ecbdab",
				Gusset = "d22f00a4-9c73-4300-9e8a-4613ed436a79",
				Tab = "33d8d2c0-7c1d-4606-8c24-88e638c461b7",
				Cap = "88bb1d8e-481f-423a-b913-ab1b95ba3325",
				GussetBolts = "e422f52e-4611-45ff-95d6-72b97a79c7e1",
				BraceToCap = "44969cb8-0336-4433-bb3f-7b39aa37d02d",
				CapToTab = "791f1e6b-8b13-4f7e-9158-7201a750a730",
			},
		};

		/// <summary>
		/// Captured from a Tekla import in the order the link handed the items over, one entry per source part, under
		/// the settings the Model Coordinator branch of the Tekla plugin passes. Parents are the Tekla GUIDs.
		/// </summary>
		private static (SorterData Data, SorterSettings Settings) CapturedPortalFrame()
		{
			var settings = new SorterSettings { ChainNodes = true, EnlargeNodeXout = 0.90000000000000002, EnlargeNodeXin = 0.90000000000000002, EnlargeNodeY = 0.80000000000000004, EnlargeNodeZ = 0.80000000000000004, LengthTolerance = 0.00050000000000000001, PlateThicknessMult4Tolerance = 2, MaxInflateExtent = 230, JoinContinuousMemberByNodeBox = true };
			var m1 = new Member("5899f6d6-12e9-4903-98b9-14ac9c494b2e", new Matrix44(new Point3D(3200, 2500, 2560), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 0, 1)), new Point3D(3200, 2500, 2560), new Point3D(6400, 2500, 2560), new System.Windows.Rect(new System.Windows.Point(-64, -60), new System.Windows.Point(64, 60)));
			var m2 = new Member("e2afdc0f-6088-47cf-85f5-c1838e045dfd", new Matrix44(new Point3D(3200, 0, 2560), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 0, 1)), new Point3D(3200, 0, 2560), new Point3D(6400, 0, 2560), new System.Windows.Rect(new System.Windows.Point(-64, -60), new System.Windows.Point(64, 60)));
			var m3 = new Member("e71d1006-d215-45a3-a0cc-4e7c047ad20a", new Matrix44(new Point3D(3186.948345839171, 0, 4983.2938826742156), new Vector3D(0.78802437381552759, 0, -0.61564404185588084), new Vector3D(0, -1, 0), new Vector3D(0.61564404185588084, 0, 0.78802437381552759)), new Point3D(3186.948345839171, 0, 4983.2938826742156), new Point3D(6386.9483462101707, 0, 2483.2938831439246), new System.Windows.Rect(new System.Windows.Point(-42.399999999999999, -42.399999999999999), new System.Windows.Point(42.399999999999999, 42.399999999999999)));
			var m4 = new Member("ec40a1d7-8038-4dd4-bd7d-39f73f7b5d90", new Matrix44(new Point3D(3200, 5000, 2560), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 0, 1)), new Point3D(3200, 5000, 2560), new Point3D(6400, 5000, 2560), new System.Windows.Rect(new System.Windows.Point(-64, -60), new System.Windows.Point(64, 60)));
			var m5 = new Member("357cff22-9058-48a6-a1c5-59a206ecbdab", new Matrix44(new Point3D(3186.9483458392046, 5000, 4983.2938826742075), new Vector3D(0.78802437381552759, 0, -0.61564404185588095), new Vector3D(0, -1, 0), new Vector3D(0.61564404185588084, 0, 0.78802437381552759)), new Point3D(3186.9483458392046, 5000, 4983.2938826742075), new Point3D(6386.9483462102035, 5000, 2483.2938831439174), new System.Windows.Rect(new System.Windows.Point(-42.399999999999999, -42.399999999999999), new System.Windows.Point(42.399999999999999, 42.399999999999999)));
			var m6 = new Member("c380ab0f-48ab-4fc3-91f2-4a60b03eaad7", new Matrix44(new Point3D(3186.9483458392046, 2500, 4983.2938826742075), new Vector3D(0.78802437381552759, 0, -0.61564404185588095), new Vector3D(0, -1, 0), new Vector3D(0.61564404185588084, 0, 0.78802437381552759)), new Point3D(3186.9483458392046, 2500, 4983.2938826742075), new Point3D(6386.9483462102035, 2500, 2483.2938831439174), new System.Windows.Rect(new System.Windows.Point(-42.399999999999999, -42.399999999999999), new System.Windows.Point(42.399999999999999, 42.399999999999999)));
			var m7 = new Member("ae152965-7a9f-47b3-8705-00d711d03d38", new Matrix44(new Point3D(6368, 2500, 2560), new Vector3D(0, 1, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)), new Point3D(6368, 2500, 2560), new Point3D(6368, 5000, 2560), new System.Windows.Rect(new System.Windows.Point(-64, -60), new System.Windows.Point(64, 60)));
			var m8 = new Member("27a961d2-b5b4-47d2-92b4-9e32a0685d19", new Matrix44(new Point3D(6368, 0, 2560), new Vector3D(0, 1, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)), new Point3D(6368, 0, 2560), new Point3D(6368, 2500, 2560), new System.Windows.Rect(new System.Windows.Point(-64, -60), new System.Windows.Point(64, 60)));
			var m9 = new Member("4e808a01-070e-4cf0-a5b5-60e2127dd2a1", new Matrix44(new Point3D(4860, 2500, 2560), new Vector3D(0, 1, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)), new Point3D(4860, 2500, 2560), new Point3D(4860, 5000, 2560), new System.Windows.Rect(new System.Windows.Point(-64, -60), new System.Windows.Point(64, 60)));
			var m10 = new Member("9b944586-41e2-4690-b340-24cdcfffddd2", new Matrix44(new Point3D(4860, 0, 2560), new Vector3D(0, 1, 0), new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)), new Point3D(4860, 0, 2560), new Point3D(4860, 2500, 2560), new System.Windows.Rect(new System.Windows.Point(-64, -60), new System.Windows.Point(64, 60)));
			var p1 = new Plate("d22f00a4-9c73-4300-9e8a-4613ed436a79", new Matrix44(new Point3D(6170.8993184509154, 4992, 2695), new Vector3D(-1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 0, -1)), new List<IPoint3D> { new Point3D(6170.8993184509154, 4992, 2620), new Point3D(6020.8993184509154, 4992, 2620), new Point3D(6020.8993184509154, 4992, 2770), new Point3D(6170.8993184509154, 4992, 2770) }, 8);
			var p2 = new Plate("33d8d2c0-7c1d-4606-8c24-88e638c461b7", new Matrix44(new Point3D(6099.4701842524946, 5000, 2752.3010669543132), new Vector3D(-0.61564404185587607, 0, -0.78802437381553125), new Vector3D(0, -1, 0), new Vector3D(0.78802437381553148, 0, -0.61564404185587596)), new List<IPoint3D> { new Point3D(6172.6083024350619, 5000, 2695.1619121415856), new Point3D(6129.5132195050555, 5000, 2640.000205974417), new Point3D(5983.236983139921, 5000, 2754.2785155998722), new Point3D(6026.3320660699274, 5000, 2809.4402217670408) }, 8);
			var p3 = new Plate("88bb1d8e-481f-423a-b913-ab1b95ba3325", new Matrix44(new Point3D(6023.1799685747283, 5000, 2811.9027979345228), new Vector3D(-0.61564404185587929, 0, -0.78802437381552892), new Vector3D(0.78802437381552892, 0, -0.61564404185587929), new Vector3D(0, 1, 0)), new List<IPoint3D> { new Point3D(6023.1799685747283, 5025, 2811.9027979345228), new Point3D(5980.0848856447219, 5025, 2756.7410917673542), new Point3D(5980.0848856447219, 4975, 2756.7410917673542), new Point3D(6023.1799685747283, 4975, 2811.9027979345228) }, 8);
			var p4 = new Plate("64d082a3-fa44-49c2-a577-45584d1c21d5", new Matrix44(new Point3D(4853.7999999525, 4965.2999999525, 2520), new Vector3D(0, 0, 1), new Vector3D(-1, 0, 0), new Vector3D(0, 1, 0)), new List<IPoint3D> { new Point3D(4853.7999999525, 4997.7999999525, 2520), new Point3D(4853.7999999525, 4997.7999999525, 2600), new Point3D(4853.7999999525, 4932.7999999525, 2600), new Point3D(4853.7999999525, 4932.7999999525, 2520) }, 8);
			var p5 = new Plate("62a0f34e-9bfd-4d7a-96e7-eebea7bc17ae", new Matrix44(new Point3D(6170.8993184508918, -8, 2695), new Vector3D(-1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 0, -1)), new List<IPoint3D> { new Point3D(6170.8993184508918, -8, 2620), new Point3D(6020.8993184508918, -8, 2620), new Point3D(6020.8993184508918, -8, 2770), new Point3D(6170.8993184508918, -8, 2770) }, 8);
			var p6 = new Plate("9287cd5c-44cd-4f5d-8b1a-21bd8205f718", new Matrix44(new Point3D(6099.4701842524673, 0, 2752.3010669543128), new Vector3D(-0.61564404185587951, 0, -0.78802437381552848), new Vector3D(0, -1, 0), new Vector3D(0.78802437381552859, 0, -0.61564404185587951)), new List<IPoint3D> { new Point3D(6172.6083024350346, 0, 2695.1619121415847), new Point3D(6129.5132195050273, 0, 2640.0002059744143), new Point3D(5983.2369831398928, 0, 2754.2785155998704), new Point3D(6026.3320660699001, 0, 2809.4402217670408) }, 8);
			var p7 = new Plate("7870ff24-6bb1-4cf1-9b78-a6606d03fe21", new Matrix44(new Point3D(6023.1799685747019, 0, 2811.9027979345246), new Vector3D(-0.61564404185587929, 0, -0.78802437381552892), new Vector3D(0.78802437381552892, 0, -0.61564404185587929), new Vector3D(0, 1, 0)), new List<IPoint3D> { new Point3D(6023.1799685747019, 25, 2811.9027979345246), new Point3D(5980.0848856446964, 25, 2756.7410917673574), new Point3D(5980.0848856446964, -25, 2756.7410917673574), new Point3D(6023.1799685747019, -25, 2811.9027979345246) }, 8);
			var p8 = new Plate("3c99bebb-b27a-4906-96e4-1a5ce7e5ea8a", new Matrix44(new Point3D(6361.7999999525, 4965.2999999525, 2520), new Vector3D(0, 0, 1), new Vector3D(-1, 0, 0), new Vector3D(0, 1, 0)), new List<IPoint3D> { new Point3D(6361.7999999525, 4997.7999999525, 2520), new Point3D(6361.7999999525, 4997.7999999525, 2600), new Point3D(6361.7999999525, 4932.7999999525, 2600), new Point3D(6361.7999999525, 4932.7999999525, 2520) }, 8);
			var p9 = new Plate("03472dfa-b53b-409c-8bfa-3dab0b88ac01", new Matrix44(new Point3D(6170.8993184509154, 2492, 2695), new Vector3D(-1, 0, 0), new Vector3D(0, -1, 0), new Vector3D(0, 0, -1)), new List<IPoint3D> { new Point3D(6170.8993184509154, 2492, 2620), new Point3D(6020.8993184509154, 2492, 2620), new Point3D(6020.8993184509154, 2492, 2770), new Point3D(6170.8993184509154, 2492, 2770) }, 8);
			var p10 = new Plate("e1a62e5d-d565-4e94-a57d-ebbe1d6f37d9", new Matrix44(new Point3D(6099.4701842524946, 2500, 2752.3010669543132), new Vector3D(-0.61564404185587607, 0, -0.78802437381553125), new Vector3D(0, -1, 0), new Vector3D(0.78802437381553148, 0, -0.61564404185587596)), new List<IPoint3D> { new Point3D(6172.6083024350619, 2500, 2695.1619121415856), new Point3D(6129.5132195050555, 2500, 2640.000205974417), new Point3D(5983.236983139921, 2500, 2754.2785155998722), new Point3D(6026.3320660699274, 2500, 2809.4402217670408) }, 8);
			var p11 = new Plate("9d9850d0-9cfe-413b-b8b5-a15a7f228213", new Matrix44(new Point3D(6023.1799685747283, 2500, 2811.9027979345228), new Vector3D(-0.61564404185587929, 0, -0.78802437381552892), new Vector3D(0.78802437381552892, 0, -0.61564404185587929), new Vector3D(0, 1, 0)), new List<IPoint3D> { new Point3D(6023.1799685747283, 2525, 2811.9027979345228), new Point3D(5980.0848856447219, 2525, 2756.7410917673542), new Point3D(5980.0848856447219, 2475, 2756.7410917673542), new Point3D(6023.1799685747283, 2475, 2811.9027979345228) }, 8);
			var p12 = new Plate("dd5ee886-d1c9-40be-a863-227b51406f68", new Matrix44(new Point3D(4853.7999999525, 2534.7000000475, 2520), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0)), new List<IPoint3D> { new Point3D(4853.7999999525, 2502.2000000475, 2520), new Point3D(4853.7999999525, 2502.2000000475, 2600), new Point3D(4853.7999999525, 2567.2000000475, 2600), new Point3D(4853.7999999525, 2567.2000000475, 2520) }, 8);
			var p13 = new Plate("16b8034e-120b-48fa-be9c-3e1b47b255b4", new Matrix44(new Point3D(4853.7999999525, 34.700000047499998, 2520), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0)), new List<IPoint3D> { new Point3D(4853.7999999525, 2.2000000474999979, 2520), new Point3D(4853.7999999525, 2.2000000474999979, 2600), new Point3D(4853.7999999525, 67.200000047499998, 2600), new Point3D(4853.7999999525, 67.200000047499998, 2520) }, 8);
			var p14 = new Plate("c07847ac-3cac-4262-b2f7-1130c1781fd7", new Matrix44(new Point3D(4853.7999999525, 2465.2999999525, 2520), new Vector3D(0, 0, 1), new Vector3D(-1, 0, 0), new Vector3D(0, 1, 0)), new List<IPoint3D> { new Point3D(4853.7999999525, 2497.7999999525, 2520), new Point3D(4853.7999999525, 2497.7999999525, 2600), new Point3D(4853.7999999525, 2432.7999999525, 2600), new Point3D(4853.7999999525, 2432.7999999525, 2520) }, 8);
			var p15 = new Plate("11340922-ac2c-4a65-8040-4b6acf924b5a", new Matrix44(new Point3D(6361.7999999525, 34.700000047499998, 2520), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0)), new List<IPoint3D> { new Point3D(6361.7999999525, 2.2000000474999979, 2520), new Point3D(6361.7999999525, 2.2000000474999979, 2600), new Point3D(6361.7999999525, 67.200000047499998, 2600), new Point3D(6361.7999999525, 67.200000047499998, 2520) }, 8);
			var p16 = new Plate("7130f5b4-10cd-4e45-9868-7e4dd7352c87", new Matrix44(new Point3D(6361.7999999525, 2465.2999999525, 2520), new Vector3D(0, 0, 1), new Vector3D(-1, 0, 0), new Vector3D(0, 1, 0)), new List<IPoint3D> { new Point3D(6361.7999999525, 2497.7999999525, 2520), new Point3D(6361.7999999525, 2497.7999999525, 2600), new Point3D(6361.7999999525, 2432.7999999525, 2600), new Point3D(6361.7999999525, 2432.7999999525, 2520) }, 8);
			var p17 = new Plate("32df1f8a-a7b3-4471-a501-2a8495bcfc04", new Matrix44(new Point3D(6361.7999999525, 2534.7000000475, 2520), new Vector3D(0, 0, 1), new Vector3D(1, 0, 0), new Vector3D(0, -1, 0)), new List<IPoint3D> { new Point3D(6361.7999999525, 2502.2000000475, 2520), new Point3D(6361.7999999525, 2502.2000000475, 2600), new Point3D(6361.7999999525, 2567.2000000475, 2600), new Point3D(6361.7999999525, 2567.2000000475, 2520) }, 8);
			var f1 = new FastenerGrid("c2d81350-6223-41c9-91f5-a53d6edbaa3e", new Matrix44(new Point3D(6368, 4982.7999999525, 2620), new Vector3D(0, 0, -1), new Vector3D(-1, 0, 0), new Vector3D(0, -1, 0)), new List<Point3D> { new Point3D(6368, 4957.7999999525, 2585), new Point3D(6368, 4957.7999999525, 2535) });
			f1.ClampedItems.AddRange(new Item[] { p8, m7 });
			var f2 = new FastenerGrid("4d394932-fc65-4cda-8d44-5b4c78cfe926", new Matrix44(new Point3D(6368, 2517.2000000475, 2620), new Vector3D(0, 0, -1), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0)), new List<Point3D> { new Point3D(6368, 2542.2000000475, 2585), new Point3D(6368, 2542.2000000475, 2535) });
			f2.ClampedItems.AddRange(new Item[] { p17, m7 });
			var f3 = new FastenerGrid("d7c88b8d-1f01-45c5-a7f6-b891cb68808f", new Matrix44(new Point3D(6368, 2482.7999999525, 2620), new Vector3D(0, 0, -1), new Vector3D(-1, 0, 0), new Vector3D(0, -1, 0)), new List<Point3D> { new Point3D(6368, 2457.7999999525, 2585), new Point3D(6368, 2457.7999999525, 2535) });
			f3.ClampedItems.AddRange(new Item[] { p16, m8 });
			var f4 = new FastenerGrid("d37cabe0-a124-43dc-aef4-3c7372dff348", new Matrix44(new Point3D(6368, 17.200000047499998, 2620), new Vector3D(0, 0, -1), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0)), new List<Point3D> { new Point3D(6368, 42.200000047499998, 2585), new Point3D(6368, 42.200000047499998, 2535) });
			f4.ClampedItems.AddRange(new Item[] { p15, m8 });
			var f5 = new FastenerGrid("5e2f4488-e1ed-4a98-86d7-cd4effcaa1c8", new Matrix44(new Point3D(4860, 4982.7999999525, 2620), new Vector3D(0, 0, -1), new Vector3D(-1, 0, 0), new Vector3D(0, -1, 0)), new List<Point3D> { new Point3D(4860, 4957.7999999525, 2585), new Point3D(4860, 4957.7999999525, 2535) });
			f5.ClampedItems.AddRange(new Item[] { p4, m9 });
			var f6 = new FastenerGrid("13de695a-0985-49dd-9711-f0daf3cc61e6", new Matrix44(new Point3D(4860, 2517.2000000475, 2620), new Vector3D(0, 0, -1), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0)), new List<Point3D> { new Point3D(4860, 2542.2000000475, 2585), new Point3D(4860, 2542.2000000475, 2535) });
			f6.ClampedItems.AddRange(new Item[] { p12, m9 });
			var f7 = new FastenerGrid("33a4ed24-8d0f-4bd1-817e-1c85c6a505f0", new Matrix44(new Point3D(4860, 2482.7999999525, 2620), new Vector3D(0, 0, -1), new Vector3D(-1, 0, 0), new Vector3D(0, -1, 0)), new List<Point3D> { new Point3D(4860, 2457.7999999525, 2585), new Point3D(4860, 2457.7999999525, 2535) });
			f7.ClampedItems.AddRange(new Item[] { p14, m10 });
			var f8 = new FastenerGrid("88480431-cf5e-4cbf-8a90-70e6e9638d0b", new Matrix44(new Point3D(4860, 17.200000047499998, 2620), new Vector3D(0, 0, -1), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0)), new List<Point3D> { new Point3D(4860, 42.200000047499998, 2585), new Point3D(4860, 42.200000047499998, 2535) });
			f8.ClampedItems.AddRange(new Item[] { p13, m10 });
			var f9 = new FastenerGrid("e422f52e-4611-45ff-95d6-72b97a79c7e1", new Matrix44(new Point3D(6119.5400496653538, 5000, 2692.2066147576661), new Vector3D(-0.78802437381552748, 0, 0.61564404185588084), new Vector3D(0, -1, 0), new Vector3D(-0.61564404185588051, 0, -0.78802437381552781)), new List<Point3D> { new Point3D(6119.5400496653538, 5000, 2692.2066147576661), new Point3D(6072.2585872364261, 5000, 2729.1452572690159) });
			f9.ClampedItems.AddRange(new Item[] { p1, p2 });
			var f10 = new FastenerGrid("8657237b-e1d9-4211-8c85-3be727f80763", new Matrix44(new Point3D(6119.5400496653274, 0, 2692.2066147576666), new Vector3D(-0.78802437381552781, 0, 0.61564404185588073), new Vector3D(0, -1, 0), new Vector3D(-0.61564404185588051, 0, -0.78802437381552781)), new List<Point3D> { new Point3D(6119.5400496653274, 0, 2692.2066147576666), new Point3D(6072.2585872363998, 0, 2729.1452572690164) });
			f10.ClampedItems.AddRange(new Item[] { p5, p6 });
			var f11 = new FastenerGrid("e36b3a00-b5fd-42f4-b856-b9467116422c", new Matrix44(new Point3D(6119.5400496653538, 2500, 2692.2066147576661), new Vector3D(-0.78802437381552748, 0, 0.61564404185588084), new Vector3D(0, -1, 0), new Vector3D(-0.61564404185588051, 0, -0.78802437381552781)), new List<Point3D> { new Point3D(6119.5400496653538, 2500, 2692.2066147576661), new Point3D(6072.2585872364261, 2500, 2729.1452572690159) });
			f11.ClampedItems.AddRange(new Item[] { p9, p10 });
			var w1 = new Weld("1bcb99e7-5c7a-4bf9-9780-e66e445fdc7e", m1, p9);
			var x28 = new Member("35d75da4-934d-404a-88c3-8c4b4ffffd8b", new Matrix44(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1)), new Point3D(0, 0, 0), new Point3D(0, 0, 0), System.Windows.Rect.Empty);
			var w2 = new Weld("f6aff830-56d1-4cc6-a4f5-a048fe60701b", m1, x28);
			var w3 = new Weld("fd6df574-0db4-4381-851c-0e029e8f0358", m1, x28);
			var w4 = new Weld("21bbe7a6-348c-47f2-b07d-148eeef08f73", m1, x28);
			var w5 = new Weld("c31c60d1-4a0c-416b-a94b-a4808cab7ee6", m1, p12);
			var w6 = new Weld("aaa56a10-bb1a-45a9-a87f-13cb78048161", m1, p14);
			var w7 = new Weld("365e6579-1de0-4e61-ab59-0e6b1e5e84e1", m1, p16);
			var w8 = new Weld("ef79f177-c832-4a52-b659-f85d22bf0695", m1, p17);
			var x29 = new Member("00850de5-cfd0-453f-abdc-a4039980ed4c", new Matrix44(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1)), new Point3D(0, 0, 0), new Point3D(0, 0, 0), System.Windows.Rect.Empty);
			var w9 = new Weld("6d3321a4-8bd3-4df0-9d4b-1cccc8dc925b", m2, x29);
			var w10 = new Weld("7676b3fb-f522-4b60-8049-4a7efcf62b6a", m2, x29);
			var w11 = new Weld("293e7427-1795-4868-a27b-1d982a42916f", m2, x29);
			var w12 = new Weld("1af21b76-cb27-4290-b833-bceca1482560", m2, p5);
			var w13 = new Weld("2e1451de-656a-497b-b663-8364aa94790e", m2, p13);
			var w14 = new Weld("8d0ccdf3-cf07-4240-97cd-cb9368610a9e", m2, p15);
			var w15 = new Weld("368e866a-78b4-4e5e-96c6-892a13c145b1", m3, p7);
			var x30 = new Member("579dada0-caf1-4ec0-8bd6-3533c97e3625", new Matrix44(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1)), new Point3D(0, 0, 0), new Point3D(0, 0, 0), System.Windows.Rect.Empty);
			var w16 = new Weld("8630fc62-8ffc-42ad-800d-f27b166a283b", m3, x30);
			var w17 = new Weld("fb6360e2-fc86-44d3-a438-56f2ff96d466", m4, p1);
			var x31 = new Member("04cf0004-16db-4569-abb6-236392b09a36", new Matrix44(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1)), new Point3D(0, 0, 0), new Point3D(0, 0, 0), System.Windows.Rect.Empty);
			var w18 = new Weld("ba044d96-2f69-45e9-80e9-e5eb8a44f651", m4, x31);
			var w19 = new Weld("837ca929-c101-415e-af41-1b392dcf78c3", m4, x31);
			var w20 = new Weld("d9ea7924-469b-4499-9966-45fb09a8cddc", m4, x31);
			var w21 = new Weld("69b4d612-40f0-40cb-8484-b35eec3daca6", m4, p4);
			var w22 = new Weld("f871d18e-7cd1-4f1d-88e2-957cedb8c7b4", m4, p8);
			var w23 = new Weld("44969cb8-0336-4433-bb3f-7b39aa37d02d", m5, p3);
			var x32 = new Member("9c348637-0e4f-404d-bebb-626934c08c98", new Matrix44(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1)), new Point3D(0, 0, 0), new Point3D(0, 0, 0), System.Windows.Rect.Empty);
			var w24 = new Weld("06c8e94e-0976-4b6b-a7ae-4948d2b592cc", m5, x32);
			var w25 = new Weld("e22ffc3c-167f-4e22-9b98-f0e9d0492e45", m6, p11);
			var x33 = new Member("15c49a17-6c57-430a-b4ac-c1672e45947d", new Matrix44(new Point3D(0, 0, 0), new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), new Vector3D(0, 0, 1)), new Point3D(0, 0, 0), new Point3D(0, 0, 0), System.Windows.Rect.Empty);
			var w26 = new Weld("6b67d029-f4a1-4a32-b08e-98491594fc99", m6, x33);
			var w27 = new Weld("791f1e6b-8b13-4f7e-9158-7201a750a730", p3, p2);
			var w28 = new Weld("05927c99-e6cd-4417-b362-db0ae8b419d6", p7, p6);
			var w29 = new Weld("ef627f23-e7f8-4abd-af14-27bba7085cb7", p11, p10);
			var data = new SorterData { Members = new List<Member> { m1, m2, m3, m4, m5, m6, m7, m8, m9, m10 }, Plates = new List<Plate> { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17 }, Fasteners = new List<FastenerGrid> { f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11 }, Welds = new List<Weld> { w1, w2, w3, w4, w5, w6, w7, w8, w9, w10, w11, w12, w13, w14, w15, w16, w17, w18, w19, w20, w21, w22, w23, w24, w25, w26, w27, w28, w29 } };
			return (data, settings);
		}

		private sealed class ParentIdentityComparer : IEqualityComparer<Item>
		{
			public bool Equals(Item x, Item y) => x.Parent.Equals(y.Parent);

			public int GetHashCode(Item obj) => obj.Parent.GetHashCode();
		}
	}
}
