using FluentAssertions;
using IdeaStatiCa.RcsApi;
using IdeaStatiCa.Api.RCS.Model;
using IdeaStatiCa.RcsApi.Client;
using IdeaRS.OpenModel.Geometry2D;

namespace ST_RcsRestApiClient
{
	/// <summary>
	/// Tests for CrossSection API functionality
	/// </summary>
	public class RcsRestApiCrossSectionTests : RcsRestApiBaseTest
	{
		[SetUp]
		public async Task SetUp()
		{
			RcsApiClient = await ApiFactory.CreateApiClient();
			if (RcsApiClient == null)
			{
				throw new Exception("RcsApiClient is not created");
			}

			string rcsProjectFilePath = Path.Combine(ProjectPath, "Project1.IdeaRcs");
			this.Project = await RcsApiClient!.Project.OpenProjectAsync(rcsProjectFilePath);
			this.ActiveProjectId = Project.ProjectId;
			if (this.ActiveProjectId == Guid.Empty)
			{
				throw new Exception("Project is not opened");
			}

			Project.Should().NotBeNull();
			Project.ProjectId.Should().NotBe(Guid.Empty);
		}

		[TearDown]
		public async Task TearDown()
		{
			if (RcsApiClient != null)
			{
				await RcsApiClient!.DisposeAsync();
			}
		}

		#region Get Reinforced Cross-Sections Tests

		[Test]
		public async Task ShouldGetReinforcedCrossSectionsTest()
		{
			// Act
			var reinforcedCrossSections = await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId);

			// Assert
			reinforcedCrossSections.Should().NotBeNull();
			reinforcedCrossSections.Should().NotBeEmpty("project should contain reinforced cross-sections");

			TestContext.Out.WriteLine($"Reinforced cross-sections count: {reinforcedCrossSections.Count}");

			foreach (var rcs in reinforcedCrossSections)
			{
				rcs.Id.Should().BeGreaterThan(0, "reinforced cross-section should have valid Id");
				rcs.Name.Should().NotBeNullOrEmpty("reinforced cross-section should have name");
				TestContext.Out.WriteLine($"  RCS Id: {rcs.Id}, Name: {rcs.Name}");
			}
		}

		[Test]
		public async Task ShouldGetReinforcedCrossSectionDataTest()
		{
			// Arrange
			var reinforcedCrossSections = await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId);
			reinforcedCrossSections.Should().NotBeEmpty();

			var firstRcs = reinforcedCrossSections.First();

			// Act
			var rcsData = await RcsApiClient!.CrossSection.GetReinforcedCrossSectionDataAsync(ActiveProjectId, firstRcs.Id);

			// Assert
			rcsData.Should().NotBeNull();
			rcsData.Id.Should().Be(firstRcs.Id);
			rcsData.Name.Should().Be(firstRcs.Name);

			// Verify cross-section reference
			rcsData.CrossSection.Should().NotBeNull("reinforced cross-section should reference a cross-section");

			// Verify bars
			rcsData.Bars.Count.Should().Be(6, "Reinforced cross-section should have 6 reinforcement bars");
			if (rcsData.Bars != null && rcsData.Bars.Count > 0)
			{
				TestContext.Out.WriteLine($"Number of reinforcement bars: {rcsData.Bars.Count}");
				foreach (var bar in rcsData.Bars)
				{
					bar.Diameter.Should().BeGreaterThan(0, "bar should have valid diameter");
					bar.Material.Element.Should().NotBeNull("bar should have material reference");
					bar.Point.Should().NotBeNull("bar should have position");
				}
			}

