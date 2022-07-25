using FluentAssertions;
using IdeaStatiCa.PluginRunner.HealthCheck;
using NUnit.Framework;

namespace IdeaStatiCa.PluginRunner.Tests.Unit.HealthCheck
{
	[TestFixture]
	public class DefaultHealthCheckerTest
	{
		[Test]
		public void WhenGotHeartBeat_ShouldPing()
		{
			// Arrange
			DefaultHealthChecker sut = new(TimeSpan.MaxValue, TimeSpan.MaxValue);

			// Act
			DefaultHealthState result = sut.Tick(DefaultHealthState.New, DateTime.Now, true);

			// Assert
			result.PingNow.Should().BeTrue();
		}

		[Test]
		public void WhenGotHeartBeat_ShouldNotBeDead()
		{
			// Arrange
			DefaultHealthChecker sut = new(TimeSpan.MaxValue, TimeSpan.MaxValue);

			// Act
			DefaultHealthState result = sut.Tick(DefaultHealthState.New, DateTime.Now, true);

			// Assert
			result.IsDead.Should().BeFalse();
		}

		[Test]
		public void WhenLastHeartbeatWasLongerThanPeriod_ShouldPing()
		{
			// Arrange
			DateTime now = DateTime.Now;
			DefaultHealthChecker sut = new(TimeSpan.FromSeconds(10), TimeSpan.MaxValue);
			DefaultHealthState state = DefaultHealthState.New.WithHeartbeat(now);

			// Act
			DefaultHealthState result = sut.Tick(state, now.AddHours(1), false);

			// Assert
			result.PingNow.Should().BeTrue();
		}

		[Test]
		public void WhenLastHeartbeatWasLongerThanTimeout_ShouldBeDead()
		{
			// Arrange
			DateTime now = DateTime.Now;
			DefaultHealthChecker sut = new(TimeSpan.MaxValue, TimeSpan.FromSeconds(10));
			DefaultHealthState state = DefaultHealthState.New.WithHeartbeat(now);

			// Act
			DefaultHealthState result = sut.Tick(state, now.AddHours(1), false);

			// Assert
			result.IsDead.Should().BeTrue();
		}
	}
}