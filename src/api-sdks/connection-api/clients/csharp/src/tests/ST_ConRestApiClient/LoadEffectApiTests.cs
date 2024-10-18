using FluentAssertions;
using IdeaStatiCa.ConnectionApi.Model;

namespace ST_ConnectionRestApi
{
	public class LoadEffectApiTests : ConRestApiBaseTest
	{
		[SetUp]
		public async Task SetUp()
		{
			//if (this.RunServer)
			//{
			//	ConnectionApiClient = await ApiFactory.CreateConnectionApiClient();
			//}
			//else
			//{
			//	if (ApiUri == null)
			//	{
			//		throw new Exception("ApiUri is not set");
			//	}

			//	ConnectionApiClient = await ApiFactory.CreateConnectionApiClient(ApiUri);
			//}

			ConnectionApiClient = await ApiFactory.CreateConnectionApiClient();

			string connProjectFilePath = Path.Combine(ProjectPath, "Simple-1-ECEN.ideaCon");
			this.Project = await ConnectionApiClient.Project.OpenProjectAsync(connProjectFilePath);
			this.ActiveProjectId = Project.ProjectId;
			if (this.ActiveProjectId == Guid.Empty)
			{
				throw new Exception("Project is not opened");
			}

			this.Project = await ConnectionApiClient.Project.GetProjectDataAsync(ActiveProjectId);

			Project.Should().NotBeNull();
			Project.ProjectId.Should().NotBe(Guid.Empty);
		}

		[TearDown]
		public async Task TearDown()
		{
			if (ConnectionApiClient != null)
			{
				await ConnectionApiClient!.DisposeAsync();
			}
		}
		[TestCase(true)]
		[TestCase(false)]
		public async Task ShouldGetLoadEffects(bool percentage)
		{
			var con1 = Project!.Connections.First();
			var loadEffect = await ConnectionApiClient!.LoadEffect!.GetLoadEffectsAsync(ActiveProjectId, con1.Id, percentage);
			loadEffect.Should().NotBeNull();
			var le1 = loadEffect.First();
			le1.Should().NotBeNull();
			le1.IsPercentage.Should().Be(percentage);
			le1.MemberLoadings.Count().Should().Be(4);

			if(percentage)
			{
				//Mx is ignored for Percentage value
				le1.MemberLoadings.Select(x => x.SectionLoad.Mx.Should().Be(0));
			}
		}

		[TestCase(true)]
		[TestCase(false)]
		public async Task ShouldAddLoadEffect(bool percentage)
		{
			var con1 = Project!.Connections.First();
			var loadEffect = await ConnectionApiClient!.LoadEffect!.GetLoadEffectsAsync(ActiveProjectId, con1.Id, percentage);
			var originalCount = loadEffect.Count();

			var newLe = new ConLoadEffect()
			{
				Id = originalCount + 1,
				Active = true,
				Name = "Unit test LE",
				IsPercentage = percentage,
				MemberLoadings = new List<ConLoadEffectMemberLoad>
				{
					new ConLoadEffectMemberLoad
					{
						MemberId = 1,
						Position = ConLoadEffectPositionEnum.End,
						SectionLoad = new ConLoadEffectSectionLoad
						{
							N = percentage ? 10 : 60000,
							Vz = percentage ? -10 : -30000,
							Mx = percentage ? 10 : 6000
						}		
					}
				}
			};

			await ConnectionApiClient!.LoadEffect!.AddLoadEffectAsync(ActiveProjectId, con1.Id, newLe);
			var updatedState = await ConnectionApiClient!.LoadEffect!.GetLoadEffectsAsync(ActiveProjectId, con1.Id, percentage);

			updatedState.Count().Should().Be(originalCount + 1);

			var added = updatedState.Last();
			added.Active.Should().BeTrue();
			added.Name.Should().Be("Unit test LE");
			added.MemberLoadings.Count().Should().Be(4);

			if (percentage)
			{
				//Mx is ignored for Percentage value
				added.MemberLoadings.Select(x => x.SectionLoad.Mx.Should().Be(0));
			}
		}