			rcsData.Stirrups.Count.Should().Be(1, "Reinforced cross-section should have one stirrups");
			// Verify stirrups
			if (rcsData.Stirrups != null && rcsData.Stirrups.Count > 0)
			{
				TestContext.Out.WriteLine($"Number of stirrups: {rcsData.Stirrups.Count}");
				foreach (var stirrup in rcsData.Stirrups)
				{
					stirrup.Diameter.Should().BeGreaterThan(0, "stirrup should have valid diameter");
					stirrup.Material.Element.Should().NotBeNull("stirrup should have material reference");
					stirrup.Geometry.Should().NotBeNull("stirrup should have geometry");
				}
			}
		}

		#endregion

		#region Import Reinforced Cross-Section Tests

		[Test]
		public async Task ShouldImportReinforcedCrossSectionTemplateTest()
		{
			// Arrange
			var templatePath = Path.Combine(ProjectPath, "RcsTemplate.nav");

			string template = File.ReadAllText(templatePath);

			var importData = new RcsReinforcedCrossSectionImportData
			{
				Setting = new RcsReinforcedCrosssSectionImportSetting
				{
					ReinforcedCrossSectionId = null, // Create new
					PartsToImport = "Complete"
				},
				Template = template
			};

			var rcsCountBefore = (await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId)).Count;

			// Act
			var importedRcs = await RcsApiClient!.CrossSection.ImportReinforcedCrossSectionAsync(ActiveProjectId, importData);

			// Assert
			importedRcs.Should().NotBeNull();
			importedRcs.Id.Should().BeGreaterThan(0, "imported RCS should have valid Id");

			var rcsCountAfter = (await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId)).Count;
			rcsCountAfter.Should().Be(rcsCountBefore + 1, "one reinforced cross-section should be added");

			TestContext.Out.WriteLine($"Imported RCS Id: {importedRcs.Id}, Name: {importedRcs.Name}");
		}

		[Test]
		public async Task ShouldUpdateExistingReinforcedCrossSectionFromTemplateTest()
		{
			// Arrange
			var templatePath = Path.Combine(ProjectPath, "RcsTemplate.nav");

			string template = File.ReadAllText(templatePath);

			var existingRcs = (await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId)).First();

			var importData = new RcsReinforcedCrossSectionImportData
			{
				Setting = new RcsReinforcedCrosssSectionImportSetting
				{
					ReinforcedCrossSectionId = existingRcs.Id, // Update existing
					PartsToImport = "Reinf"
				},
				Template = template
			};

			var rcsCountBefore = (await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId)).Count;

			// Act
			var updatedRcs = await RcsApiClient!.CrossSection.ImportReinforcedCrossSectionAsync(ActiveProjectId, importData);

			// Assert
			updatedRcs.Should().NotBeNull();
			updatedRcs.Id.Should().Be(existingRcs.Id, "should update existing RCS");

			var rcsCountAfter = (await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId)).Count;
			rcsCountAfter.Should().Be(rcsCountBefore, "count should remain same when updating");

			TestContext.Out.WriteLine($"Updated RCS Id: {updatedRcs.Id}, Name: {updatedRcs.Name}");
		}

		#endregion

		#region Add Reinforced Cross-Section Tests

		[Test]
		public async Task ShouldAddReinforcedCrossSectionTest()
		{
			// Arrange - Create a simple rectangular cross-section with reinforcement
			var rcsData = CreateSimpleReinforcedCrossSection();

			var rcsCountBefore = (await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId)).Count;

			// Act
			var addedRcs = await RcsApiClient!.CrossSection.AddReinforcedCrossSectionAsync(ActiveProjectId, rcsData);

			// Assert
			addedRcs.Should().NotBeNull();
			addedRcs.Id.Should().BeGreaterThan(0, "added RCS should have valid Id");
			addedRcs.Name.Should().Be(rcsData.Name);

			var rcsCountAfter = (await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId)).Count;
			rcsCountAfter.Should().Be(rcsCountBefore + 1, "one reinforced cross-section should be added");

			TestContext.Out.WriteLine($"Added RCS Id: {addedRcs.Id}, Name: {addedRcs.Name}");
		}

		[Test]
		public async Task ShouldVerifyAddedReinforcedCrossSectionGeometryTest()
		{
			// Arrange
			var rcsData = CreateSimpleReinforcedCrossSection();

			// Act
			var addedRcs = await RcsApiClient!.CrossSection.AddReinforcedCrossSectionAsync(ActiveProjectId, rcsData);
			var retrievedRcs = await RcsApiClient!.CrossSection.GetReinforcedCrossSectionDataAsync(ActiveProjectId, addedRcs.Id);

			// Assert
			retrievedRcs.Should().NotBeNull();
			retrievedRcs.Bars.Should().HaveCount(rcsData.Bars.Count, "should have same number of bars");
			retrievedRcs.Stirrups.Should().HaveCount(rcsData.Stirrups.Count, "should have same number of stirrups");

			TestContext.Out.WriteLine($"Verified RCS Id: {retrievedRcs.Id}");
			TestContext.Out.WriteLine($"  Bars count: {retrievedRcs.Bars.Count}");
			TestContext.Out.WriteLine($"  Stirrups count: {retrievedRcs.Stirrups.Count}");
		}

		[Test]
		public void ShouldFailWhenAddingRcsWithInvalidMaterialTest()
		{
			// Arrange
			var rcsData = CreateSimpleReinforcedCrossSection();
			// Use invalid material name that doesn't exist in project
			rcsData.CrossSection.Components[0].MaterialName = "InvalidMaterial12345";

			// Act & Assert
			var exception = Assert.ThrowsAsync<ApiException>(async () =>
			{
				await RcsApiClient!.CrossSection.AddReinforcedCrossSectionAsync(ActiveProjectId, rcsData);
			});

			exception.Should().NotBeNull("adding RCS with invalid material should throw exception");
			TestContext.Out.WriteLine($"Expected exception occurred: {exception.Message}");
		}

		#endregion

		#region Cross-Section Verification Tests

		[Test]
		public async Task ShouldVerifyReinforcedCrossSectionBarsTest()
		{
			// Arrange
			var reinforcedCrossSections = await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId);
			var firstRcs = reinforcedCrossSections.First();

			// Act
			var rcsData = await RcsApiClient!.CrossSection.GetReinforcedCrossSectionDataAsync(ActiveProjectId, firstRcs.Id);

			// Assert
			if (rcsData.Bars != null && rcsData.Bars.Count > 0)
			{
				foreach (var bar in rcsData.Bars)
				{
					// Verify bar properties
					bar.Diameter.Should().BeGreaterThan(0, "bar diameter should be positive");
					bar.Diameter.Should().BeLessOrEqualTo(0.05, "bar diameter should be reasonable (≤50mm)");

					bar.Point.Should().NotBeNull("bar should have position");
					bar.Material.Element.Should().NotBeNull("bar should reference material");

					TestContext.Out.WriteLine($"  Bar: Diameter={bar.Diameter:F4}m, Position=({bar.Point.X:F3}, {bar.Point.Y:F3})");
				}
			}
			else
			{
				TestContext.Out.WriteLine("No reinforcement bars found in cross-section");
			}
		}

		[Test]
		public async Task ShouldVerifyReinforcedCrossSectionStirrupsTest()
		{
			// Arrange
			var reinforcedCrossSections = await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId);
			var firstRcs = reinforcedCrossSections.First();

			// Act
			var rcsData = await RcsApiClient!.CrossSection.GetReinforcedCrossSectionDataAsync(ActiveProjectId, firstRcs.Id);

			// Assert
			if (rcsData.Stirrups != null && rcsData.Stirrups.Count > 0)
			{
				foreach (var stirrup in rcsData.Stirrups)
				{
					// Verify stirrup properties
					stirrup.Diameter.Should().BeGreaterThan(0, "stirrup diameter should be positive");
					stirrup.Diameter.Should().BeLessOrEqualTo(0.02, "stirrup diameter should be reasonable (≤20mm)");

					stirrup.Geometry.Should().NotBeNull("stirrup should have geometry");
					stirrup.Material.Element.Should().NotBeNull("stirrup should reference material");

					stirrup.Distance.Should().BeGreaterThan(0, "stirrup distance should be positive");

					TestContext.Out.WriteLine($"  Stirrup: Diameter={stirrup.Diameter:F4}m, Distance={stirrup.Distance:F3}m, IsClosed={stirrup.IsClosed}");
				}
			}
			else
			{
				TestContext.Out.WriteLine("No stirrups found in cross-section");
			}
		}

		[Test]
		public async Task ShouldVerifyStirrupGeometryTest()
		{
			// Arrange
			var reinforcedCrossSections = await RcsApiClient!.CrossSection.ReinforcedCrossSectionsAsync(ActiveProjectId);
			var firstRcs = reinforcedCrossSections.First();
			var rcsData = await RcsApiClient!.CrossSection.GetReinforcedCrossSectionDataAsync(ActiveProjectId, firstRcs.Id);

			// Act & Assert
			if (rcsData.Stirrups != null && rcsData.Stirrups.Count > 0)
			{
				var firstStirrup = rcsData.Stirrups.First();

				firstStirrup.Geometry.Should().NotBeNull("stirrup should have geometry");
				firstStirrup.Geometry.StartPoint.Should().NotBeNull("stirrup geometry should have start point");
				firstStirrup.Geometry.Segments.Should().NotBeNull("stirrup geometry should have segments");
				firstStirrup.Geometry.Segments.Count.Should().BeGreaterThan(0, "stirrup should have at least one segment");

				TestContext.Out.WriteLine($"Stirrup geometry:");
				TestContext.Out.WriteLine($"  Start point: ({firstStirrup.Geometry.StartPoint.X:F3}, {firstStirrup.Geometry.StartPoint.Y:F3})");
				TestContext.Out.WriteLine($"  Segments count: {firstStirrup.Geometry.Segments.Count}");

				foreach (var segment in firstStirrup.Geometry.Segments)
				{
					segment.EndPoint.Should().NotBeNull("segment should have end point");
					TestContext.Out.WriteLine($"    Segment end: ({segment.EndPoint.X:F3}, {segment.EndPoint.Y:F3})");
				}
			}
			else
			{
				Assert.Ignore("No stirrups found for geometry verification");
			}
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Creates a simple rectangular reinforced cross-section for testing
		/// </summary>
		private ReinforcedCrossSectionData CreateSimpleReinforcedCrossSection()
		{
			var rcsData = new ReinforcedCrossSectionData
			{
				Name = "Test RCS Rectangle 500x300",
				CrossSection = new RcsCrossSectionData
				{
					Name = "CSS Rectangle 500x300",
					CrossSectionRotation = 0,
					Components = new List<RcsCssComponentData>
					{
						new RcsCssComponentData
						{
							MaterialName = "C30/37", // Assumes this material exists in project
							Geometry = CreateRectangleRegion(0.5, 0.3) // 500x300mm
						}
					}
				},
				Bars = new List<RcsReinforcedBarData>
				{
					// Corner bars - 4x20mm
					CreateReinforcementBar(-0.215, -0.115, 0.020, "B 500B"),
					CreateReinforcementBar(0.215, -0.115, 0.020, "B 500B"),
					CreateReinforcementBar(0.215, 0.115, 0.020, "B 500B"),
					CreateReinforcementBar(-0.215, 0.115, 0.020, "B 500B")
				},
				Stirrups = new List<RcsStirrupsData>
				{
					CreateRectangularStirrup(0.43, 0.23, 0.010, 0.15, "B 500B")
				}
			};

			return rcsData;
		}

		/// <summary>
		/// Creates a rectangular region for cross-section component
		/// </summary>
		private Region2D CreateRectangleRegion(double width, double height)
		{
			var halfWidth = width / 2;
			var halfHeight = height / 2;

			var outline = new PolyLine2D
			{
				StartPoint = new Point2D { X = -halfWidth, Y = -halfHeight },
				Segments = new List<Segment2D>
				{
					new LineSegment2D { EndPoint = new Point2D { X = halfWidth, Y = -halfHeight } },
					new LineSegment2D { EndPoint = new Point2D { X = halfWidth, Y = halfHeight } },
					new LineSegment2D { EndPoint = new Point2D { X = -halfWidth, Y = halfHeight } },
					new LineSegment2D { EndPoint = new Point2D { X = -halfWidth, Y = -halfHeight } }
				}
			};

			return new Region2D
			{
				Outline = outline,
				Openings = new List<PolyLine2D>()
			};
		}

		/// <summary>
		/// Creates a reinforcement bar
		/// </summary>
		private RcsReinforcedBarData CreateReinforcementBar(double x, double y, double diameter, string materialName)
		{
			return new RcsReinforcedBarData
			{
				Point = new Point2D { X = x, Y = y },
				Diameter = diameter,
				MaterialName = materialName
			};
		}

		/// <summary>
		/// Creates a rectangular stirrup
		/// </summary>
		private RcsStirrupsData CreateRectangularStirrup(double width, double height, double diameter, double distance, string materialName)
		{
			var halfWidth = width / 2;
			var halfHeight = height / 2;

			var geometry = new PolyLine2D
			{
				StartPoint = new Point2D { X = -halfWidth, Y = -halfHeight },
				Segments = new List<Segment2D>
				{
					new LineSegment2D { EndPoint = new Point2D { X = halfWidth, Y = -halfHeight } },
					new LineSegment2D { EndPoint = new Point2D { X = halfWidth, Y = halfHeight } },
					new LineSegment2D { EndPoint = new Point2D { X = -halfWidth, Y = halfHeight } },
					new LineSegment2D { EndPoint = new Point2D { X = -halfWidth, Y = -halfHeight } }
				}
			};

			return new RcsStirrupsData
			{
				Geometry = geometry,
				Diameter = diameter,
				Distance = distance,
				DiameterOfMandrel = 4.0, // 4 times diameter as per EC2
				MaterialName = materialName,
				IsClosed = true,
				ShearCheck = true,
				TorsionCheck = false
			};
		}

		#endregion
	}
}