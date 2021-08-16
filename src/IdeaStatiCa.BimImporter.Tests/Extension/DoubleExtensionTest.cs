using IdeaStatiCa.BimImporter.Extensions;
using NUnit.Framework;

namespace IdeaStatiCa.BimImporter.Tests.Extension
{
	[TestFixture]
	public class DoubleExtensionTest
	{
		[Test]
		public void NegLog10_asd()
		{
			Assert.That((int)(0.003.LeadingDecimalZeros()), Is.EqualTo(2));
		}
	}
}