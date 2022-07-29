using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Loading;
using IdeaRS.OpenModel.Material;
using IdeaRS.OpenModel.Model;
using IdeaRS.OpenModel.Result;
using Newtonsoft.Json;

namespace IOM.GeneratorExample
{
	public static class SteelFrameExample
	{
		/// <summary>
		/// Create example of the IOM
		/// </summary>
		/// <returns>Valid open model</returns>
		public static OpenModel CreateIOM()
		{
			OpenModel model = new OpenModel();

			// add setting
			AddSettingsToIOM(model);

			// add nodes
			AddNodesToIOM(model);

			// add materials
			AddMaterialsToIOM(model);

			// add cross section
			AddCrossSectionToIOM(model);

			// add members 1D and connection point
			CreateFrameGeometry(model);

			// add load cases
			AddLoadCasesToIOM(model);

			// add combinations
			AddCombinationsToIOM(model);

			model = CreateConnectionGeometry(model);

			return model;
		}

		static private OpenModel CreateConnectionGeometry(OpenModel openModel)
		{
			//Add connection point
			IdeaRS.OpenModel.Connection.ConnectionPoint connection = new IdeaRS.OpenModel.Connection.ConnectionPoint();

			IdeaRS.OpenModel.Geometry3D.Point3D point = new IdeaRS.OpenModel.Geometry3D.Point3D() { X = -2, Y = 3, Z = 3 };
			point.Id = openModel.GetMaxId(point) + 1;
			point.Name = point.Id.ToString();
			openModel.Point3D.Add(point);

			connection.Node = new ReferenceElement(point);
			connection.Name = point.Name;
			connection.Id = openModel.GetMaxId(connection) + 1;

			//add connection
			openModel.Connections.Add(new IdeaRS.OpenModel.Connection.ConnectionData());

			//add Beams
			openModel.Connections[0].Beams = new List<IdeaRS.OpenModel.Connection.BeamData>();

			//member1D 1
			IdeaRS.OpenModel.Connection.BeamData beam1Data = new IdeaRS.OpenModel.Connection.BeamData
			{
				Name = "M1",
				Id = 1,
				OriginalModelId = "1",
				IsAdded = false,
				MirrorY = false,
				RefLineInCenterOfGravity = false,
			};
			openModel.Connections[0].Beams.Add(beam1Data);

			var member1 = openModel.Member1D.Find(x => x.Id == 1);
			IdeaRS.OpenModel.Connection.ConnectedMember conMb = new IdeaRS.OpenModel.Connection.ConnectedMember
			{
				Id = member1.Id,
				MemberId = new ReferenceElement(member1),
				IsContinuous = false,
			};
			connection.ConnectedMembers.Add(conMb);

			//member1D 3
			var member3 = openModel.Member1D.Find(x => x.Id == 3);
			IdeaRS.OpenModel.Connection.ConnectedMember conMb3 = new IdeaRS.OpenModel.Connection.ConnectedMember
			{
				Id = member3.Id,
				MemberId = new ReferenceElement(member3),
				IsContinuous = true,
			};
			connection.ConnectedMembers.Add(conMb3);

			IdeaRS.OpenModel.Connection.BeamData beam2Data = new IdeaRS.OpenModel.Connection.BeamData
			{
				Name = "M3",
				Id = 3,
				OriginalModelId = member3.Id.ToString(),
				IsAdded = false,
				MirrorY = false,
				RefLineInCenterOfGravity = false,
			};
			openModel.Connections[0].Beams.Add(beam2Data);

			openModel.AddObject(connection);

			//add plate
			IdeaRS.OpenModel.Connection.PlateData plateData = new IdeaRS.OpenModel.Connection.PlateData
			{
				Name = "P1",
				Thickness = 0.02,
				Id = 11,
				Material = "S355",
				OriginalModelId = "11",
				Origin = new IdeaRS.OpenModel.Geometry3D.Point3D
				{
					X = -1.87,
					Y = 2.88,
					Z = 2.7
				},
				AxisX = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = 1,
					Z = 0
				},
				AxisY = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = 0,
					Z = 1
				},
				AxisZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 1,
					Y = 0,
					Z = 0
				},
#pragma warning disable CS0618 // Type or member is obsolete
				Region = "M 0 0 L 0.24 0 L 0.24 0.5 L 0 0.5 L 0 0",
#pragma warning restore CS0618 // Type or member is obsolete
			};

			(openModel.Connections[0].Plates ?? (openModel.Connections[0].Plates = new List<IdeaRS.OpenModel.Connection.PlateData>())).Add(plateData);

			// add cut
			openModel.Connections[0].CutBeamByBeams = new List<IdeaRS.OpenModel.Connection.CutBeamByBeamData>
			{
				new IdeaRS.OpenModel.Connection.CutBeamByBeamData
				{
					CuttingObject = new ReferenceElement(plateData),
					ModifiedObject = new ReferenceElement(beam1Data),
					Orientation = CutOrientation.Parallel,
					WeldType = WeldType.DoubleFillet,
					IsWeld = true,
				}
			};

			IdeaRS.OpenModel.Connection.BoltGrid boltGrid = new IdeaRS.OpenModel.Connection.BoltGrid()
			{
				Id = 41,
				ConnectedPartIds = new List<string>(),
				Diameter = 0.016,
				HeadDiameter = 0.024,
				DiagonalHeadDiameter = 0.026,
				HeadHeight = 0.01,
				BoreHole = 0.018,
				TensileStressArea = 157,
				NutThickness = 0.013,
				AnchorLen = 0.05,
				Material = "8.8",
				Standard = "M 16",
			};

			boltGrid.Origin = new IdeaRS.OpenModel.Geometry3D.Point3D() { X = plateData.Origin.X, Y = plateData.Origin.Y, Z = plateData.Origin.Z };
			boltGrid.AxisX = new IdeaRS.OpenModel.Geometry3D.Vector3D() { X = plateData.AxisX.X, Y = plateData.AxisX.Y, Z = plateData.AxisX.Z };
			boltGrid.AxisY = new IdeaRS.OpenModel.Geometry3D.Vector3D() { X = plateData.AxisY.X, Y = plateData.AxisY.Y, Z = plateData.AxisY.Z };
			boltGrid.AxisZ = new IdeaRS.OpenModel.Geometry3D.Vector3D() { X = plateData.AxisZ.X, Y = plateData.AxisZ.Y, Z = plateData.AxisZ.Z };
			boltGrid.Positions = new List<IdeaRS.OpenModel.Geometry3D.Point3D>
			{
				new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.87,
					Y = 2.92,
					Z = 2.8
				},
				new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.87,
					Y = 3.08,
					Z = 2.8
				},
				new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.87,
					Y = 2.92,
					Z = 3.15
				},
				new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.87,
					Y = 3.08,
					Z = 3.15
				}
			};

			boltGrid.ConnectedPartIds = new List<string>() { beam2Data.OriginalModelId, plateData.OriginalModelId };

			(openModel.Connections[0].BoltGrids ?? (openModel.Connections[0].BoltGrids = new List<IdeaRS.OpenModel.Connection.BoltGrid>())).Add(boltGrid);

			//add plate 2
			IdeaRS.OpenModel.Connection.PlateData plateData2 = new IdeaRS.OpenModel.Connection.PlateData
			{
				Name = "P2",
				Thickness = 0.02,
				Id = 12,
				Material = "S355",
				OriginalModelId = "12",
				Origin = new IdeaRS.OpenModel.Geometry3D.Point3D
				{
					X = -2.103,
					Y = 2.88,
					Z = 2.75
				},
				AxisX = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 1,
					Y = 0,
					Z = 0
				},
				AxisY = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = 1,
					Z = 0
				},
				AxisZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = 0,
					Z = 1
				},