		[Test]
		public async Task ShouldTestLoadEffectSettings()
		{
			var con1 = Project!.Connections.First();
			var settings = await ConnectionApiClient!.LoadEffect!.GetLoadSettingsAsync(ActiveProjectId, con1.Id);

			settings.LoadsInEquilibrium.Should().BeTrue();
			settings.LoadsInPercentage.Should().BeFalse();

			settings.LoadsInEquilibrium = false;
			settings.LoadsInPercentage = true;

			var updated = await ConnectionApiClient!.LoadEffect!.SetLoadSettingsAsync(ActiveProjectId, con1.Id, settings);
			updated.LoadsInEquilibrium.Should().BeFalse();
			updated.LoadsInPercentage.Should().BeTrue();


		}

		[Test]
		public async Task ShouldDeleteLoadEffects()
		{
			var con1 = Project!.Connections.First();
			var loadEffect = await ConnectionApiClient!.LoadEffect!.GetLoadEffectsAsync(ActiveProjectId, con1.Id);
			var originalCount = loadEffect.Count();

			var newLe = new ConLoadEffect()
			{
				Id = originalCount + 1,
				Active = true,
				Name = "Unit test LE",
				MemberLoadings = new List<ConLoadEffectMemberLoad>
				{
					new ConLoadEffectMemberLoad
					{
						MemberId = 1,
						Position = ConLoadEffectPositionEnum.End,
						SectionLoad = new ConLoadEffectSectionLoad
						{
							N = 60000,
							Vz =  -30000,
							Mx =  6000
						}
					}
				}
			};

			await ConnectionApiClient!.LoadEffect!.AddLoadEffectAsync(ActiveProjectId, con1.Id, newLe);

			var updatedState = await ConnectionApiClient!.LoadEffect!.GetLoadEffectsAsync(ActiveProjectId, con1.Id);
			updatedState.Count().Should().Be(originalCount + 1);

			await ConnectionApiClient!.LoadEffect!.DeleteLoadEffectAsync(ActiveProjectId, con1.Id, originalCount + 1);

			var afterDeleteState = await ConnectionApiClient!.LoadEffect!.GetLoadEffectsAsync(ActiveProjectId, con1.Id);
			afterDeleteState.Count().Should().Be(originalCount);
		}

		[Test]
		public async Task ShouldUpdateLoadEffect()
		{
			var con1 = Project!.Connections.First();
			var loadEffect = await ConnectionApiClient!.LoadEffect!.GetLoadEffectsAsync(ActiveProjectId, con1.Id);
			var le1 = loadEffect.First();

			var member1 = le1.MemberLoadings.First();
			var mx = member1.SectionLoad.Mx;
			var my = member1.SectionLoad.My;
			var mz = member1.SectionLoad.Mz;
			var n = member1.SectionLoad.N;
			var vy = member1.SectionLoad.Vy;
			var vz = member1.SectionLoad.Vz;

			le1.MemberLoadings.ElementAt(0).SectionLoad.Mx = mx + 10;
			le1.MemberLoadings.ElementAt(0).SectionLoad.My = my + 10;
			le1.MemberLoadings.ElementAt(0).SectionLoad.Mz = mz + 10;
			le1.MemberLoadings.ElementAt(0).SectionLoad.N = n + 10;
			le1.MemberLoadings.ElementAt(0).SectionLoad.Vy = vy + 10;
			le1.MemberLoadings.ElementAt(0).SectionLoad.Vz = vz + 10;

			await ConnectionApiClient!.LoadEffect!.UpdateLoadEffectAsync(ActiveProjectId, con1.Id, le1);

			var updated = await ConnectionApiClient!.LoadEffect!.GetLoadEffectsAsync(ActiveProjectId, con1.Id, cancellationToken: CancellationToken.None);

			var updatedMemberLoading = updated.First().MemberLoadings.First();
			updatedMemberLoading.SectionLoad.Mx.Should().Be(mx + 10);
			updatedMemberLoading.SectionLoad.My.Should().Be(my + 10);
			updatedMemberLoading.SectionLoad.Mz.Should().Be(mz + 10);
			updatedMemberLoading.SectionLoad.N.Should().Be(n + 10);
			updatedMemberLoading.SectionLoad.Vy.Should().Be(vy + 10);
			updatedMemberLoading.SectionLoad.Vz.Should().Be(vz + 10);
		}
	}
}
