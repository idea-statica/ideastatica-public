using IdeaRS.OpenModel.Message;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace IdeaRS.OpenModel.Connection
{
	/// <summary>
	/// Check summary
	/// </summary>
	[XmlRootAttribute(ElementName = "CheckResSummary", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class CheckResSummary
	{
		/// <summary>
		/// Check value
		/// </summary>
		[DataMember]
		public double CheckValue { get; set; }

		/// <summary>
		/// Status of check
		/// </summary>
		[DataMember]
		public bool CheckStatus { get; set; }

		/// <summary>
		/// Id of Load Case
		/// </summary>
		[DataMember]
		public int LoadCaseId { get; set; }

		/// <summary>
		/// Name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Detail message about overall check
		/// </summary>
		[DataMember]
		public string UnityCheckMessage { get; set; }

		/// <summary>
		/// Whether the check was calculated or not.
		/// If <see langword="true"/>, the check was not calculated and <see cref="CheckValue"/> should be ignored, otherwise <see langword="false"/>.
		/// </summary>
		[DataMember]
		public bool Skipped { get; set; }
	}

	/// <summary>
	/// Results for connection in project
	/// </summary>
	[XmlRootAttribute(ElementName = "ConnectionCheckRes", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class ConnectionCheckRes
	{
		/// <summary>
		///
		/// </summary>
		public ConnectionCheckRes()
		{
			CheckResSummary = new List<CheckResSummary>();
			CheckResPlate = new List<CheckResPlate>();
			CheckResWeld = new List<CheckResWeld>();
			CheckResBolt = new List<CheckResBolt>();
			CheckResAnchor = new List<CheckResAnchor>();
			CheckResConcreteBlock = new List<CheckResConcreteBlock>();
			BucklingResults = new List<BucklingRes>();
		}

		/// <summary>
		/// List of CheckResSummary
		/// </summary>
		[DataMember]
		public List<CheckResSummary> CheckResSummary { get; set; }

		/// <summary>
		/// List of check results for plates
		/// </summary>
		[DataMember]
		public List<CheckResPlate> CheckResPlate { get; set; }

		/// <summary>
		/// List of check results for welds
		/// </summary>
		[DataMember]
		public List<CheckResWeld> CheckResWeld { get; set; }

		/// <summary>
		/// List of check results for bolts
		/// </summary>
		[DataMember]
		public List<CheckResBolt> CheckResBolt { get; set; }

		/// <summary>
		/// List of check results for anchors
		/// </summary>
		[DataMember]
		public List<CheckResAnchor> CheckResAnchor { get; set; }

		/// <summary>
		/// List of check results for concrete blocks
		/// </summary>
		[DataMember]
		public List<CheckResConcreteBlock> CheckResConcreteBlock { get; set; }

		/// <summary>
		/// List of results of buckling analysis
		/// </summary>
		[DataMember]
		public List<BucklingRes> BucklingResults { get; set; }

		/// <summary>
		/// Name of connection
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Guid of connection
		/// </summary>
		[DataMember]
		public Guid ConnectionID { get; set; }

		/// <summary>
		/// Integer Id of connection
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the list of errors, that apears during validation and/or calculation.
		/// </summary>
		[DataMember]
		public OpenMessages Messages { get; set; }
	}

	/// <summary>
	/// Results for all connections in project
	/// </summary>
	[XmlRootAttribute(ElementName = "ConnectionResultsData", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class ConnectionResultsData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ConnectionResultsData()
		{
			ConnectionCheckRes = new List<ConnectionCheckRes>();
		}

		/// <summary>
		/// List of ConnectionCheckRes
		/// </summary>
		[DataMember]
		public List<ConnectionCheckRes> ConnectionCheckRes { get; set; }
	}

	/// <summary>
	/// Check value for Concrete Block
	/// </summary>
	[XmlRootAttribute(ElementName = "CheckResConcreteBlock", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class CheckResConcreteBlock
	{
		/// <summary>
		/// Name of Concrete Block
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Unity Check
		/// </summary>
		[DataMember]
		public double UnityCheck { get; set; }

		/// <summary>
		/// Status of the Check
		/// </summary>
		[DataMember]
		public bool CheckStatus { get; set; }

		/// <summary>
		/// Id of Load Case
		/// </summary>
		[DataMember]
		public int LoadCaseId { get; set; }
	}

	/// <summary>
	/// Check value for Bolts
	/// </summary>
	[XmlRootAttribute(ElementName = "CheckResBolt", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class CheckResBolt
	{
		/// <summary>
		/// Name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Unity Check
		/// </summary>
		[DataMember]
		public double UnityCheck { get; set; }

		/// <summary>
		/// Status of the Check
		/// </summary>
		[DataMember]
		public bool CheckStatus { get; set; }

		/// <summary>
		/// Id of Load Case
		/// </summary>
		public int LoadCaseId { get; set; }
	}

	/// <summary>
	/// Check value for Anchor
	/// </summary>
	[XmlRootAttribute(ElementName = "CheckResAnchor", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class CheckResAnchor
	{
		/// <summary>
		/// Name
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Unity Check
		/// </summary>
		[DataMember]
		public double UnityCheck { get; set; }

		/// <summary>
		/// Status of the Check
		/// </summary>
		[DataMember]
		public bool CheckStatus { get; set; }

		/// <summary>
		/// Id of Load Case
		/// </summary>
		public int LoadCaseId { get; set; }
	}

	/// <summary>
	/// Check value for Plate
	/// </summary>
	[XmlRootAttribute(ElementName = "CheckResPlate", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class CheckResPlate
	{
		/// <summary>
		/// Name of Plate
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Status of the Check
		/// </summary>
		[DataMember]
		public bool CheckStatus { get; set; }

		/// <summary>
		/// Id of Load Case
		/// </summary>
		[DataMember]
		public int LoadCaseId { get; set; }

		/// <summary>
		/// Max Strain
		/// </summary>
		[DataMember]
		public double MaxStrain { get; set; }

		/// <summary>
		/// Max Stress
		/// </summary>
		[DataMember]
		public double MaxStress { get; set; }

		/// <summary>
		/// In case of presentation of groups plates (uncoiled beams)
		/// </summary>
		[DataMember]
		public List<int> Items { get; set; }
	}

	/// <summary>
	/// Check value for Weld
	/// </summary>
	[XmlRootAttribute(ElementName = "CheckResWeld", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class CheckResWeld
	{
		/// <summary>
		/// Name of Weld
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Unique id of weld
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Unity Check Stress
		/// </summary>
		[DataMember]
		public double UnityCheck { get; set; }

		/// <summary>
		/// Status of the Check
		/// </summary>
		[DataMember]
		public bool CheckStatus { get; set; }

		/// <summary>
		/// Id of Load Case
		/// </summary>
		[DataMember]
		public int LoadCaseId { get; set; }

		/// <summary>
		/// In case of presentation of groups plates (uncoiled beams)
		/// </summary>
		[DataMember]
		public List<int> Items { get; set; }
	}

	/// <summary>
	/// Results of the buckling analysis
	/// </summary>
	[XmlRootAttribute(ElementName = "BucklingRes", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class BucklingRes
	{
		// Id of the loadcase
		[DataMember]
		public int LoadCaseId { get; set; }

		/// <summary>
		/// Shape lc calculated by solver
		/// </summary>
		[DataMember]
		public int Shape { get; set; }

		/// <summary>
		/// Buckling factor
		/// </summary>
		[DataMember]
		public double Factor { get; set; }
	}

	/// <summary>
	/// Information about connection load update
	/// </summary>
	[XmlRootAttribute(ElementName = "ConnectionLoadInfo", IsNullable = false)]
	[Serializable]
	[DataContract]
	public class ConnectionLoadInfo
	{
		/// <summary>
		/// Name of Weld
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// BeamMapping
		/// </summary>
		public List<BeamLoadsImportMappingData> BeamMapping { get; set; }
	}

	/// <summary>
	/// BeamLoadsImportMappingData
	/// </summary>
	[XmlRootAttribute(ElementName = "BeamLoadsImportMappingData", IsNullable = false)]
	[Serializable]
	public class BeamLoadsImportMappingData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public BeamLoadsImportMappingData()
		{
			BeamID = 0;
			CADBeamID = 0;
			CoeffVM_X = 1.0;
			CoeffVM_Y = 1.0;
			CoeffVM_Z = 1.0;
		}

		/// <summary>
		/// BeamID
		/// </summary>
		[DataMember]
		public int BeamID { get; set; }

		/// <summary>
		/// CADBeamID
		/// </summary>
		[DataMember]
		public int CADBeamID { get; set; }

		/// <summary>
		/// CoeffVM_Y
		/// </summary>
		[DataMember]
		public double CoeffVM_Y { get; set; }

		/// <summary>
		/// CoeffVM_Z
		/// </summary>
		[DataMember]
		public double CoeffVM_Z { get; set; }
		
		/// <summary>
		/// CoeffVM_X
		/// </summary>
		[DataMember]
		public double CoeffVM_X { get; set; }
	}
}