using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace IdeaRS.OpenModel.Result
{
	/// <summary>
	/// Type of results
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public enum ResultType
	{
		/// <summary>
		/// Internal forces
		/// </summary>
		InternalForces,

		/// <summary>
		/// Deformation
		/// </summary>
		Deformation,

		/// <summary>
		/// NLA on cross-section
		/// </summary>
		CrossSectionNLA,

		/// <summary>
		/// TA on cross-section
		/// </summary>
		CrossSectionTA,

		///// <summary>
		///// Load displacemet chart
		///// </summary>
		//LoadDisplacementChart

		/// <summary>
		/// Interaction digaram on section
		/// </summary>
		InteractionDiagram,

		/// <summary>
		/// Mesh on cross-section
		/// </summary>
		CrossSectionMesh,
	}

	/// <summary>
	/// Type of local system of result
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public enum ResultLocalSystemType
	{
		/// <summary>
		/// Local system x,y,z
		/// </summary>
		Local,

		/// <summary>
		/// Global system X, Y, Z
		/// </summary>
		Global,

		/// <summary>
		/// Principal system x, u, v
		/// </summary>
		Principle
	}

	/// <summary>
	/// Result of the member
	/// </summary>
	[Obfuscation(Feature = "renaming")]
	public class ResultOnMember
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ResultOnMember()
		{
			Results = new List<ResultBase>();
			LocalSystemType = ResultLocalSystemType.Principle;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="member">Member</param>
		/// <param name="resultType">Type of result</param>
		public ResultOnMember(IdeaRS.OpenModel.Result.Member member, ResultType resultType)
		{
			Member = member;
			ResultType = resultType;
			Results = new List<ResultBase>();
			LocalSystemType = ResultLocalSystemType.Principle;
		}

		/// <summary>
		/// Member
		/// </summary>
		public IdeaRS.OpenModel.Result.Member Member { get; set; }

		///// <summary>
		///// Loading
		///// </summary>
		//public IdeaRS.OpenModel.Result.Loading Loading { get; set; }

		/// <summary>
		/// Type of  result
		/// </summary>
		public ResultType ResultType { get; set; }

		/// <summary>
		/// Type of local system of result
		/// </summary>
		public ResultLocalSystemType LocalSystemType { get; set; }

		/// <summary>
		/// List of result
		/// </summary>
		public List<ResultBase> Results { get; set; }
	}

	/// <summary>
	/// List of result of member
	/// </summary>
	[Obsolete]
	public class ListOfResultOfMember : List<ResultOnMember>
	{
		/// <summary>
		/// Gets schema
		/// </summary>
		/// <returns></returns>
		public XmlSchema GetSchema() { return null; }

		/// <summary>
		/// Reads XML
		/// </summary>
		/// <param name="reader">XML reader</param>
		public void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement(typeof(ListOfResultOfMember).Name);
			while (reader.IsStartElement(typeof(ResultOnMember).Name))
			{
				Type type = Type.GetType(reader.GetAttribute("AssemblyQualifiedName"));
				XmlSerializer serial = new XmlSerializer(type);

				reader.ReadStartElement(typeof(ResultOnMember).Name);
				this.Add((ResultOnMember)serial.Deserialize(reader));
				reader.ReadEndElement();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Writes XML
		/// </summary>
		/// <param name="writer">XML writer</param>
		public void WriteXml(XmlWriter writer)
		{
			foreach (ResultOnMember test in this)
			{
				writer.WriteStartElement(typeof(ResultOnMember).Name);
				writer.WriteAttributeString("AssemblyQualifiedName", test.GetType().AssemblyQualifiedName);
				XmlSerializer xmlSerializer = new XmlSerializer(test.GetType());
				xmlSerializer.Serialize(writer, test);
				writer.WriteEndElement();
			}
		}
	}
}