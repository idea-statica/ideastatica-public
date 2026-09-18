using FluentAssertions;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using NUnit.Framework;

namespace IdeaStatiCa.TeklaStructuresTest
{
	public class BoltAssemblyIdentityTest
	{
		[Test]
		public void SameValuesGiveSameIdentity()
		{
			string first = BoltAssemblyIdentity.Create("A490M-N/GR_10.9_BLACK", "A490M-N/ GR_10.9 BLACK", 0.024);
			string second = BoltAssemblyIdentity.Create("A490M-N/GR_10.9_BLACK", "A490M-N/ GR_10.9 BLACK", 0.024);

			second.Should().Be(first);
		}

		[Test]
		public void DifferentGradeGivesDifferentIdentity()
		{
			string tenNine = BoltAssemblyIdentity.Create("A490M-N/GR_10.9_BLACK", "A490M-N/ GR_10.9 BLACK", 0.03);
			string eightEight = BoltAssemblyIdentity.Create("A490M-N/GR_10.9_BLACK", "A325M-N/GR_8.8_BLACK", 0.03);

			eightEight.Should().NotBe(tenNine);
		}

		/// <summary>
		/// M24 and M25 render into the same display name, so an identity taken from the name would merge two different
		/// assemblies. The identity uses the raw diameter and keeps them apart.
		/// </summary>
		[Test]
		public void DiametersThatShareADisplayNameGiveDifferentIdentities()
		{
			0.025.MetersToInchesFormated().Should().Be(0.024.MetersToInchesFormated());

			string m24 = BoltAssemblyIdentity.Create("A490M-N/GR_10.9_BLACK", "A490M-N/ GR_10.9 BLACK", 0.024);
			string m25 = BoltAssemblyIdentity.Create("A490M-N/GR_10.9_BLACK", "A490M-N/ GR_10.9 BLACK", 0.025);

			m25.Should().NotBe(m24);
		}

		[Test]
		public void MissingNameAndGradeStillGiveAStableIdentity()
		{
			string first = BoltAssemblyIdentity.Create(null, null, 0.024);
			string second = BoltAssemblyIdentity.Create(null, null, 0.024);

			second.Should().Be(first);
			first.Should().NotBe(BoltAssemblyIdentity.Create(null, null, 0.03));
		}
	}
}