#pragma warning disable CS0618 // Type or member is obsolete
				Region = "M 0 0 L 0.206 0 L 0.206 0.105 L 0.195 0.115 L 0.011 0.115 L 0.0 0.105 L 0 0",
#pragma warning restore CS0618 // Type or member is obsolete
			};

			(openModel.Connections[0].Plates ?? (openModel.Connections[0].Plates = new List<IdeaRS.OpenModel.Connection.PlateData>())).Add(plateData2);
			//add weld between memeber 2 and plate 2 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 31,
				ConnectedPartIds = new List<string>() { plateData2.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2,
					Y = 2.995,
					Z = 2.76
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2,
					Y = 2.995,
					Z = 2.76
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			(openModel.Connections[0].Welds ?? (openModel.Connections[0].Welds = new List<IdeaRS.OpenModel.Connection.WeldData>())).Add(weldData);

			//add weld3 between memeber 2 and plate 2 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData3 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 33,
				ConnectedPartIds = new List<string>() { plateData2.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2.103,
					Y = 2.90,
					Z = 2.76
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2.103,
					Y = 2.90,
					Z = 2.76
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData3);

			//add weld4 between memeber 2 and plate 2 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData4 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 34,
				ConnectedPartIds = new List<string>() { plateData2.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.897,
					Y = 2.90,
					Z = 2.76
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.897,
					Y = 2.90,
					Z = 2.76
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData4);

			//add plate 3
			IdeaRS.OpenModel.Connection.PlateData plateData3 = new IdeaRS.OpenModel.Connection.PlateData
			{
				Name = "P3",
				Thickness = 0.02,
				Id = 13,
				Material = "S355",
				OriginalModelId = "13",
				Origin = new IdeaRS.OpenModel.Geometry3D.Point3D
				{
					X = -2.103,
					Y = 2.88,
					Z = 3.1
				},
				AxisX = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 1,
					Y = 0,
					Z = 0
				},
				AxisY = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = 1,
					Z = 0
				},
				AxisZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = 0,
					Z = 1
				},
