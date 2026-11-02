using IdeaStatiCa.BimImporter.Common;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests.Common
{
	[TestFixture]
	public class DoubleApproximateEqualityComparerTest
	{
		[Test]
		public void GetHashCode_IfNumbersAreWithinPrecisionRange_HashCodesShouldNotEqual()
		{
			// Setup: with precision 0.1
			DoubleApproximateEqualityComparer comparer = new DoubleApproximateEqualityComparer(0.1);

			// Assert: 0.1 and 0.2 are within of 0.1 precision range, hashcodes shouldn't equal
			Assert.That(comparer.GetHashCode(0.1), Is.Not.EqualTo(comparer.GetHashCode(0.2)));
		}

		[Test]
		public void GetHashCode_IfNumbersAreOutsidePrecisionRange_HashCodesShouldEqual()
		{
			// Setup: with precision 0.1
			DoubleApproximateEqualityComparer comparer = new DoubleApproximateEqualityComparer(0.1);

			// Assert: 0.1 and 0.11 are outside of 0.1 precision range, hashcodes equal
			Assert.That(comparer.GetHashCode(0.1), Is.EqualTo(comparer.GetHashCode(0.11)));
		}

		[Test]
		public void GetHashCode_AfterChangingPrecision_IfNumbersAreWithinPrecisionRange_HashCodesShouldNotEqual()
		{
			// Setup: with precision 0.1
			DoubleApproximateEqualityComparer comparer = new DoubleApproximateEqualityComparer(0.1);

			// change precision to 0.01
			comparer.Precision = 0.01;

			// Assert: 0.1 and 0.11 are outside of 0.1 precision range, hashcodes equal
			Assert.That(comparer.GetHashCode(0.1), Is.Not.EqualTo(comparer.GetHashCode(0.11)));
		}

		[Test]
		public void Equals_IfNumbersAreWithinPrecisionRange_ShouldReturnFalse()
		{
			// Setup: with precision 0.1
			DoubleApproximateEqualityComparer comparer = new DoubleApproximateEqualityComparer(0.1);

			// Assert: 0.1 and 0.2 are within of 0.1 precision range, Equals should return false
			Assert.That(comparer.Equals(0.1, 0.2), Is.False);
		}

		[Test]
		public void Equals_IfNumbersAreOutsidePrecisionRange_ShouldReturnTrue()
		{
			// Setup: with precision 0.1
			DoubleApproximateEqualityComparer comparer = new DoubleApproximateEqualityComparer(0.1);

			// Assert: 0.1 and 0.15 are outside of 0.1 precision range, Equals should return true
			Assert.That(comparer.Equals(0.1, 0.15), Is.True);
		}

		[Test]
		public void Equals_AfterChangingPrecision_IfNumbersAreWithinPrecisionRange_ShouldReturnFalse()
		{
			// Setup: with precision 0.1
			DoubleApproximateEqualityComparer comparer = new DoubleApproximateEqualityComparer(0.1);

			comparer.Precision = 0.01;
			Assert.That(comparer.Equals(0.1, 0.15), Is.False);
		}
	}
}