using IdeaStatiCa.BimApi;
using IdeaStatiCa.BimApiLink.Identifiers;
using IdeaStatiCa.BimApiLink.Utils;
using IdeaStatiCa.Plugin;
using IdeaStatiCa.TeklaStructuresPlugin.BimApi;
using IdeaStatiCa.TeklaStructuresPlugin.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TS = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using TSV = Tekla.Structures.TeklaStructuresSettings;

namespace IdeaStatiCa.TeklaStructuresPlugin.Importers
{
	internal class AnchorGridImporter : BaseImporter<IIdeaAnchorGrid>
	{
		private static readonly string BoltGradeKey = "GRADE";
		private static readonly string BoltDiameterKey = "DIAMETER";
		private static readonly string BoltAssemblyNameKey = "TYPE";

		public AnchorGridImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaAnchorGrid Create(string id)
		{
			var ids = id.Split(';');

			var item = Model.GetItemByHandler(ids.First());
			if (item is TS.Part anchor2)
			{

				var anchorGrid = new AnchorGrid(anchor2.Identifier.GUID.ToString())
				{
					BoltShearType = IdeaRS.OpenModel.Parameters.BoltShearType.Interaction,
					ConnectedParts = new List<IIdeaObjectConnectable>(),
					Positions = new List<IIdeaNode>(),
					AnchorType = IdeaRS.OpenModel.Parameters.AnchorType.Straight, //check washer etc.
					AnchoringLength = 0,
					HookLength = 0,
					WasherSize = 0,
					ShearInThread = false,
				};

				//add concrete block
				for (int i = 1; i < ids.Count(); i++)
				{
					IIdeaConcreteBlock cb = null;
					// find concrete block
					if (!string.IsNullOrEmpty(ids[i]))
					{
						cb = Get<IIdeaConcreteBlock>(new StringIdentifier<IIdeaConcreteBlock>(ids[i]));

						if (cb != null)
						{
							PlugInLogger.LogInformation($"Add concrete block {cb.Name}");
							anchorGrid.ConcreteBlock = cb;
							break;
						}
					}
				}


				var father = anchor2.GetFatherComponent();

				if (father is TS.Detail detail)
				{
					List<TS.BoltGroup> bolts = new List<TS.BoltGroup>();
					List<TS.Part> anchors = new List<TS.Part>();
					TS.Part nut = null;
					TS.Part washer = null;
					List<TS.Part> plateWasherList = new List<TS.Part>();

					var anchorAssemblies = new List<TS.Assembly>();
					var anchorObjects = new List<List<TS.Part>>();

					var detailCoordSystem = detail.GetCoordinateSystem();

					// Set workplane to design plane
					var column = detail.GetPrimaryObject() as TS.Part;

					if (column != null)
					{
						var columnAssemblyParts = column.GetAssembly().GetSecondaries().Cast<TS.ModelObject>();
						var columnCoordSys = column.GetCoordinateSystem();
						var yAxis = TSG.Vector.Cross(columnCoordSys.AxisX, columnCoordSys.AxisY);
						var designPlane = new TS.TransformationPlane(detailCoordSystem.Origin, columnCoordSys.AxisY, yAxis);

						// Find bolts and anchors from the detail
						foreach (var mo in detail.GetChildren())
						{
							string type = string.Empty;
							if (mo is TS.Part part)
							{
								var fc = part.GetFatherComponent();
								if (IdentifierHelper.AnchorMemberFilter(part))
								{
									anchors.Add(part);
								}

								if (IdentifierHelper.NutMemberFilter(part))
								{
									nut = part;
								}

								if (IdentifierHelper.WasherMemberFilter(part))
								{
									washer = part;
								}

								if (IdentifierHelper.PlateWasherMemberFilter(part))
								{
									plateWasherList.Add(part);
								}
							}

							if (mo is TS.BoltGroup bolt)
							{
								var boltfather = bolt.GetFatherComponent();
								bolts.Add(bolt);

							}
						}

						// Get anchor assemblies, may return 1..n assemblies
						if (anchors.Count > 0 && bolts.Count > 0)
						{
							anchorAssemblies = GetAnchorAssemblies(anchors);
						}

						var boltGroup = bolts.First();
						var anchorF = anchors.First();

						anchorGrid.BoltAssembly = GetAssembly(bolts.First(), anchorF, nut, washer);
						anchorGrid.Length = GetAnchorLen(anchorF).MilimetersToMeters();

						//This test due to plate as member and we are not sure if its imported as plate or member
						CheckAndAddConnectedObject<IIdeaPlate>(boltGroup.PartToBoltTo, anchorGrid);
						CheckAndAddConnectedObject<IIdeaMember1D>(boltGroup.PartToBoltTo, anchorGrid);

						CheckAndAddConnectedObject<IIdeaPlate>(boltGroup.PartToBeBolted, anchorGrid);
						CheckAndAddConnectedObject<IIdeaMember1D>(boltGroup.PartToBeBolted, anchorGrid);
						foreach (var plateWasher in plateWasherList)
						{
							if (plateWasher != null)
							{
								CheckAndAddConnectedObject<IIdeaPlate>(plateWasher, anchorGrid);
							}
						}
						if (boltGroup.OtherPartsToBolt != null)
						{
							foreach (var obj in boltGroup.OtherPartsToBolt)
							{
								if (!(obj is TS.Part otherPart))
								{
									continue;
								}
								CheckAndAddConnectedObject<IIdeaPlate>(otherPart, anchorGrid);
								CheckAndAddConnectedObject<IIdeaMember1D>(otherPart, anchorGrid);
							}
						}

						var boltCs = boltGroup.GetCoordinateSystem();
						var boltAxisZ = TSG.Vector.Cross(boltCs.AxisX, boltCs.AxisY);

						anchorGrid.OriginNo = Model.GetPointId(boltCs.Origin);
						anchorGrid.LocalCoordinateSystem = new IdeaRS.OpenModel.Geometry3D.CoordSystemByVector()
						{
							VecX = new IdeaRS.OpenModel.Geometry3D.Vector3D
							{
								X = boltCs.AxisX.X,
								Y = boltCs.AxisX.Y,
								Z = boltCs.AxisX.Z
							},
							VecY = new IdeaRS.OpenModel.Geometry3D.Vector3D
							{
								X = boltCs.AxisY.X,
								Y = boltCs.AxisY.Y,
								Z = boltCs.AxisY.Z
							},
							VecZ = new IdeaRS.OpenModel.Geometry3D.Vector3D
							{
								X = boltAxisZ.X,
								Y = boltAxisZ.Y,
								Z = boltAxisZ.Z

							}
						};

						foreach (var assembly in anchorAssemblies)
						{
							// Get all parts from the assembly including main part
							var parts = GetAllPartsFromAssembly(assembly);

							foreach (var bolt in bolts)
							{
								if (bolt.BoltPositions.Count > 1)
								{
									foreach (var point in bolt.BoltPositions)
									{
										//Find anchor near the bolt hole position point
										var anchor = FindNearestAnchor(parts, point as TSG.Point, "AnchorBolt");

										if (anchor != null)
										{
											// Print anchor information
											//PrintAnchorInformation(anchor);

											var p = point as TSG.Point;
											var pointId = Model.GetPointId(p);
											(anchorGrid.Positions as List<IIdeaNode>).Add(Get<IIdeaNode>(pointId));
											break;
										}
									}
									break;
								}
							}
						}
						return anchorGrid;
					}
				}
			}

			return null;
		}