#pragma warning disable CS0618 // Type or member is obsolete
				Region = "M 0 0 L 0.206 0 L 0.206 0.105 L 0.195 0.115 L 0.011 0.115 L 0.0 0.105 L 0 0",
#pragma warning restore CS0618 // Type or member is obsolete
			};
			openModel.Connections[0].Plates.Add(plateData3);

			//add weld between memeber 2 and plate 3 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData2 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 32,
				ConnectedPartIds = new List<string>() { plateData3.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2,
					Y = 2.995,
					Z = 3.11
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2,
					Y = 2.995,
					Z = 3.11
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData2);

			//add weld5 between memeber 2 and plate 3 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData5 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 35,
				ConnectedPartIds = new List<string>() { plateData3.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2.103,
					Y = 2.90,
					Z = 3.11
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2.103,
					Y = 2.90,
					Z = 3.11
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData5);

			//add weld6 between memeber 2 and plate 3 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData6 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 36,
				ConnectedPartIds = new List<string>() { plateData3.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.897,
					Y = 2.90,
					Z = 3.11
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.897,
					Y = 2.90,
					Z = 3.11
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData6);

			//add plate 4
			IdeaRS.OpenModel.Connection.PlateData plateData4 = new IdeaRS.OpenModel.Connection.PlateData
			{
				Name = "P4",
				Thickness = 0.02,
				Id = 14,
				Material = "S355",
				OriginalModelId = "14",
				Origin = new IdeaRS.OpenModel.Geometry3D.Point3D
				{
					X = -2.103,
					Y = 3.12,
					Z = 2.75
				},
				AxisX = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 1,
					Y = 0,
					Z = 0
				},
				AxisY = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = -1,
					Z = 0
				},
				AxisZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = 0,
					Z = 1
				},
#pragma warning disable CS0618 // Type or member is obsolete
				Region = "M 0 0 L 0.206 0 L 0.206 0.105 L 0.195 0.115 L 0.011 0.115 L 0.0 0.105 L 0 0",
