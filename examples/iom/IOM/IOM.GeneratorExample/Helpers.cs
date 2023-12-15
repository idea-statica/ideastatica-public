using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Geometry3D;
using IdeaRS.OpenModel.Material;
using IdeaRS.OpenModel.Model;
using IdeaRS.OpenModel.Result;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace IOM.GeneratorExample
{
	/// <summary>
	/// This class contains helper methods for creating objects.
	/// </summary>
	public static class Helpers
	{
		/// <summary>
		/// Create cross section
		/// </summary>
		/// <param name="id">Css id</param>
		/// <param name="name">Name of css</param>
		/// <param name="material">Material of css</param>
		/// <returns>CSS with appropriate id, name and material</returns>
		public static CrossSectionParameter CreateCSSParameter(int id, string name, MatSteel material)
		{
			CrossSectionParameter css = new CrossSectionParameter();

			css.Id = id;
			css.Name = name;
			css.CrossSectionRotation = 0;
			css.CrossSectionType = CrossSectionType.RolledI;

			css.Parameters.Add(new ParameterString() { Name = "UniqueName", Value = name });
			css.Material = new ReferenceElement(material);

			return css;
		}

		/// <summary>
		/// Create member
		/// </summary>
		/// <param name="model">Idea open model</param>
		/// <param name="id">Member id</param>
		/// <param name="type">Member type</param>
		/// <param name="css">Cross section</param>
		/// <param name="startNode">Start node</param>
		/// <param name="endNode">End node</param>
		/// <returns>Connected member</returns>
		public static ConnectedMember CreateMember(OpenModel model, int id, Member1DType type, CrossSection css, string startNode, string endNode)
		{
			// create line segments
			LineSegment3D segment = CreateLineSegment3D(model, startNode, endNode);

			// create polylines
			PolyLine3D polyline = new PolyLine3D();
			polyline.Id = model.GetMaxId(polyline) + 1;
			polyline.Segments.Add(new ReferenceElement(segment));

			// add polylines and segments to the model
			model.AddObject(polyline);
			model.AddObject(segment);

			// create 1D elements
			Element1D element = CreateElement1D(model, segment);
			model.AddObject(element);

			// create 1D members
			Member1D member = CreateMember1D(model, id, type, element, css);
			model.Member1D.Add(member);

			// create and return connected member
			ConnectedMember connectedMember = new ConnectedMember();

			connectedMember.Id = id;
			connectedMember.MemberId = new ReferenceElement(member);

			return connectedMember;
		}

		/// <summary>
		/// Create member
		/// </summary>
		/// <param name="model">Idea open model</param>
		/// <param name="id">Member id</param>
		/// <param name="type">Member type</param>
		/// <param name="css">Cross section</param>
		/// <param name="startNode">Start node</param>
		/// <param name="middleNode">Middle node</param>
		/// <param name="endNode">End node</param>
		/// <returns>Connected member</returns>
		public static ConnectedMember CreateMember(OpenModel model, int id, Member1DType type, CrossSection css, string startNode, string middleNode, string endNode)
		{
			// column members have different coordination system in our example
			bool transformCoordSystem = type == Member1DType.Column ? true : false;

			// create line segments
			LineSegment3D segment1 = CreateLineSegment3D(model, startNode, middleNode, transformCoordSystem);
			model.AddObject(segment1);

			LineSegment3D segment2 = CreateLineSegment3D(model, middleNode, endNode, transformCoordSystem);
			model.AddObject(segment2);

			// create polylines
			PolyLine3D polyline = new PolyLine3D();
			polyline.Id = model.GetMaxId(polyline) + 1;
			polyline.Segments.Add(new ReferenceElement(segment1));
			polyline.Segments.Add(new ReferenceElement(segment2));
			model.AddObject(polyline);

			// create 1D elements
			Element1D element1 = CreateElement1D(model, segment1);
			model.AddObject(element1);

			Element1D element2 = CreateElement1D(model, segment2);
			model.AddObject(element2);

			// create 1D members
			Member1D member = CreateMember1D(model, id, type, element1, element2, css);
			model.Member1D.Add(member);

			// create and return connected member
			ConnectedMember connectedMember = new ConnectedMember();

			connectedMember.Id = id;
			connectedMember.MemberId = new ReferenceElement(member);

			return connectedMember;
		}

		/// <summary>
		/// Create line segment
		/// </summary>
		/// <param name="model">Idea open model</param>
		/// <param name="startNode">Start node</param>
		/// <param name="endNode">End node</param>
		/// <param name="transformCoordSystem">Tranform coordinate system</param>
		/// <returns>Line segment 3D</returns>
		private static LineSegment3D CreateLineSegment3D(OpenModel model, string startNode, string endNode, bool transformCoordSystem = false)
		{
			LineSegment3D segment3D = new LineSegment3D();

			segment3D.Id = model.GetMaxId(segment3D) + 1;
			segment3D.StartPoint = new ReferenceElement(model.Point3D.FirstOrDefault(item => item.Name == startNode));
			segment3D.EndPoint = new ReferenceElement(model.Point3D.FirstOrDefault(item => item.Name == endNode));

			if (transformCoordSystem)
			{
				CoordSystemByPoint system = new CoordSystemByPoint();
				system.Point = new Point3D() { X = 100000, Y = 0, Z = 0 };
				system.InPlane = Plane.ZX;
				segment3D.LocalCoordinateSystem = system;
			}

			return segment3D;
		}

		/// <summary>
		/// Create element 1D
		/// </summary>
		/// <param name="model">Idea open model</param>
		/// <param name="css">Cross section</param>
		/// <param name="segment">Line segment</param>
		/// <returns>Element 1D</returns>
		private static Element1D CreateElement1D(OpenModel model, LineSegment3D segment)
		{
			Element1D element1D = new Element1D();

			element1D.Id = model.GetMaxId(element1D) + 1;
			element1D.Name = "E" + element1D.Id.ToString();
			element1D.Segment = new ReferenceElement(segment);

			return element1D;
		}

		/// <summary>
		/// Create member 1D
		/// </summary>
		/// <param name="model">Idea open model</param>
		/// <param name="id">Member id</param>
		/// <param name="type">Member type</param>
		/// <param name="element">Element</param>
		/// <returns>Member 1D</returns>
		private static Member1D CreateMember1D(OpenModel model, int id, Member1DType type, Element1D element, CrossSection css)
		{
			Member1D member1D = new Member1D();

			member1D.Id = id;
			member1D.Name = "M" + member1D.Id.ToString();
			member1D.Member1DType = type;
			member1D.Elements1D.Add(new ReferenceElement(element));
			member1D.CrossSection = new ReferenceElement(css);

			return member1D;
		}

		/// <summary>
		/// Create compound member 1D
		/// </summary>
		/// <param name="model">Idea open model</param>
		/// <param name="id">Member id</param>
		/// <param name="type">Member type</param>
		/// <param name="element1">First element</param>
		/// <param name="element2">Second element</param>
		/// <returns>Member 1D</returns>
		private static Member1D CreateMember1D(OpenModel model, int id, Member1DType type, Element1D element1, Element1D element2, CrossSection css)
		{
			Member1D member1D = new Member1D();

			member1D.Id = id;
			member1D.Name = "M" + member1D.Id.ToString();
			member1D.Member1DType = type;
			member1D.Elements1D.Add(new ReferenceElement(element1));
			member1D.Elements1D.Add(new ReferenceElement(element2));
			member1D.CrossSection = new ReferenceElement(css);

			return member1D;
		}

		/// <summary>
		/// Load file with the results
		/// </summary>
		/// <returns>Instance of open model result</returns>
		public static OpenModelResult GetResults()
		{
			string rootDir = AppDomain.CurrentDomain.BaseDirectory;
			FileStream resultFile = new FileStream(rootDir + "\\SampleFiles\\IOM-SteelFrame.xmlR", FileMode.Open);

			XmlSerializer serializer = new XmlSerializer(typeof(OpenModelResult));
			OpenModelResult result = serializer.Deserialize(resultFile) as OpenModelResult;

			resultFile.Close();
			return result;
		}

		/// <summary>
		/// Send Post HTTP Request
		/// </summary>
		/// <returns>Instance of open model result</returns>
		public static string PostXMLData(string destinationUrl, string requestXml)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
			byte[] bytes = System.Text.Encoding.ASCII.GetBytes(requestXml);
			request.ContentType = "text/xml; encoding='utf-8'";
			request.ContentLength = bytes.Length;
			request.Method = "POST";
			Stream requestStream = request.GetRequestStream();
			requestStream.Write(bytes, 0, bytes.Length);
			requestStream.Close();
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode == HttpStatusCode.OK)
			{
				Stream responseStream = response.GetResponseStream();
				return new StreamReader(responseStream).ReadToEnd();
			}
			return null;
		}
	}
}