		private double GetAnchorLen(TS.Part anchor)
		{
			double embedmentDept = 0.0;
			PlugInLogger.LogDebug("GetAnchorLen - Try to read EmbedmentDepth");
			anchor.GetUserProperty("EmbedmentDepth", ref embedmentDept);

			if (embedmentDept > 0)
			{
				PlugInLogger.LogDebug($"GetAnchorLen - found EmbedmentDepth {embedmentDept}");
				return embedmentDept;
			}
			else
			{
				embedmentDept = MemberHelper.GetPartLength(anchor);
				PlugInLogger.LogDebug($"GetAnchorLen - found EmbedmentDepth {embedmentDept}");
				return embedmentDept;
			}
		}

		/// <summary>
		/// Find anchor nearest to the given bolt position point
		/// </summary>
		/// <param name="parts">The part list where to check from</param>
		/// <param name="point">The point to check against</param>
		/// <param name="type">The type to check against</param>
		/// <returns></returns>
		private static TS.Part FindNearestAnchor(List<TS.Part> parts, TSG.Point point, string type)
		{
			foreach (var part in parts)
			{
				if (IdentifierHelper.AnchorMemberFilter(part))
				{
					var anchorSys = part.GetCoordinateSystem();
					var line = new TSG.Line(anchorSys.Origin, anchorSys.Origin + anchorSys.AxisX);

					// Check distance between bolt position point and anchor coordinate system
					if (TSG.Distance.PointToLine(point, line) < 1.0)
					{
						return part;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Get parts from assembly, main part as first part
		/// </summary>
		/// <param name="assembly">The assembly where to check from</param>
		private static List<TS.Part> GetAllPartsFromAssembly(TS.Assembly assembly)
		{
			var parts = new List<TS.Part>
						{
							assembly.GetMainPart() as TS.Part
						};

			foreach (TS.ModelObject modelObject in assembly.GetSecondaries())
			{
				if (modelObject is TS.Part part)
				{
					parts.Add(part);
				}
			}

			return parts;
		}

		/// <summary>
		/// Get assemblies from anchor 
		/// </summary>
		/// <param name="anchors">The list of anchors</param>
		private static List<TS.Assembly> GetAnchorAssemblies(List<TS.Part> anchors)
		{
			var assemblyIDs = new List<int>();
			var anchorAssemblies = new List<TS.Assembly>();
			foreach (TS.Part anchor in anchors)
			{
				var assembly = anchor.GetAssembly();
				assembly.Select();
				if (!assemblyIDs.Contains(assembly.Identifier.ID))
				{
					anchorAssemblies.Add(assembly);
					assemblyIDs.Add(assembly.Identifier.ID);
				}
			}

			return anchorAssemblies;
		}

		private void CheckAndAddConnectedObject<T>(TS.Part part, AnchorGrid boltGrid)
			where T : IIdeaObjectConnectable
		{
			IIdeaObject ideaObject = CheckMaybe<T>(part.Identifier.GUID.ToString());
			if (ideaObject != null)
			{
				IIdeaObjectConnectable mainObject = GetMaybe<T>(part.Identifier.GUID.ToString());
				if (mainObject != null)
				{
					(boltGrid.ConnectedParts as List<IIdeaObjectConnectable>).Add(mainObject);
				}
			}
		}

		private static double ExtractNumber(string input)
		{
			Match match = Regex.Match(input, @"\d+(\.\d+)?"); // Find first sequence of digits, allowing a decimal

			if (match.Success)
			{
				return double.Parse(match.Value, CultureInfo.InvariantCulture); // Convert found number to double
			}

			return 0.0; // Return 0.0 if no number found
		}

		private IIdeaBoltAssembly GetAssembly(TS.BoltGroup boltGroup, TS.Part anchor, TS.Part nut, TS.Part washer)
		{
			bool isImperialUnitPresented = false;
			TSV.GetAdvancedOption("XS_IMPERIAL", ref isImperialUnitPresented);

			var stringPropTable = new Hashtable();
			boltGroup.GetStringReportProperties(new ArrayList
				{
					BoltGradeKey,
					BoltAssemblyNameKey
				}, ref stringPropTable);

			string boltAssemblyName = (string)stringPropTable[BoltAssemblyNameKey];
			string boltGrade = (string)stringPropTable[BoltGradeKey] ?? boltAssemblyName;

			var doublePropTable = new Hashtable();
			ArrayList dNames = new ArrayList();
			dNames.Add(BoltDiameterKey);
			dNames.Add("NUT.INNER_DIAMETER");
			dNames.Add("HEAD_DIAMETER");
			dNames.Add("THICKNESS");
			dNames.Add("NUT.THICKNESS");
			dNames.Add("NUT.OUTER_DIAMETER");
			dNames.Add("WASHER.THICKNESS1");
			dNames.Add("WASHER.THICKNESS2");

			boltGroup.GetDoubleReportProperties(dNames, ref doublePropTable);


			/*ArrayList sNames = new ArrayList();
			sNames.Add("NAME");
			sNames.Add("SCREW_NAME");
			sNames.Add("SCREW_TYPE");
			sNames.Add("TYPE");
			sNames.Add("TYPE1");
			sNames.Add("TYPE2");
			sNames.Add("TYPE3");
			sNames.Add("TYPE4");
			sNames.Add("STANDARD");
			sNames.Add("SHORT_NAME");
			sNames.Add("MATERIAL");
			sNames.Add("FINISH");
			sNames.Add("GRADE");
			ArrayList iNames = new ArrayList();
			iNames.Add("DATE");
			iNames.Add("FATHER_ID");
			iNames.Add("GROUP_ID");
			iNames.Add("HIERARCHY_LEVEL");
			iNames.Add("MODEL_TOTAL");*/
			//ArrayList dNames = new ArrayList();
			/*dNames.Add("EXTRA_LENGTH");
			dNames.Add("FLANGE_THICKNESS");
			dNames.Add("FLANGE_WIDTH");
			dNames.Add("HEIGHT");
			dNames.Add("LENGTH");
			dNames.Add("PRIMARYWEIGHT");
			dNames.Add("PROFILE_WEIGHT");
			dNames.Add("ROUNDING_RADIUS");
			dNames.Add("LENGTH");
			dNames.Add("DIAMETER");
			dNames.Add("WEIGHT");


			dNames.Add("WASHER.THICKNESS");
			dNames.Add("WASHER.INNER_DIAMETER");
			dNames.Add("WASHER.OUTER_DIAMETER");

			dNames.Add("WASHER.INNER_DIAMETER1");
			dNames.Add("WASHER.OUTER_DIAMETER1");

			dNames.Add("WASHER.INNER_DIAMETER2");
			dNames.Add("WASHER.OUTER_DIAMETER2");
			dNames.Add("NUT.THICKNESS2");
			dNames.Add("NUT.OUTER_DIAMETER2");*/

			/*
			dNames.Add("NUT.INNER_DIAMETER");
			dNames.Add("HEAD_DIAMETER");
			dNames.Add("THICKNESS");
			dNames.Add("NUT.THICKNESS");
			dNames.Add("NUT.OUTER_DIAMETER");
			dNames.Add("WASHER.THICKNESS1");
			dNames.Add("WASHER.THICKNESS2");*/

			/*Hashtable sValues = new Hashtable(dNames.Count);
			if (boltGroup.GetAllReportProperties(sNames, dNames, iNames, ref sValues))
			{
				foreach (DictionaryEntry value in sValues)
					Console.WriteLine(value.Key.ToString() + " : " + value.Value.ToString());
			}*/

			double rodsize = 0.0;
			anchor.GetUserProperty("RodSize", ref rodsize);
			PlugInLogger.LogDebug($"GetAssembly - found RodSize from GetUserProperty {rodsize}");
			rodsize = rodsize.MilimetersToMeters();
			if (rodsize <= 0.0)
			{
				rodsize = ExtractNumber(anchor.Profile.ProfileString).MilimetersToMeters();
				if (rodsize <= 0.0)
				{
					rodsize = ((double)doublePropTable["NUT.INNER_DIAMETER"]).MilimetersToMeters();
					PlugInLogger.LogDebug($"GetAssembly - found R RodSize from NUT.INNER_DIAMETE {rodsize}");
					if (rodsize <= 0.0)
					{
						rodsize = ((double)doublePropTable[BoltDiameterKey]).MilimetersToMeters();
						PlugInLogger.LogDebug($"GetAssembly - found RodSize from {BoltDiameterKey} {rodsize}");
					}
				}
			}
			var headDiameter = ((double)doublePropTable["HEAD_DIAMETER"]).MilimetersToMeters();
			if (headDiameter <= 0.0)
			{
				headDiameter = rodsize * 1.7;
			}
			var headHeight = ((double)doublePropTable["THICKNESS"]).MilimetersToMeters();
			if (headHeight <= 0.0)
			{
				headHeight = 0.6 * rodsize;
			}

			var nutThickness = 0.0;
			if (nut != null)
			{
				nutThickness = MemberHelper.GetPartLength(nut).MilimetersToMeters();
			}
			else
			{
				nutThickness = ((double)doublePropTable["NUT.THICKNESS"]).MilimetersToMeters();
				if (nutThickness <= 0.0)
				{
					nutThickness = 0.6 * rodsize;
				}
			}

			var boreHole = ((double)doublePropTable[BoltDiameterKey]).MilimetersToMeters();
			if (boreHole <= 0 || rodsize <= boreHole)
			{
				boreHole = rodsize + 0.001;
			}
			var diagonalHeadDiameter = ((double)doublePropTable["NUT.OUTER_DIAMETER"]).MilimetersToMeters();
			if (diagonalHeadDiameter <= 0.0)
			{
				diagonalHeadDiameter = rodsize * 1.7;
			}

			var washerAtHead = false;
			var washerAtNut = false;
			var washerThickness = 0.0;
			if (washer != null)
			{
				washerThickness = MemberHelper.GetPartLength(washer).MilimetersToMeters();
				washerAtHead = ((double)doublePropTable["WASHER.THICKNESS1"]) > 0;
				washerAtNut = ((double)doublePropTable["WASHER.THICKNESS2"]) > 0;
				if (washerThickness <= 0.0)
				{
					washerThickness = ((double)doublePropTable["WASHER.THICKNESS1"]).MilimetersToMeters();
				}
			}


			return new BoltAssembly(boltGroup.Identifier.GUID.ToString())
			{
				BoreHole = boreHole,
				DiagonalHeadDiameter = diagonalHeadDiameter,
				Diameter = rodsize,
				NutThickness = nutThickness,
				HeadDiameter = headDiameter,
				HeadHeight = headHeight,
				HoleDiameter = boreHole,
				Standard = string.Empty,
				TensileStressArea = 0.0,
				BoltGradeNo = boltGrade,
				Name = $"{(isImperialUnitPresented ? rodsize.MetersToInchesFormated() : doublePropTable[BoltDiameterKey])} {boltAssemblyName}",
				WasherAtHead = washerAtHead,
				WasherAtNut = washerAtNut,
				WasherThickness = washerThickness,

			};
		}
	}
}