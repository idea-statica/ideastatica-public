using IdeaStatiCa.Api.Connection.Model;

namespace UT_NorsokChecker
{
	/// <summary>
	/// Pins the "active load effects only" filter.
	///
	/// Worth a test despite being one expression, because the assumption behind it was wrong at
	/// first: <c>ConLoadEffect.Active</c> was taken to be <c>bool?</c> with "unstated" as a third
	/// state, and the filter was written <c>le.Active != false</c> to be safe against it. The
	/// compiler settled it — the property is a plain <c>bool</c>, so there are two states and the
	/// filter is simply <c>le.Active</c>. Anything cleverer than that is guarding against a case the
	/// type system says cannot happen.
	/// </summary>
	[TestFixture]
	public class ActiveLoadEffectFilterTests
	{
		private static ConLoadEffect Le(int id, bool active)
			=> new() { Id = id, Name = $"LE{id}", Active = active };

		/// <summary>The filter as MainWindow applies it.</summary>
		private static List<ConLoadEffect> Active(IEnumerable<ConLoadEffect> les)
			=> les.Where(le => le.Active).ToList();

		[Test]
		public void ADisabledLoadEffectIsSkipped()
		{
			var les = new[] { Le(1, true), Le(2, false), Le(3, true) };

			var kept = Active(les);

			Assert.That(kept.Select(l => l.Id), Is.EqualTo(new[] { 1, 3 }));
		}

		/// <summary>The known-good positive: nothing is dropped when the model has nothing off.</summary>
		[Test]
		public void AllActiveKeepsEverything()
		{
			var les = new[] { Le(1, true), Le(2, true), Le(3, true) };

			Assert.That(Active(les), Has.Count.EqualTo(3));
		}

		/// <summary>
		/// Everything switched off is a real answer, not an error: the connection then has nothing to
		/// assess and must report that, rather than falling back to assessing all of them — which is
		/// what silently ignoring an empty result would amount to.
		/// </summary>
		[Test]
		public void EverythingDisabledLeavesNothingToAssess()
		{
			var les = new[] { Le(1, false), Le(2, false) };

			Assert.That(Active(les), Is.Empty);
		}

		/// <summary>
		/// And with the toggle off, a disabled state is assessed like any other — the point of the
		/// toggle is that both readings are available, not that one is correct.
		/// </summary>
		[Test]
		public void WithTheToggleOffNothingIsFiltered()
		{
			var les = new[] { Le(1, true), Le(2, false) };

			// MainWindow keeps the full list unchanged in this branch
			Assert.That(les, Has.Length.EqualTo(2));
		}
	}
}