#pragma warning restore CS0618 // Type or member is obsolete
			};
			openModel.Connections[0].Plates.Add(plateData4);

			//add weld7 between memeber 2 and plate 4 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData7 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 37,
				ConnectedPartIds = new List<string>() { plateData4.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2,
					Y = 3.005,
					Z = 2.76
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2,
					Y = 3.005,
					Z = 2.76
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData7);

			//add weld8 between memeber 2 and plate 4 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData8 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 38,
				ConnectedPartIds = new List<string>() { plateData4.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2.103,
					Y = 3.1,
					Z = 2.76
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2.103,
					Y = 3.1,
					Z = 2.76
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData8);

			//add weld9 between memeber 2 and plate 4 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData9 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 39,
				ConnectedPartIds = new List<string>() { plateData4.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.897,
					Y = 3.1,
					Z = 2.76
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.897,
					Y = 3.1,
					Z = 2.76
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData9);

			//add plate 5
			IdeaRS.OpenModel.Connection.PlateData plateData5 = new IdeaRS.OpenModel.Connection.PlateData
			{
				Name = "P5",
				Thickness = 0.02,
				Id = 15,
				Material = "S355",
				OriginalModelId = "15",
				Origin = new IdeaRS.OpenModel.Geometry3D.Point3D
				{
					X = -2.103,
					Y = 3.12,
					Z = 3.1
				},
				AxisX = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 1,
					Y = 0,
					Z = 0
				},
				AxisY = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = -1,
					Z = 0
				},
				AxisZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
				{
					X = 0,
					Y = 0,
					Z = 1
				},
#pragma warning disable CS0618 // Type or member is obsolete
				Region = "M 0 0 L 0.206 0 L 0.206 0.105 L 0.195 0.115 L 0.011 0.115 L 0.0 0.105 L 0 0",
