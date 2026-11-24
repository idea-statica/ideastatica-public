using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Loading;
using IdeaRS.OpenModel.Material;
using IdeaRS.OpenModel.Model;
using IdeaRS.OpenModel.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IOM.SteelFrameModel
{
	/// <summary>
	/// The steel frame of the tutorial, designed to EN: three columns, two floor beams and a roof beam. It has two
	/// connections: at node N2 with plates, bolts and welds, and at node N4 with the members only.
	/// </summary>
	public static class SteelFrame
	{
		/// <summary>
		/// Builds the model of the frame.
		/// </summary>
		public static OpenModel CreateIOM()
		{
			var model = new OpenModel();
			AddSettings(model);
			AddNodes(model);
			AddMaterial(model);
			AddCrossSections(model);
			AddMembers(model);
			AddLoadCases(model);
			AddCombinations(model);
			AddConnectionN2(model);
			AddConnectionN4(model);
			return model;
		}

		/// <summary>
		/// Loads the internal forces on the members of the frame, for the load cases of <see cref="CreateIOM"/>, as an
		/// FEA application computed them. They are in SampleFiles\IOM-SteelFrame.xmlR next to the application.
		/// </summary>
		public static OpenModelResult LoadInternalForces()
		{
			return OpenModelResult.LoadFromXmlFile(Path.Combine(AppContext.BaseDirectory, "SampleFiles", "IOM-SteelFrame.xmlR"));
		}

		private static void AddSettings(OpenModel model)
		{
			model.OriginSettings = new OriginSettings
			{
				CrossSectionConversionTable = CrossSectionConversionTable.SCIA,
				CountryCode = CountryCode.ECEN,
				ProjectName = "Project",
				Author = "IDEA StatiCa s.r.o.",
				ProjectDescription = "Training example",
				CheckEquilibrium = true,
			};
		}

		private static void AddNodes(OpenModel model)
		{
			IomHelpers.AddNode(model, 1, "N1", -2, 3, 0);
			IomHelpers.AddNode(model, 2, "N2", -2, 3, 3);
			IomHelpers.AddNode(model, 3, "N3", 2, 3, 0);
			IomHelpers.AddNode(model, 4, "N4", 2, 3, 3);
			IomHelpers.AddNode(model, 5, "N5", 6, 3, 0);
			IomHelpers.AddNode(model, 6, "N6", 6, 3, 3);
			IomHelpers.AddNode(model, 7, "N7", -2, 3, 6);
			IomHelpers.AddNode(model, 8, "N8", 2, 3, 6);
			IomHelpers.AddNode(model, 9, "N9", 6, 3, 6);
		}

		private static void AddMaterial(OpenModel model)
		{
			var material = new MatSteelEc2
			{
				Id = 1,
				Name = "S355",
				E = 210e9,
				Poisson = 0.3,
				UnitMass = 7850,
				SpecificHeat = 0.6,
				ThermalExpansion = 0.000012,
				ThermalConductivity = 45,
				IsDefaultMaterial = false,
				OrderInCode = 0,
				StateOfThermalExpansion = ThermalExpansionState.Code,
				StateOfThermalConductivity = ThermalConductivityState.Code,
				StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code,
				StateOfThermalStressStrain = ThermalStressStrainState.Code,
				StateOfThermalStrain = ThermalStrainState.Code,
				fy = 355e6,
				fu = 510e6,
				fy40 = 335e6,
				fu40 = 470e6,
				DiagramType = SteelDiagramType.Bilinear,
			};
			material.G = material.E / (2 * (1 + material.Poisson));
			IomHelpers.Add(model, material);
		}

		private static void AddCrossSections(OpenModel model)
		{
			MatSteel material = model.MatSteel.First();
			IomHelpers.Add(model, IomHelpers.CreateRolledI(1, "HE200B", material));
			IomHelpers.Add(model, IomHelpers.CreateRolledI(2, "HE240B", material));
		}

		private static void AddMembers(OpenModel model)
		{
			CrossSection beam = model.CrossSection.First(css => css.Name == "HE200B");
			CrossSection column = model.CrossSection.First(css => css.Name == "HE240B");

			IomHelpers.AddMember(model, 1, Member1DType.Beam, beam, "N2", "N4");          // left floor beam
			IomHelpers.AddMember(model, 2, Member1DType.Beam, beam, "N4", "N6");          // right floor beam
			IomHelpers.AddMember(model, 3, Member1DType.Column, column, "N1", "N2", "N7"); // left column
			IomHelpers.AddMember(model, 4, Member1DType.Column, column, "N3", "N4", "N8"); // middle column
			IomHelpers.AddMember(model, 5, Member1DType.Column, column, "N5", "N6", "N9"); // right column
			IomHelpers.AddMember(model, 6, Member1DType.Beam, beam, "N7", "N8", "N9");    // roof beam
		}

		private static void AddLoadCases(OpenModel model)
		{
			var permanent = new LoadGroupEC
			{
				Id = 1,
				Name = "PERM1",
				Relation = Relation.Standard,
				GroupType = LoadGroupType.Permanent,
				GammaQ = 1.35,
				Dzeta = 0.85,
				GammaGInf = 1,
				GammaGSup = 1.35,
			};
			IomHelpers.Add(model, permanent);

			var variable = new LoadGroupEC
			{
				Id = 2,
				Name = "VAR1",
				Relation = Relation.Exclusive,
				GroupType = LoadGroupType.Variable,
				GammaQ = 1.5,
				Dzeta = 0.85,
				GammaGInf = 0,
				GammaGSup = 1.5,
				Psi0 = 0.7,
				Psi1 = 0.5,
				Psi2 = 0.3,
			};
			IomHelpers.Add(model, variable);

			IomHelpers.Add(model, new LoadCase
			{
				Id = 1,
				Name = "SelfWeight",
				LoadType = LoadCaseType.Permanent,
				Type = LoadCaseSubType.PermanentStandard,
				Variable = VariableType.Standard,
				LoadGroup = new ReferenceElement(permanent),
			});
			IomHelpers.Add(model, new LoadCase
			{
				Id = 2,
				Name = "PermanentLoading",
				LoadType = LoadCaseType.Permanent,
				Type = LoadCaseSubType.PermanentStandard,
				Variable = VariableType.Standard,
				LoadGroup = new ReferenceElement(permanent),
			});
			IomHelpers.Add(model, new LoadCase
			{
				Id = 3,
				Name = "LiveLoad",
				LoadType = LoadCaseType.Variable,
				Type = LoadCaseSubType.VariableStatic,
				Variable = VariableType.Standard,
				LoadGroup = new ReferenceElement(variable),
			});
		}

		private static void AddCombinations(OpenModel model)
		{
			AddUlsCombination(model, "Co.#1", "SelfWeight", "PermanentLoading", "LiveLoad");
			AddUlsCombination(model, "Co.#2", "SelfWeight");
		}

		/// <summary>
		/// Adds a linear ULS combination of the given load cases, each with the coefficient 1.
		/// </summary>
		private static void AddUlsCombination(OpenModel model, string name, params string[] loadCases)
		{
			var combination = new CombiInputEC
			{
				Name = name,
				Description = string.Join(" + ", loadCases),
				TypeCombiEC = TypeOfCombiEC.ULS,
				TypeCalculationCombi = TypeCalculationCombiEC.Linear,
			};
			combination.Id = model.GetMaxId(combination) + 1;

			for (int i = 0; i < loadCases.Length; i++)
			{
				combination.Items.Add(new CombiItem
				{
					Id = i + 1,
					Coeff = 1,
					LoadCase = new ReferenceElement(model.LoadCase.First(lc => lc.Name == loadCases[i])),
				});
			}

			IomHelpers.Add(model, combination);
		}

		/// <summary>
		/// The connection at node N2. The left floor beam M1 ends there, and the left column M3 runs through. An end
		/// plate welded to the beam is bolted to the column, and four stiffeners are welded into the column.
		/// </summary>
		private static void AddConnectionN2(OpenModel model)
		{
			Member1D beam = model.Member1D.First(m => m.Id == 1);
			Member1D column = model.Member1D.First(m => m.Id == 3);
			ConnectionPoint connectionPoint = IomHelpers.AddConnectionPoint(model, "N2", IomHelpers.Ended(beam), IomHelpers.Continuous(column));

			BeamData beamData = IomHelpers.CreateBeamData(beam);
			BeamData columnData = IomHelpers.CreateBeamData(column);
			var connection = new ConnectionData
			{
				ConnectionPoint = new ReferenceElement(connectionPoint),
				Beams = new List<BeamData> { beamData, columnData },
				Plates = new List<PlateData>(),
				CutBeamByBeams = new List<CutBeamByBeamData>(),
				BoltGrids = new List<BoltGrid>(),
				Welds = new List<WeldData>(),
			};
			MatSteel steel = model.MatSteel.First();

			// The end plate: it cuts the beam, and is welded to it.
			var endPlate = new PlateData
			{
				Id = 11,
				Name = "P1",
				OriginalModelId = "11",
				Thickness = 0.02,
				Material = new ReferenceElement(steel),
				Origin = new Point3D { X = -1.87, Y = 2.88, Z = 2.7 },
				AxisX = new Vector3D { X = 0, Y = 1, Z = 0 },
				AxisY = new Vector3D { X = 0, Y = 0, Z = 1 },
				AxisZ = new Vector3D { X = 1, Y = 0, Z = 0 },
				Geometry = IomHelpers.CreateOutline(0, 0, 0.24, 0, 0.24, 0.5, 0, 0.5),
			};
			connection.Plates.Add(endPlate);
			connection.CutBeamByBeams.Add(new CutBeamByBeamData
			{
				CuttingObject = new ReferenceElement(endPlate),
				ModifiedObject = new ReferenceElement(beamData),
				Orientation = CutOrientation.Parallel,
				IsWeld = true,
			});

			// Four M16 bolts of grade 8.8 join the end plate and the column.
			var boltGrade = new MaterialBoltGradeEc2 { Id = 1, Name = "8.8", LoadFromLibrary = true };
			IomHelpers.Add(model, boltGrade);
			var boltAssembly = new BoltAssembly
			{
				Id = 1,
				Name = "M 16",
				BoltGrade = new ReferenceElement(boltGrade),
				Diameter = 0.016,
				Borehole = 0.018,
				HeadDiameter = 0.024,
				DiagonalHeadDiameter = 0.026,
				HeadHeight = 0.01,
				GrossArea = 0.000201,
				TensileStressArea = 0.000157,
				NutThickness = 0.013,
			};
			IomHelpers.Add(model, boltAssembly);
			connection.BoltGrids.Add(new BoltGrid
			{
				Id = 41,
				BoltAssembly = new ReferenceElement(boltAssembly),
				Length = 0.05,
				Origin = endPlate.Origin,
				AxisX = endPlate.AxisX,
				AxisY = endPlate.AxisY,
				AxisZ = endPlate.AxisZ,
				Positions = new List<Point3D>
				{
					new Point3D { X = -1.87, Y = 2.92, Z = 2.8 },
					new Point3D { X = -1.87, Y = 3.08, Z = 2.8 },
					new Point3D { X = -1.87, Y = 2.92, Z = 3.15 },
					new Point3D { X = -1.87, Y = 3.08, Z = 3.15 },
				},
				ConnectedParts = new List<ReferenceElement> { new ReferenceElement(columnData), new ReferenceElement(endPlate) },
			});

			// Four stiffeners in the column, on both sides of its web, each welded to the column at three places.
			PlateData stiffener = AddStiffener(connection, steel, 12, "P2", -2.103, 2.88, 2.75, 1);
			AddWeld(connection, 31, stiffener, columnData, -2, 2.995, 2.76);
			AddWeld(connection, 33, stiffener, columnData, -2.103, 2.90, 2.76);
			AddWeld(connection, 34, stiffener, columnData, -1.897, 2.90, 2.76);

			stiffener = AddStiffener(connection, steel, 13, "P3", -2.103, 2.88, 3.1, 1);
			AddWeld(connection, 32, stiffener, columnData, -2, 2.995, 3.11);
			AddWeld(connection, 35, stiffener, columnData, -2.103, 2.90, 3.11);
			AddWeld(connection, 36, stiffener, columnData, -1.897, 2.90, 3.11);

			stiffener = AddStiffener(connection, steel, 14, "P4", -2.103, 3.12, 2.75, -1);
			AddWeld(connection, 37, stiffener, columnData, -2, 3.005, 2.76);
			AddWeld(connection, 38, stiffener, columnData, -2.103, 3.10, 2.76);
			AddWeld(connection, 39, stiffener, columnData, -1.897, 3.10, 2.76);

			stiffener = AddStiffener(connection, steel, 15, "P5", -2.103, 3.12, 3.1, -1);
			AddWeld(connection, 40, stiffener, columnData, -2, 3.005, 3.11);
			AddWeld(connection, 41, stiffener, columnData, -2.103, 3.10, 3.11);
			AddWeld(connection, 46, stiffener, columnData, -1.897, 3.10, 3.11);

			model.Connections.Add(connection);
		}

		/// <summary>
		/// Adds a horizontal stiffener plate, 206 by 115 mm with two chamfered corners. Its local y and z axes point
		/// along global Y and Z when <paramref name="directionY"/> is 1, and against them when it is -1.
		/// </summary>
		private static PlateData AddStiffener(ConnectionData connection, MatSteel material, int id, string name, double x, double y, double z, int directionY)
		{
			var plate = new PlateData
			{
				Id = id,
				Name = name,
				OriginalModelId = id.ToString(),
				Thickness = 0.02,
				Material = new ReferenceElement(material),
				Origin = new Point3D { X = x, Y = y, Z = z },
				AxisX = new Vector3D { X = 1, Y = 0, Z = 0 },
				AxisY = new Vector3D { X = 0, Y = directionY, Z = 0 },
				AxisZ = new Vector3D { X = 0, Y = 0, Z = directionY },
				Geometry = IomHelpers.CreateOutline(0, 0, 0.206, 0, 0.206, 0.105, 0.195, 0.115, 0.011, 0.115, 0, 0.105),
			};
			connection.Plates.Add(plate);
			return plate;
		}

		/// <summary>
		/// Adds a 4 mm double fillet weld of the plate to the member, at a point where they meet.
		/// </summary>
		private static void AddWeld(ConnectionData connection, int id, PlateData plate, BeamData member, double x, double y, double z)
		{
			connection.Welds.Add(new WeldData
			{
				Id = id,
				ConnectedParts = new List<ReferenceElement> { new ReferenceElement(plate), new ReferenceElement(member) },
				Start = new Point3D { X = x, Y = y, Z = z },
				End = new Point3D { X = x, Y = y, Z = z },
				Thickness = 0.004,
				WeldType = WeldType.DoubleFillet,
			});
		}

		/// <summary>
		/// The connection at node N4, with the members only. The two floor beams M1 and M2 end there, and the middle
		/// column M4 runs through. The engineer designs the connection in IDEA StatiCa Connection.
		/// </summary>
		private static void AddConnectionN4(OpenModel model)
		{
			Member1D leftBeam = model.Member1D.First(m => m.Id == 1);
			Member1D rightBeam = model.Member1D.First(m => m.Id == 2);
			Member1D column = model.Member1D.First(m => m.Id == 4);
			ConnectionPoint connectionPoint = IomHelpers.AddConnectionPoint(model, "N4",
				IomHelpers.Ended(leftBeam), IomHelpers.Ended(rightBeam), IomHelpers.Continuous(column));

			model.Connections.Add(new ConnectionData
			{
				ConnectionPoint = new ReferenceElement(connectionPoint),
				Beams = new List<BeamData>
				{
					IomHelpers.CreateBeamData(leftBeam),
					IomHelpers.CreateBeamData(rightBeam),
					IomHelpers.CreateBeamData(column),
				},
			});
		}
	}
}
