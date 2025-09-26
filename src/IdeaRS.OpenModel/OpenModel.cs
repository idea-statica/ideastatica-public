using IdeaRS.OpenModel.Concrete;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.Detail;
using IdeaRS.OpenModel.Detail.Loading;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Loading;
using IdeaRS.OpenModel.Material;
using IdeaRS.OpenModel.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel
{
	///Version history
	/// V3.0 24.9.2025 Replaced Eccentricity properties by a single vector. Added CardinalPoint and EccentricityReference to Element1D
	/// V2.2 19.6.2025 Added new property 'ReferencedGeometry' to 'LoadOnSurface' object
	/// V2.1 27.08 2024 Added Pins
	/// V2.0.6 27.08 2024 Grids connectedParts by reference => delete connectedParttIds
	/// V2.0.5 27.08 2024 Added separated Bold, Anchor, 
	/// V2.0.1 27.08 2024 Version switch type from int to string
	/// V2 15. 12. 2021 cad plugins have member id conversion table => member id must be lower than 32 768

	/// <summary>
	/// Open model
	/// </summary>
	public class OpenModel
	{
		private Dictionary<string, IList> data;

		/// <summary>
		/// Constructor
		/// </summary>
		public OpenModel()
		{
			Version = "3.0.0";
			OriginSettings = null;
			Point3D = new List<Point3D>();
			LineSegment3D = new List<LineSegment3D>();
			ArcSegment3D = new List<ArcSegment3D>();
			PolyLine3D = new List<PolyLine3D>();
			Region3D = new List<Region3D>();
			MatConcrete = new List<MatConcrete>();
			MatReinforcement = new List<MatReinforcement>();
			MatSteel = new List<MatSteel>();
			MatPrestressSteel = new List<MatPrestressSteel>();
			MatWelding = new List<MatWelding>();
			MatBoltGrade = new List<MaterialBoltGrade>();
			MatHeadedStudGrade = new List<MaterialHeadedStudGrade>();
			CrossSection = new List<CrossSection.CrossSection>();
			BoltAssembly = new List<BoltAssembly>();
			Pin = new List<Pin>();
			ReinforcedCrossSection = new List<CrossSection.ReinforcedCrossSection>();
			HingeElement1D = new List<HingeElement1D>();
			Opening = new List<Opening>();
			DappedEnd = new List<Detail.DappedEnd>();
			PatchDevice = new List<PatchDevice>();
			Element1D = new List<Element1D>();
			Beam = new List<Detail.Beam>();
			Member1D = new List<Member1D>();
			Element2D = new List<Element2D>();
			Wall = new List<Wall>();
			Member2D = new List<Member2D>();
			RigidLink = new List<RigidLink>();
			PointOnLine3D = new List<PointOnLine3D>();
			PointSupportNode = new List<PointSupportNode>();
			LineSupportSegment = new List<LineSupportSegment>();
			LoadsInPoint = new List<LoadInPoint>();
			LoadsOnLine = new List<LoadOnLine>();
			StrainLoadsOnLine = new List<StrainLoadOnLine>();
			PointLoadsOnLine = new List<PointLoadOnLine>();
			LoadsOnSurface = new List<LoadOnSurface>();
			Settlements = new List<Settlement>();
			TemperatureLoadsOnLine = new List<TemperatureLoadOnLine>();
			LoadGroup = new List<LoadGroup>();
			LoadCase = new List<LoadCase>();
			CombiInput = new List<CombiInput>();
			Attribute = new List<OpenAttribute>();
			ConnectionPoint = new List<ConnectionPoint>();
			Connections = new List<ConnectionData>();
			InitialImperfectionOfPoint = new List<InitialImperfectionOfPoint>();
			Tendon = new List<Tendon>();
			Reinforcement = new List<Reinforcement>();
			ISDModel = new List<ISDModel>();
			ResultClass = new List<ResultClass>();

			CheckMember = new List<CheckMember>();
			ConcreteCheckSection = new List<CheckSection>();

			DesignMember = new List<DesignMember>();
			SubStructure = new List<SubStructure>();

			RebarShape = new List<RebarShape>();
			RebarGeneral = new List<RebarGeneral>();
			RebarSingle = new List<RebarSingle>();
			RebarStirrups = new List<RebarStirrups>();

			Taper = new List<Taper>();
			Span = new List<Span>();

			SolidBlocks3D = new List<SolidBlock3D>();
			SurfaceSupports3D = new List<SurfaceSupport3D>();
			BasePlates3D = new List<BasePlate3D>();
			Anchors3D = new List<Anchor3D>();
			DetailLoadCase = new List<DetailLoadCase>();
			DetailCombination = new List<DetailCombination>();



		}

		/// <summary>
		/// Data format version
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Information about origin project
		/// </summary>
		public OriginSettings OriginSettings { get; set; }

		/// <summary>
		/// List of Point3D
		/// </summary>
		public List<Point3D> Point3D { get; set; }

		/// <summary>
		/// List of LineSegment3D
		/// </summary>
		public List<LineSegment3D> LineSegment3D { get; set; }

		/// <summary>
		/// List of ArcSegment3D
		/// </summary>
		public List<ArcSegment3D> ArcSegment3D { get; set; }

		/// <summary>
		/// List of PolyLine3D
		/// </summary>
		public List<PolyLine3D> PolyLine3D { get; set; }

		/// <summary>
		/// List of Region3D
		/// </summary>
		public List<Region3D> Region3D { get; set; }

		/// <summary>
		/// List of MatConcrete
		/// </summary>
		public List<MatConcrete> MatConcrete { get; set; }

		/// <summary>
		/// List of MatReinforcement
		/// </summary>
		public List<MatReinforcement> MatReinforcement { get; set; }

		/// <summary>
		/// List of MatSteel
		/// </summary>
		public List<MatSteel> MatSteel { get; set; }

		/// <summary>
		/// List of MatPrestressSteel
		/// </summary>
		public List<MatPrestressSteel> MatPrestressSteel { get; set; }

		/// <summary>
		/// List of MatWelding
		/// </summary>
		public List<MatWelding> MatWelding { get; set; }

		/// <summary>
		/// List of BoltGrades
		/// </summary>
		public List<MaterialBoltGrade> MatBoltGrade { get; set; }

		/// <summary>
		/// List of HeadedStudGrades
		/// </summary>
		public List<MaterialHeadedStudGrade> MatHeadedStudGrade { get; set; }

		/// <summary>
		/// List of CrossSection
		/// </summary>
		public List<CrossSection.CrossSection> CrossSection { get; set; }

		/// <summary>
		/// List of BoltAssemblys
		/// </summary>
		public List<BoltAssembly> BoltAssembly { get; set; }

		/// <summary>
		/// List of Pins
		/// </summary>
		public List<Pin> Pin { get; set; }

		/// <summary>
		/// List of Reinforced CrossSection
		/// </summary>
		public List<CrossSection.ReinforcedCrossSection> ReinforcedCrossSection { get; set; }

		/// <summary>
		/// List of hinge elements 1D
		/// </summary>
		public List<HingeElement1D> HingeElement1D { get; set; }

		/// <summary>
		/// List of openings for Detail
		/// </summary>
		public List<Opening> Opening { get; set; }

		/// <summary>
		/// List of dapped ends in Detail
		/// </summary>
		public List<DappedEnd> DappedEnd { get; set; }

		/// <summary>
		/// List of dapped ends in Detail
		/// </summary>
		public List<PatchDevice> PatchDevice { get; set; }

		/// <summary>
		/// List of Elements 1D
		/// </summary>
		public List<Element1D> Element1D { get; set; }

		/// <summary>
		/// List of Elements 1D
		/// </summary>
		public List<Detail.Beam> Beam { get; set; }

		/// <summary>
		/// List of Member 1D
		/// </summary>
		public List<Member1D> Member1D { get; set; }

		/// <summary>
		/// List of Elements 2D
		/// </summary>
		public List<Element2D> Element2D { get; set; }

		/// <summary>
		/// List of Elements 2D
		/// </summary>
		public List<Wall> Wall { get; set; }

		/// <summary>
		/// List of Member 2D
		/// </summary>
		public List<Member2D> Member2D { get; set; }

		/// <summary>
		/// List of Rigid link
		/// </summary>
		public List<RigidLink> RigidLink { get; set; }

		/// <summary>
		/// List of Point on line 3D
		/// </summary>
		public List<PointOnLine3D> PointOnLine3D { get; set; }

		/// <summary>
		/// List of Point support in node
		/// </summary>
		public List<PointSupportNode> PointSupportNode { get; set; }

		/// <summary>
		/// List of Line support on segment
		/// </summary>
		public List<LineSupportSegment> LineSupportSegment { get; set; }

		/// <summary>
		/// List of point load impulses in this load case
		/// </summary>
		public List<LoadInPoint> LoadsInPoint { get; set; }

		/// <summary>
		/// List of line load impulses in this load case
		/// </summary>
		public List<LoadOnLine> LoadsOnLine { get; set; }

		/// <summary>
		/// List of generalized strain load impulses along the line in this load case.
		/// </summary>
		public List<StrainLoadOnLine> StrainLoadsOnLine { get; set; }

		/// <summary>
		/// List of point load impulses in this load case
		/// </summary>
		public List<PointLoadOnLine> PointLoadsOnLine { get; set; }

		/// <summary>
		/// List surafce load in this load case
		/// </summary>
		public List<LoadOnSurface> LoadsOnSurface { get; set; }

		/// <summary>
		/// Settlements in this load case
		/// </summary>
		public List<Settlement> Settlements { get; set; }

		/// <summary>
		/// List of temperature load in this load case
		/// </summary>
		public List<TemperatureLoadOnLine> TemperatureLoadsOnLine { get; set; }

		/// <summary>
		/// List of Load groups
		/// </summary>
		public List<LoadGroup> LoadGroup { get; set; }

		/// <summary>
		/// List of Load cases
		/// </summary>
		public List<LoadCase> LoadCase { get; set; }

		/// <summary>
		/// List of Combinations
		/// </summary>
		public List<CombiInput> CombiInput { get; set; }

		/// <summary>
		/// List of attributes
		/// </summary>
		public List<OpenAttribute> Attribute { get; set; }

		/// <summary>
		/// List of Connection Points
		/// </summary>
		public List<ConnectionPoint> ConnectionPoint { get; set; }

		/// <summary>
		/// List of Connection data
		/// </summary>
		public List<ConnectionData> Connections { get; set; }

		/// <summary>
		/// List of reinforcement in IDEA StatiCa Detail
		/// </summary>
		public List<Reinforcement> Reinforcement { get; set; }

		/// <summary>
		/// List of Details
		/// </summary>
		public List<ISDModel> ISDModel { get; set; }

		/// <summary>
		/// List of InitialmperfectionOfPoint
		/// </summary>
		public List<InitialImperfectionOfPoint> InitialImperfectionOfPoint { get; set; }

		/// <summary>
		/// Tendon
		/// </summary>
		public List<Tendon> Tendon { get; set; }

		/// <summary>
		/// Result Class
		/// </summary>
		public List<ResultClass> ResultClass { get; set; }

		/// <summary>
		/// Design Member
		/// </summary>
		public List<DesignMember> DesignMember { get; set; }

		/// <summary>
		/// Design Member
		/// </summary>
		public List<SubStructure> SubStructure { get; set; }

		/// <summary>
		/// Information about Connection Setup
		/// </summary>
		public ConnectionSetup ConnectionSetup { get; set; }

		/// <summary>
		/// Gets or sets the projet data
		/// </summary>
		public ProjectData ProjectData { get; set; }

		/// <summary>
		/// List of the Check members
		/// </summary>
		public List<CheckMember> CheckMember { get; set; }

		/// <summary>
		/// List of the concrete check section
		/// </summary>
		public List<CheckSection> ConcreteCheckSection { get; set; }

		/// <summary>
		/// Gets or sets the concrete member data
		/// </summary>
		public ConcreteSetup ConcreteSetup { get; set; }

		/// <summary>
		/// Gets or sets the projet data concrete
		/// </summary>
		public ProjectDataConcrete ProjectDataConcrete { get; set; }

		/// <summary>
		/// Gets or sets the rebars shapes
		/// </summary>
		public List<RebarShape> RebarShape { get; set; }

		/// <summary>
		/// Gets or sets the rebar General collection
		/// </summary>
		public List<RebarGeneral> RebarGeneral { get; set; }

		/// <summary>
		/// Gets or sets the rebar single collection
		/// </summary>
		public List<RebarSingle> RebarSingle { get; set; }

		/// <summary>
		/// Gets or sets the rebar group (stirrups) collection
		/// </summary>
		public List<RebarStirrups> RebarStirrups { get; set; }

		public List<Taper> Taper { get; set; }

		public List<Span> Span { get; set; }

		/// <summary>
		/// List of Solid Blocks 3D
		/// </summary>
		public List<SolidBlock3D> SolidBlocks3D { get; set; }

		/// <summary>
		/// List of Surface Supports 3D
		/// </summary>
		public List<SurfaceSupport3D> SurfaceSupports3D { get; set; }

		/// <summary>
		/// List of Base Plates 3D
		/// </summary>
		public List<BasePlate3D> BasePlates3D { get; set; }

		/// <summary>
		/// List of Anchors 3D
		/// </summary>
		public List<Anchor3D> Anchors3D { get; set; }

		/// <summary>
		/// List of Load cases
		/// </summary>
		public List<DetailLoadCase> DetailLoadCase { get; set; }

		/// <summary>
		/// List of Combinations
		/// </summary>
		public List<DetailCombination> DetailCombination { get; set; }

		/// <summary>
		/// Get max Id value for specified type
		/// </summary>
		/// <param name="typeName">Name of type</param>
		/// <returns>Max Id value or zero if don't any exists</returns>
		public int GetMaxId(string typeName)
		{
			IList obj;
			if (!GetData().TryGetValue(typeName, out obj))
			{
				return -10;
			}

			IEnumerable<OpenElementId> lst = obj.Cast<OpenElementId>();
			if (lst.Any())
			{
				return lst.Max(o => o.Id);
			}

			return 0;
		}

		/// <summary>
		/// Get max Id value for specified type
		/// </summary>
		/// <param name="obj">Object</param>
		/// <returns>Max Id value or zero if don't any exists</returns>
		public int GetMaxId(OpenElementId obj)
		{
			string typeName = GetObjectListName(obj);
			return GetMaxId(typeName);
		}

		/// <summary>
		/// Get max Id value for specified type
		/// </summary>
		/// <param name="t">Type</param>
		/// <returns>Max Id value or zero if don't any exists</returns>
		public int GetMaxId(Type t)
		{
			string typeName = GetObjectListName(t);
			return GetMaxId(typeName);
		}

		/// <summary>
		/// Add new atribute object into collections
		/// </summary>
		/// <param name="obj">Added object</param>
		/// <returns>Return value 0 - OK, -1 - false</returns>
		/// <remarks>
		///
		/// </remarks>
		public int AddObject(OpenAttribute obj)
		{
			string typeName = typeof(OpenAttribute).Name;
			IList lst1;
			if (!GetData().TryGetValue(typeName, out lst1))
			{
				return -10;
			}

			lst1.Add(obj);
			return 0;
		}

		/// <summary>
		/// Add new object into collections
		/// </summary>
		/// <param name="obj">Added object</param>
		/// <returns>Return value 0 - OK, -1 - duplicit Id, -10 - the type of object there is not in the data colections</returns>
		/// <remarks>
		/// If the obj.Id is set to zero or lesser, this method finds the first available free Id and sets it into the obj.Id.
		/// </remarks>
		public int AddObject(OpenElementId obj)
		{
			string typeName = GetObjectListName(obj);
			IList lst1;
			if (!GetData().TryGetValue(typeName, out lst1))
			{
				return -10;
			}

			IEnumerable<OpenElementId> lst = lst1.Cast<OpenElementId>();
			int id = obj.Id;
			if (id < 1)
			{
				id = obj.Id = GetMaxId(typeName) + 1;
			}

			if (lst.Any(o => o.Id == id))
			{
				System.Diagnostics.Debug.Assert(false, "Snažíš se přidat do IOM objekt se stejným ID");
				return -1;
			}

			lst1.Add(obj);
			return 0;
		}

		/// <summary>
		/// Add new object into collections
		/// </summary>
		/// <param name="obj">Added object</param>
		/// <returns>Return value 0 - OK, -1 - duplicit Id, -10 - the type of object there is not in the data colections</returns>
		/// <remarks>
		/// If the obj.Id is set to zero or lesser, this method finds the first available free Id and sets it into the obj.Id.
		/// </remarks>
		public OpenElementId GetExistingObject(OpenElementId obj)
		{
			string typeName = GetObjectListName(obj);
			IList lst1;
			if (!GetData().TryGetValue(typeName, out lst1))
			{
				return null;
			}

			IEnumerable<OpenElementId> lst = lst1.Cast<OpenElementId>();
			int id = obj.Id;
			if (id < 1)
			{
				id = obj.Id = GetMaxId(typeName) + 1;
			}

			OpenElementId ret = lst.FirstOrDefault(o => o.Id == id);
			return ret;
		}

		/// <summary>
		/// Saves content into file as a XML format
		/// </summary>
		/// <param name="xmlFileName">XML file name</param>
		/// <returns>True if succeeded</returns>
		public bool SaveToXmlFile(string xmlFileName)
		{
			// Storing to standard xml file
			XmlSerializer xs = new XmlSerializer(typeof(OpenModel));

			Stream fs = new FileStream(xmlFileName, FileMode.Create);
			XmlTextWriter writer = new XmlTextWriter(fs, Encoding.Unicode)
			{
				Formatting = Formatting.Indented
			};
			// Serialize using the XmlTextWriter.
			xs.Serialize(writer, this);
			writer.Close();
			fs.Close();

			return true;
		}

		/// <summary>
		/// Creates the instance of Open Model from the XML file
		/// </summary>
		/// <param name="xmlFileName">XML file name</param>
		/// <returns>Open Model or null</returns>
		public static OpenModel LoadFromXmlFile(string xmlFileName)
		{
			OpenModel openModel = null;
			using (FileStream fs = new FileStream(xmlFileName, FileMode.Open, FileAccess.Read))
			{
				openModel = LoadFromStream(fs);
			}

			return openModel;
		}

		/// <summary>
		/// Creates the instance of Open Model from the stream
		/// </summary>
		/// <param name="xmlFileStream">The input stream</param>
		/// <returns>The new instance of Open Model</returns>
		public static OpenModel LoadFromStream(Stream xmlFileStream)
		{
			XmlReaderSettings xmlSettings = new XmlReaderSettings
			{
				CloseInput = false
			};

			XmlReader reader = XmlReader.Create(xmlFileStream, xmlSettings);
			XmlSerializer xs = new XmlSerializer(typeof(OpenModel));
			OpenModel openModel = (xs.Deserialize(reader) as OpenModel);
			openModel.ReferenceElementsReconstruction();

			return openModel;
		}

		/// <summary>
		/// Creates the instance of Open Model from xml string
		/// </summary>
		/// <param name="xmlString">The input string</param>
		/// <returns>The new instance of Open Model</returns>
		public static OpenModel LoadFromString(string xmlString)
		{
			StringReader stringReader = new System.IO.StringReader(xmlString);
			XmlSerializer serializer = new XmlSerializer(typeof(OpenModel));
			OpenModel openModel = serializer.Deserialize(stringReader) as OpenModel;
			openModel.ReferenceElementsReconstruction();

			return openModel;
		}

		/// <summary>
		/// Gets the Open element object from the Reference element
		/// </summary>
		/// <param name="element">Reference element</param>
		/// <returns>The instance of  OpenElementId object</returns>
		public OpenElementId GetObject(ReferenceElement element)
		{
			if (element.Element != null)
			{
				return element.Element;
			}

			IList obj;
			if (!GetData().TryGetValue(element.TypeName, out obj))
			{
				return null;
			}

			IEnumerable<OpenElementId> lst = obj.Cast<OpenElementId>();
			return lst.OfType<OpenElementId>().FirstOrDefault(it => it.Id == element.Id);
		}

		private Dictionary<string, IList> GetData()
		{
			if (data == null)
			{
				data = new Dictionary<string, IList>
				{
					{ typeof(Point3D).Name, Point3D },
					{ typeof(LineSegment3D).Name, LineSegment3D },
					{ typeof(ArcSegment3D).Name, ArcSegment3D },
					{ typeof(PolyLine3D).Name, PolyLine3D },
					{ typeof(Region3D).Name, Region3D },
					{ typeof(MatConcrete).Name, MatConcrete },
					{ typeof(MatReinforcement).Name, MatReinforcement },
					{ typeof(MatSteel).Name, MatSteel },
					{ typeof(MatPrestressSteel).Name, MatPrestressSteel },
					{ typeof(MatWelding).Name, MatWelding },
					{ typeof(MaterialBoltGrade).Name, MatBoltGrade },
					{ typeof(MaterialHeadedStudGrade).Name, MatHeadedStudGrade },
					{ typeof(CrossSection.CrossSection).Name, CrossSection },
					{ typeof(CrossSection.ReinforcedCrossSection).Name, ReinforcedCrossSection },
					{ typeof(BoltAssembly).Name, BoltAssembly },
					{ typeof(Pin).Name, Pin },
					{ typeof(HingeElement1D).Name, HingeElement1D },
					{ typeof(Opening).Name, Opening },
					{ typeof(DappedEnd).Name, DappedEnd },
					{ typeof(PatchDevice).Name, PatchDevice },
					{ typeof(Element1D).Name, Element1D },
					{ typeof(Detail.Beam).Name, Beam },
					{ typeof(Member1D).Name, Member1D },
					{ typeof(Element2D).Name, Element2D },
					{ typeof(Wall).Name, Wall },
					{ typeof(Member2D).Name, Member2D },
					{ typeof(RigidLink).Name, RigidLink },
					{ typeof(PointOnLine3D).Name, PointOnLine3D },
					{ typeof(PointSupportNode).Name, PointSupportNode },
					{ typeof(LineSupportSegment).Name, LineSupportSegment },
					{ typeof(LoadInPoint).Name, LoadsInPoint },
					{ typeof(LoadOnLine).Name, LoadsOnLine },
					{ typeof(StrainLoadOnLine).Name, StrainLoadsOnLine },
					{ typeof(PointLoadOnLine).Name, PointLoadsOnLine },
					{ typeof(LoadOnSurface).Name, LoadsOnSurface },
					{ typeof(Settlement).Name, Settlements },
					{ typeof(TemperatureLoadOnLine).Name, TemperatureLoadsOnLine },
					{ typeof(LoadGroup).Name, LoadGroup },
					{ typeof(LoadCase).Name, LoadCase },
					{ typeof(CombiInput).Name, CombiInput },
					{ typeof(OpenAttribute).Name, Attribute },
					{ typeof(ConnectionPoint).Name, ConnectionPoint },
					{ typeof(ConnectionData).Name, Connections },
					{ typeof(InitialImperfectionOfPoint).Name, InitialImperfectionOfPoint },
					{ typeof(Tendon).Name, Tendon },
					{ typeof(Reinforcement).Name, Reinforcement },
					{ typeof(ISDModel).Name, ISDModel },
					{ typeof(ResultClass).Name, ResultClass },
					{ typeof(CheckMember).Name, CheckMember },
					{ typeof(CheckSection).Name, ConcreteCheckSection },
					{ typeof(SubStructure).Name, SubStructure },
					{ typeof(DesignMember).Name, DesignMember },
					{ typeof(RebarShape).Name, RebarShape },
					{ typeof(RebarGeneral).Name, RebarGeneral },
					{ typeof(RebarSingle).Name, RebarSingle },
					{ typeof(RebarStirrups).Name, RebarStirrups },
					{ typeof(Taper).Name, Taper },
					{ typeof(Span).Name, Span },
					{ typeof(SolidBlock3D).Name, SolidBlocks3D },
					{ typeof(SurfaceSupport3D).Name, SurfaceSupports3D },
					{ typeof(BasePlate3D).Name, BasePlates3D },
					{ typeof(Anchor3D).Name, Anchors3D },
					{ typeof(DetailLoadCase).Name, DetailLoadCase },
					{ typeof(DetailCombination).Name, DetailCombination },

				};
			}

			data[typeof(ProjectData).Name] = new ProjectData[] { ProjectData };
			data[typeof(ProjectDataConcrete).Name] = new ProjectDataConcrete[] { ProjectDataConcrete };

			return data;
		}

		private string GetObjectListName(OpenElementId obj)
		{
			Type t = obj.GetType();
			return GetObjectListName(t);
		}

		private string GetObjectListName(Type t)
		{
			object atr = t.GetCustomAttributes(typeof(IdeaRS.OpenModel.OpenModelClassAttribute), true).FirstOrDefault();
			if (atr != null)
			{
				Type t1 = (atr as IdeaRS.OpenModel.OpenModelClassAttribute).OpenModelListType;
				if (t1 != null)
				{
					return t1.Name;
				}
			}

			return t.Name;
		}

		/// <summary>
		/// Should be called after deserialization of the open model from xml
		/// </summary>
		public void ReferenceElementsReconstruction(bool renewReferences = false)
		{
			foreach (IList listobj in GetData().Values.ToList())
			{
				foreach (object obj in listobj)
				{
					ReferenceElementReconstruction(obj, renewReferences);
				}
			}
		}

		private void ReferenceElementReconstruction(object obj, bool renewReferences = false)
		{
			if (obj is null)
			{
				return;
			}

			if (obj is ReferenceElement referenceElement)
			{
				if (renewReferences)
				{
					int id = referenceElement.Id;
					string typeName = referenceElement.TypeName;

					referenceElement.Element = null;

					referenceElement.Id = id;
					referenceElement.TypeName = typeName;
				}

				if (referenceElement.Element == null)
				{
					referenceElement.Element = GetObject(referenceElement);
				}
			}
			else if (obj is IEnumerable enumerable)
			{
				foreach (object elem in enumerable)
				{
					ReferenceElementReconstruction(elem, renewReferences);
				}
			}
			else
			{
				Type objType = obj.GetType();

				foreach (PropertyDescriptor p in TypeDescriptor.GetProperties(obj))
				{
					if (p.PropertyType == objType)
					{
						continue;
					}

					ReferenceElementReconstruction(p.GetValue(obj), renewReferences);
				}
			}
		}
	}
}