#pragma warning restore CS0618 // Type or member is obsolete
			};
			openModel.Connections[0].Plates.Add(plateData5);

			//add weld10 between memeber 2 and plate 5 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData10 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 40,
				ConnectedPartIds = new List<string>() { plateData5.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2,
					Y = 3.005,
					Z = 3.11
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2,
					Y = 3.005,
					Z = 3.11
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData10);

			//add weld11 between memeber 2 and plate 5 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData11 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 41,
				ConnectedPartIds = new List<string>() { plateData5.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2.103,
					Y = 3.10,
					Z = 3.11
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -2.103,
					Y = 3.10,
					Z = 3.11
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData11);

			//add weld12 between memeber 2 and plate 5 - stiffener
			IdeaRS.OpenModel.Connection.WeldData weldData12 = new IdeaRS.OpenModel.Connection.WeldData()
			{
				Id = 46,
				ConnectedPartIds = new List<string>() { plateData5.OriginalModelId, beam2Data.OriginalModelId },
				Start = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.897,
					Y = 3.10,
					Z = 3.11
				},
				End = new IdeaRS.OpenModel.Geometry3D.Point3D()
				{
					X = -1.897,
					Y = 3.10,
					Z = 3.11
				},
				Thickness = 0.004,
				WeldType = IdeaRS.OpenModel.Connection.WeldType.DoubleFillet,
			};
			openModel.Connections[0].Welds.Add(weldData12);

			return openModel;
		}


		/// <summary>
		/// Add settings to the IDEA open model
		/// </summary>
		/// <param name="model">Open model</param>
		private static void AddSettingsToIOM(OpenModel model)
		{
			model.OriginSettings = new OriginSettings();

			model.OriginSettings.CrossSectionConversionTable = CrossSectionConversionTable.SCIA;
			model.OriginSettings.CountryCode = CountryCode.ECEN;
			model.OriginSettings.ProjectName = "Project";
			model.OriginSettings.Author = "IDEA StatiCa s.r.o.";
			model.OriginSettings.ProjectDescription = "Training example";
			model.OriginSettings.CheckEquilibrium = true;
		}

		/// <summary>
		/// Add nodes to the IDEA open model
		/// </summary>
		/// <param name="model">Open model</param>
		private static void AddNodesToIOM(OpenModel model)
		{
			Point3D N1 = new Point3D() { X = -2, Y = 3, Z = 0 };
			N1.Name = "N1";
			N1.Id = 1;
			model.AddObject(N1);

			Point3D N2 = new Point3D() { X = -2, Y = 3, Z = 3 };
			N2.Name = "N2";
			N2.Id = 2;
			model.AddObject(N2);

			Point3D N3 = new Point3D() { X = 2, Y = 3, Z = 0 };
			N3.Name = "N3";
			N3.Id = 3;
			model.AddObject(N3);

			Point3D N4 = new Point3D() { X = 2, Y = 3, Z = 3 };
			N4.Name = "N4";
			N4.Id = 4;
			model.AddObject(N4);

			Point3D N5 = new Point3D() { X = 6, Y = 3, Z = 0 };
			N5.Name = "N5";
			N5.Id = 5;
			model.AddObject(N5);

			Point3D N6 = new Point3D() { X = 6, Y = 3, Z = 3 };
			N6.Name = "N6";
			N6.Id = 6;
			model.AddObject(N6);

			Point3D N7 = new Point3D() { X = -2, Y = 3, Z = 6 };
			N7.Name = "N7";
			N7.Id = 7;
			model.AddObject(N7);

			Point3D N8 = new Point3D() { X = 2, Y = 3, Z = 6 };
			N8.Name = "N8";
			N8.Id = 8;
			model.AddObject(N8);

			Point3D N9 = new Point3D() { X = 6, Y = 3, Z = 6 };
			N9.Name = "N9";
			N9.Id = 9;
			model.AddObject(N9);
		}

		/// <summary>
		/// Add materials to the IDEA open model
		/// </summary>
		/// <param name="model">Open model</param>
		private static void AddMaterialsToIOM(OpenModel model)
		{
			MatSteelEc2 material = new MatSteelEc2();

			material.Id = 1;
			material.Name = "S355";
			material.E = 210000000000.00003;
			material.G = material.E / (2 * (1 + 0.3));
			material.Poisson = 0.3;
			material.UnitMass = 7850;
			material.SpecificHeat = 0.6;
			material.ThermalExpansion = 0.000012;
			material.ThermalConductivity = 45;
			material.IsDefaultMaterial = false;
			material.OrderInCode = 0;
			material.StateOfThermalExpansion = ThermalExpansionState.Code;
			material.StateOfThermalConductivity = ThermalConductivityState.Code;
			material.StateOfThermalSpecificHeat = ThermalSpecificHeatState.Code;
			material.StateOfThermalStressStrain = ThermalStressStrainState.Code;
			material.StateOfThermalStrain = ThermalStrainState.Code;
			material.fy = 355000000.00000006;
			material.fu = 510000000.00000006;
			material.fy40 = 335000000.00000006;
			material.fu40 = 470000000.00000006;
			material.DiagramType = SteelDiagramType.Bilinear;

			model.AddObject(material);
		}

		/// <summary>
		/// Add cross section to the IDEA open model
		/// </summary>
		/// <param name="model">Open model</param>
		private static void AddCrossSectionToIOM(OpenModel model)
		{
			// only one material is in the model
			MatSteel material = model.MatSteel.FirstOrDefault();

			// create first cross section
			CrossSectionParameter css1 = Helpers.CreateCSSParameter(1, "HE200B", material);

			// create second cross section
			CrossSectionParameter css2 = Helpers.CreateCSSParameter(2, "HE240B", material);

			// add cross sections to the model
			model.AddObject(css1);
			model.AddObject(css2);
		}

		/// <summary>
		/// Add members and connection points to the IDEA open model
		/// </summary>
		/// <param name="model">Open model</param>
		private static void CreateFrameGeometry(OpenModel model)
		{
			// find appropriate cross sections
			var css_he_240b = model.CrossSection.FirstOrDefault(item => item.Name == "HE240B");
			var css_he_200b = model.CrossSection.FirstOrDefault(item => item.Name == "HE200B");

			// member for left floor beam
			ConnectedMember M1 = Helpers.CreateMember(model, 1, Member1DType.Beam, css_he_200b, "N2", "N4");

			// member for right floor beam
			ConnectedMember M2 = Helpers.CreateMember(model, 2, Member1DType.Beam, css_he_200b, "N4", "N6");

			// member for left column
			ConnectedMember M3 = Helpers.CreateMember(model, 3, Member1DType.Column, css_he_240b, "N1", "N2", "N7");

			// member for middle column
			ConnectedMember M4 = Helpers.CreateMember(model, 4, Member1DType.Column, css_he_240b, "N3", "N4", "N8");

			// member for right column
			ConnectedMember M5 = Helpers.CreateMember(model, 5, Member1DType.Column, css_he_240b, "N5", "N6", "N9");

			// member for upper continuous beam
			ConnectedMember M6 = Helpers.CreateMember(model, 6, Member1DType.Beam, css_he_200b, "N7", "N8", "N9");

			// add members to the model
			model.AddObject(M1);
			model.AddObject(M2);
			model.AddObject(M3);
			model.AddObject(M4);
			model.AddObject(M5);
			model.AddObject(M6);

			// create first connection point
			ConnectionPoint CP1 = new ConnectionPoint();

			CP1.Node = new ReferenceElement(model.Point3D.FirstOrDefault(n => n.Name == "N2"));
			CP1.Id = model.GetMaxId(CP1) + 1;
			CP1.Name = "CON " + CP1.Id.ToString();

			CP1.ConnectedMembers.Add(M1);
			CP1.ConnectedMembers.Add(M3);

			model.AddObject(CP1);

			// create second connection point
			ConnectionPoint CP2 = new ConnectionPoint();

			CP2.Node = new ReferenceElement(model.Point3D.FirstOrDefault(n => n.Name == "N4"));
			CP2.Id = model.GetMaxId(CP2) + 1;
			CP2.Name = "CON " + CP2.Id.ToString();

			CP2.ConnectedMembers.Add(M1);
			CP2.ConnectedMembers.Add(M2);
			CP2.ConnectedMembers.Add(M4);

			model.AddObject(CP2);
		}

		/// <summary>
		/// Add load cases to the IDEA open model
		/// </summary>
		/// <param name="model">Open model</param>
		private static void AddLoadCasesToIOM(OpenModel model)
		{
			// create LG1
			LoadGroupEC LG1 = new LoadGroupEC(); ;

			LG1.Id = 1;
			LG1.Name = "PERM1";
			LG1.Relation = Relation.Standard;
			LG1.GroupType = LoadGroupType.Permanent;
			LG1.GammaQ = 1.35;
			LG1.Dzeta = 0.85;
			LG1.GammaGInf = 1;
			LG1.GammaGSup = 1.35;
			model.AddObject(LG1);

			// create LC1
			LoadCase LC1 = new LoadCase();

			LC1.Id = 1;
			LC1.Name = "SelfWeight";
			LC1.LoadType = LoadCaseType.Permanent;
			LC1.Type = LoadCaseSubType.PermanentStandard;
			LC1.Variable = VariableType.Standard;
			LC1.LoadGroup = new ReferenceElement(LG1);

			// create LC2
			LoadCase LC2 = new LoadCase();

			LC2.Id = 2;
			LC2.Name = "PernamentLoading";
			LC2.LoadType = LoadCaseType.Permanent;
			LC2.Type = LoadCaseSubType.PermanentStandard;
			LC2.Variable = VariableType.Standard;
			LC2.LoadGroup = new ReferenceElement(LG1);

			// create LG2
			LoadGroupEC LG2 = new LoadGroupEC(); ;

			LG2.Id = 2;
			LG2.Name = "VAR1";
			LG2.Relation = Relation.Exclusive;
			LG2.GroupType = LoadGroupType.Variable;
			LG2.GammaQ = 1.5;
			LG2.Dzeta = 0.85;
			LG2.GammaGInf = 0;
			LG2.GammaGSup = 1.5;
			LG2.Psi0 = 0.7;
			LG2.Psi1 = 0.5;
			LG2.Psi2 = 0.3;
			model.AddObject(LG2);

			// create LC3
			LoadCase LC3 = new LoadCase();

			LC3.Id = 3;
			LC3.Name = "LiveLoad";
			LC3.LoadType = LoadCaseType.Variable;
			LC3.Type = LoadCaseSubType.VariableStatic;
			LC3.Variable = VariableType.Standard;
			LC3.LoadGroup = new ReferenceElement(LG2);

			// add load cases
			model.AddObject(LC1);
			model.AddObject(LC2);
			model.AddObject(LC3);
		}

		/// <summary>
		/// Add combinations to the IDEA open model
		/// </summary>
		/// <param name="model">OpenModel</param>
		private static void AddCombinationsToIOM(OpenModel model)
		{
			// create first combination input
			CombiInputEC CI1 = new CombiInputEC();

			CI1.Id = model.GetMaxId(CI1) + 1;
			CI1.Name = "Co.#1";
			CI1.Description = "SelfWeight + PernamentLoading + LiveLoad";
			CI1.TypeCombiEC = TypeOfCombiEC.ULS;
			CI1.TypeCalculationCombi = TypeCalculationCombiEC.Linear;

			CombiItem item = new CombiItem();
			item.Id = 1;
			item.Coeff = 1;
			item.LoadCase = new ReferenceElement(model.LoadCase.FirstOrDefault(l => l.Name == "SelfWeight"));
			CI1.Items.Add(item);

			item = new CombiItem();
			item.Id = 2;
			item.Coeff = 1;
			item.LoadCase = new ReferenceElement(model.LoadCase.FirstOrDefault(l => l.Name == "PernamentLoading"));
			CI1.Items.Add(item);

			item = new CombiItem();
			item.Id = 3;
			item.Coeff = 1;
			item.LoadCase = new ReferenceElement(model.LoadCase.FirstOrDefault(l => l.Name == "LiveLoad"));
			CI1.Items.Add(item);

			model.AddObject(CI1);

			// create second combination input
			CombiInputEC CI2 = new CombiInputEC();

			CI2.Id = model.GetMaxId(CI2) + 1;
			CI2.Name = "Co.#2";
			CI2.Description = "SelfWeight";
			CI2.TypeCombiEC = TypeOfCombiEC.ULS;
			CI2.TypeCalculationCombi = TypeCalculationCombiEC.Linear;

			item = new CombiItem();
			item.Id = 1;
			item.Coeff = 1;
			item.LoadCase = new ReferenceElement(model.LoadCase.FirstOrDefault(l => l.Name == "SelfWeight"));
			CI2.Items.Add(item);

			model.AddObject(CI2);
		}

		public static readonly string viewerURL = "https://viewer.ideastatica.com";

		public static void CreateOnServer(OpenModel model, OpenModelResult openModelResult, string path)
		{
			IdeaRS.OpenModel.OpenModelContainer openModelContainer = new OpenModelContainer()
			{
				OpenModel = model,
				OpenModelResult = openModelResult,
			};

			// serialize IOM to XML
			var stringwriter = new System.IO.StringWriter();
			var serializer = new XmlSerializer(typeof(OpenModelContainer));
			serializer.Serialize(stringwriter, openModelContainer);

			var serviceUrl = viewerURL + "/ConnectionViewer/CreateFromIOM";

			Console.WriteLine("Posting iom in xml to the service {0}", serviceUrl);
			var resultMessage = Helpers.PostXMLData(serviceUrl, stringwriter.ToString());

			ResponseMessage responseMessage = JsonConvert.DeserializeObject<ResponseMessage>(resultMessage);
			Console.WriteLine("Service response is : '{0}'", responseMessage.status);
			if (responseMessage.status == "OK")
			{
				byte[] dataBuffer = Convert.FromBase64String(responseMessage.fileContent);
				Console.WriteLine("Writing {0} bytes to file '{1}'", dataBuffer.Length, path);
				if (dataBuffer.Length > 0)
				{
					using (FileStream fileStream = new FileStream(path
				, FileMode.Create
				, FileAccess.Write))
					{
						fileStream.Write(dataBuffer, 0, dataBuffer.Length);
					}
				}
				else
				{
					Console.WriteLine("The service returned no data");
				}
			}
		}
	}

	class ResponseMessage
	{
		public string status { get; set; }
		public string fileContent { get; set; }
	}
}
