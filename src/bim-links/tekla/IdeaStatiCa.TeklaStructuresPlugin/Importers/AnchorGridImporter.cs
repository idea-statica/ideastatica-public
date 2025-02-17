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
		public AnchorGridImporter(IModelClient model, IPluginLogger plugInLogger)
			: base(model, plugInLogger)
		{
		}

		public override IIdeaAnchorGrid Create(string id)
		{
			PlugInLogger.LogInformation("Create - Processing ID: " + id);

			string[] ids = id.Split(';');
			var item = Model.GetItemByHandler(ids.First());

			if (!(item is TS.Part anchor2))
			{
				PlugInLogger.LogWarning("Create - No valid anchor found.");
				return null;
			}

			PlugInLogger.LogInformation("Create - Found anchor with GUID: " + anchor2.Identifier.GUID);

			var anchorGrid = new AnchorGrid(anchor2.Identifier.GUID.ToString())
			{
				BoltShearType = IdeaRS.OpenModel.Parameters.BoltShearType.Interaction,
				ConnectedParts = new List<IIdeaObjectConnectable>(),
				Positions = new List<IIdeaNode>(),
				AnchorType = IdeaRS.OpenModel.Parameters.AnchorType.Straight,
				AnchoringLength = 0,
				HookLength = 0,
				WasherSize = 0,
				ShearInThread = false
			};

			// Try to find concrete block
			for (int i = 1; i < ids.Length; i++)
			{
				if (!string.IsNullOrEmpty(ids[i]))
				{
					IIdeaConcreteBlock cb = Get<IIdeaConcreteBlock>(new StringIdentifier<IIdeaConcreteBlock>(ids[i]));
					if (cb != null)
					{
						PlugInLogger.LogInformation("Create - Found concrete block: " + cb.Name);
						anchorGrid.ConcreteBlock = cb;
						break;
					}
				}
			}

			var father = anchor2.GetFatherComponent();
			if (!(father is TS.Detail detail))
			{
				PlugInLogger.LogWarning("Create - Anchor has no valid father component.");
				return null;
			}

			PlugInLogger.LogInformation("Create - Processing detail component: " + detail.Identifier.ID);

			List<TS.BoltGroup> bolts = new List<TS.BoltGroup>();
			List<TS.Part> anchors = new List<TS.Part>();
			TS.Part nut = null;
			TS.Part washer = null;
			List<TS.Part> plateWashers = new List<TS.Part>();

			foreach (var mo in detail.GetChildren())
			{
				if (mo is TS.Part part)
				{
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
						plateWashers.Add(part);
					}
				}
				else if (mo is TS.BoltGroup bolt)
				{
					bolts.Add(bolt);
				}
			}

			if (anchors.Count == 0 || bolts.Count == 0)
			{
				PlugInLogger.LogWarning("Create - No anchors or bolts found.");
				return null;
			}

			PlugInLogger.LogInformation("Create - Found " + anchors.Count + " anchors and " + bolts.Count + " bolts.");

			List<TS.Assembly> anchorAssemblies = GetAnchorAssemblies(anchors);
			TS.BoltGroup boltGroup = bolts.First();
			TS.Part anchorF = anchors.First();

			anchorGrid.BoltAssembly = GetAssembly(boltGroup, anchorF, nut, washer);
			anchorGrid.Length = GetAnchorLen(anchorF).MilimetersToMeters();

			// Add connected objects
			AddConnectedObjects(boltGroup, plateWashers, anchorGrid);

			// Set local coordinate system
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

			// Find nearest anchors for bolt positions
			foreach (var assembly in anchorAssemblies)
			{
				List<TS.Part> parts = GetAllPartsFromAssembly(assembly);
				foreach (var bolt in bolts)
				{
					foreach (var point in bolt.BoltPositions)
					{
						if (point is TSG.Point boltPoint)
						{
							TS.Part anchor = FindNearestAnchor(parts, boltPoint, "AnchorBolt");
							if (anchor != null)
							{
								string pointId = Model.GetPointId(boltPoint);
								(anchorGrid.Positions as List<IIdeaNode>).Add(Get<IIdeaNode>(pointId));
								break;
							}
						}
					}
				}
			}

			PlugInLogger.LogInformation("Create - Successfully created AnchorGrid for anchor " + anchor2.Identifier.GUID);
			return anchorGrid;
		}


		// Helper method to add connected objects
		private void AddConnectedObjects(TS.BoltGroup boltGroup, List<TS.Part> plateWashers, AnchorGrid anchorGrid)
		{
			PlugInLogger.LogInformation("AddConnectedObjects - Adding connected parts.");

			CheckAndAddConnectedObject<IIdeaPlate>(boltGroup.PartToBoltTo, anchorGrid);
			CheckAndAddConnectedObject<IIdeaMember1D>(boltGroup.PartToBoltTo, anchorGrid);
			CheckAndAddConnectedObject<IIdeaPlate>(boltGroup.PartToBeBolted, anchorGrid);
			CheckAndAddConnectedObject<IIdeaMember1D>(boltGroup.PartToBeBolted, anchorGrid);

			foreach (var plateWasher in plateWashers)
			{
				CheckAndAddConnectedObject<IIdeaPlate>(plateWasher, anchorGrid);
			}

			if (boltGroup.OtherPartsToBolt != null)
			{
				foreach (var obj in boltGroup.OtherPartsToBolt)
				{
					if (obj is TS.Part otherPart)
					{
						CheckAndAddConnectedObject<IIdeaPlate>(otherPart, anchorGrid);
						CheckAndAddConnectedObject<IIdeaMember1D>(otherPart, anchorGrid);
					}
				}
			}

			PlugInLogger.LogInformation("AddConnectedObjects - Finished adding connected parts.");
		}

		private double GetAnchorLen(TS.Part anchor)
		{
			double embedmentDepth = 0.0;
			PlugInLogger.LogDebug("GetAnchorLen - Attempting to read EmbedmentDepth.");

			if (anchor.GetUserProperty("EmbedmentDepth", ref embedmentDepth) && embedmentDepth > 0)
			{
				PlugInLogger.LogDebug($"GetAnchorLen - Found EmbedmentDepth: {embedmentDepth}");
				return embedmentDepth;
			}

			embedmentDepth = MemberHelper.GetPartLength(anchor);
			PlugInLogger.LogDebug($"GetAnchorLen - Calculated EmbedmentDepth: {embedmentDepth}");
			return embedmentDepth;
		}

		/// <summary>
		/// Finds the anchor nearest to the given bolt position.
		/// </summary>
		private TS.Part FindNearestAnchor(List<TS.Part> parts, TSG.Point point, string type)
		{
			PlugInLogger.LogDebug("FindNearestAnchor - Searching for the nearest anchor.");

			foreach (var part in parts)
			{
				if (!IdentifierHelper.AnchorMemberFilter(part))
					continue;

				var anchorSys = part.GetCoordinateSystem();
				var line = new TSG.Line(anchorSys.Origin, anchorSys.Origin + anchorSys.AxisX);
				double distance = TSG.Distance.PointToLine(point, line);

				PlugInLogger.LogDebug($"FindNearestAnchor - Distance to line: {distance}");
				if (distance < 1.0)
				{
					PlugInLogger.LogDebug("FindNearestAnchor - Found a matching anchor.");
					return part;
				}
			}

			PlugInLogger.LogDebug("FindNearestAnchor - No anchor found.");
			return null;
		}

		/// <summary>
		/// Retrieves all parts from an assembly, ensuring the main part is listed first.
		/// </summary>
		private List<TS.Part> GetAllPartsFromAssembly(TS.Assembly assembly)
		{
			PlugInLogger.LogDebug("GetAllPartsFromAssembly - Retrieving all parts from assembly.");

			var parts = new List<TS.Part> { assembly.GetMainPart() as TS.Part };
			foreach (TS.ModelObject modelObject in assembly.GetSecondaries())
			{
				if (modelObject is TS.Part part)
				{
					parts.Add(part);
				}
			}

			PlugInLogger.LogDebug($"GetAllPartsFromAssembly - Found {parts.Count} parts.");
			return parts;
		}

		/// <summary>
		/// Gets unique assemblies from a list of anchor parts.
		/// </summary>
		private List<TS.Assembly> GetAnchorAssemblies(List<TS.Part> anchors)
		{
			var assemblyIDs = new HashSet<int>();
			var anchorAssemblies = new List<TS.Assembly>();

			PlugInLogger.LogDebug("GetAnchorAssemblies - Collecting unique anchor assemblies.");

			foreach (var anchor in anchors)
			{
				var assembly = anchor.GetAssembly();
				if (assemblyIDs.Add(assembly.Identifier.ID))
				{
					anchorAssemblies.Add(assembly);
				}
			}

			PlugInLogger.LogDebug($"GetAnchorAssemblies - Retrieved {anchorAssemblies.Count} unique assemblies.");
			return anchorAssemblies;
		}

		private void CheckAndAddConnectedObject<T>(TS.Part part, AnchorGrid boltGrid)
			where T : IIdeaObjectConnectable
		{
			string partGUID = part.Identifier.GUID.ToString();
			PlugInLogger.LogDebug($"CheckAndAddConnectedObject - Checking part: {partGUID}");

			IIdeaObject ideaObject = CheckMaybe<T>(partGUID);
			if (ideaObject != null)
			{
				IIdeaObjectConnectable mainObject = GetMaybe<T>(partGUID);
				if (mainObject != null)
				{
					(boltGrid.ConnectedParts as List<IIdeaObjectConnectable>).Add(mainObject);
					PlugInLogger.LogDebug($"CheckAndAddConnectedObject - Added connected object: {partGUID}");
				}
			}
			else
			{
				PlugInLogger.LogDebug($"CheckAndAddConnectedObject - No connected object found for: {partGUID}");
			}
		}

		private double ExtractNumber(string input)
		{
			var match = Regex.Match(input, @"\d+(\.\d+)?");

			if (match.Success)
			{
				double value = double.Parse(match.Value, CultureInfo.InvariantCulture);
				PlugInLogger.LogDebug($"ExtractNumber - Extracted number: {value} from input: {input}");
				return value;
			}

			PlugInLogger.LogDebug($"ExtractNumber - No valid number found in input: {input}");
			return 0.0;
		}

		private IIdeaBoltAssembly GetAssembly(TS.BoltGroup boltGroup, TS.Part anchor, TS.Part nut, TS.Part washer)
		{
			bool isImperialUnitPresented = false;
			TSV.GetAdvancedOption("XS_IMPERIAL", ref isImperialUnitPresented);

			var stringPropTable = new Hashtable();
			boltGroup.GetStringReportProperties(new ArrayList
				{
					TeklaPropertiesKeys.BoltGradeKey,
					TeklaPropertiesKeys.BoltAssemblyNameKey
				}, ref stringPropTable);

			string boltAssemblyName = (string)stringPropTable[TeklaPropertiesKeys.BoltAssemblyNameKey];
			string boltGrade = (string)stringPropTable[TeklaPropertiesKeys.BoltGradeKey] ?? boltAssemblyName;

			var doublePropTable = new Hashtable();
			ArrayList dNames = new ArrayList
			{
				TeklaPropertiesKeys.BoltDiameterKey,
				TeklaPropertiesKeys.NutInnerDiameterKey,
				TeklaPropertiesKeys.HeadDiameterKey,
				TeklaPropertiesKeys.ThicknessKey,
				TeklaPropertiesKeys.NutThicknessKey,
				TeklaPropertiesKeys.NutOuterDiameterKey,
				TeklaPropertiesKeys.WasherThickness1Key,
				TeklaPropertiesKeys.WasherThickness2Key
			};

			boltGroup.GetDoubleReportProperties(dNames, ref doublePropTable);

			double rodsize = 0.0;
			anchor.GetUserProperty("RodSize", ref rodsize);
			PlugInLogger.LogDebug($"GetAssembly - found RodSize from GetUserProperty {rodsize}");
			rodsize = rodsize.MilimetersToMeters();
			if (rodsize <= 0.0)
			{
				rodsize = ExtractNumber(anchor.Profile.ProfileString).MilimetersToMeters();
				if (rodsize <= 0.0)
				{
					rodsize = ((double)doublePropTable[TeklaPropertiesKeys.NutInnerDiameterKey]).MilimetersToMeters();
					PlugInLogger.LogDebug($"GetAssembly - found R RodSize from {TeklaPropertiesKeys.NutInnerDiameterKey} {rodsize}");
					if (rodsize <= 0.0)
					{
						rodsize = ((double)doublePropTable[TeklaPropertiesKeys.BoltDiameterKey]).MilimetersToMeters();
						PlugInLogger.LogDebug($"GetAssembly - found RodSize from {TeklaPropertiesKeys.BoltDiameterKey} {rodsize}");
					}
				}
			}
			var headDiameter = ((double)doublePropTable[TeklaPropertiesKeys.HeadDiameterKey]).MilimetersToMeters();
			if (headDiameter <= 0.0)
			{
				headDiameter = rodsize * 1.7;
			}
			var headHeight = ((double)doublePropTable[TeklaPropertiesKeys.ThicknessKey]).MilimetersToMeters();
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
				nutThickness = ((double)doublePropTable[TeklaPropertiesKeys.NutThicknessKey]).MilimetersToMeters();
				if (nutThickness <= 0.0)
				{
					nutThickness = 0.6 * rodsize;
				}
			}

			var boreHole = ((double)doublePropTable[TeklaPropertiesKeys.BoltDiameterKey]).MilimetersToMeters();
			if (boreHole <= 0 || rodsize <= boreHole)
			{
				boreHole = rodsize + 0.001;
			}
			var diagonalHeadDiameter = ((double)doublePropTable[TeklaPropertiesKeys.NutOuterDiameterKey]).MilimetersToMeters();
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
				washerAtHead = ((double)doublePropTable[TeklaPropertiesKeys.WasherThickness1Key]) > 0;
				washerAtNut = ((double)doublePropTable[TeklaPropertiesKeys.WasherThickness2Key]) > 0;
				if (washerThickness <= 0.0)
				{
					washerThickness = ((double)doublePropTable[TeklaPropertiesKeys.WasherThickness1Key]).MilimetersToMeters();
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
				Name = $"{(isImperialUnitPresented ? rodsize.MetersToInchesFormated() : doublePropTable[TeklaPropertiesKeys.BoltDiameterKey])} {boltAssemblyName}",
				WasherAtHead = washerAtHead,
				WasherAtNut = washerAtNut,
				WasherThickness = washerThickness,

			};
		}
	}
}