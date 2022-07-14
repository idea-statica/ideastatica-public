using FluentAssertions;
using FluentAssertions.Events;
using IdeaStatiCa.PluginRunner.HealthCheck;
using NSubstitute;
using NUnit.Framework;

namespace IdeaStatiCa.PluginRunner.Tests.Integration.HealthCheck
{
	[TestFixture]
	public class HealthCheckServiceTest
	{
		[Test]
		public void WhenPinged_ShouldCallTickWithHeartbeat()
		{
			HealthCheckerStub checker = new();
			IHealthCheckEndpoint endpoint = Substitute.For<IHealthCheckEndpoint>();
			HealthCheckService<HealthStateStub> sut = new(endpoint, checker);

			using IMonitor<HealthCheckerStub> monitor = checker.Monitor();

			sut.Run();
			endpoint.Pinged += Raise.Event<Action>();

			monitor
				.Should()
				.Raise(nameof(HealthCheckerStub.OnTick))
				.WithArgs<bool>(x => x == true);
		}

		[Test]
		public void WhenStateHasPingNow_ShouldCallPingEndpoint()
		{
			IHealthCheckEndpoint endpoint = Substitute.For<IHealthCheckEndpoint>();
			HealthCheckerStub checker = new();
			checker.Transform = x => x.WithPingNow();
			HealthCheckService<HealthStateStub> sut = new(endpoint, checker);

			using IMonitor<HealthCheckerStub> monitor = checker.Monitor();

			sut.Run();
			endpoint.Pinged += Raise.Event<Action>();

			endpoint.Received().Ping();
		}
	}

	internal class HealthCheckerStub : IHealthChecker<HealthStateStub>
	{
		public event Action<HealthStateStub, DateTime, bool> OnTick;

		public Func<HealthStateStub, HealthStateStub> Transform { get; set; }

		public HealthStateStub NewState() => new(false, false);

		public HealthStateStub Tick(HealthStateStub state, DateTime now, bool gotHeartbeat)
		{
			OnTick?.Invoke(state, now, gotHeartbeat);

			if (Transform is not null)
			{
				state = Transform(state);
			}

			return state;
		}
	}

	internal class HealthStateStub : HealthState<HealthStateStub>, IEquatable<HealthStateStub>
	{
		public HealthStateStub(bool isDead, bool pingNow)
			: base(isDead, pingNow)
		{
		}

		protected override HealthStateStub Create(bool isDead, bool pingNow)
			=> new(isDead, pingNow);

		public bool Equals(HealthStateStub other)
		{
			if (other is null)
			{
				return false;
			}

			return other.IsDead == IsDead
				&& other.PingNow == PingNow;
		}
	}
}