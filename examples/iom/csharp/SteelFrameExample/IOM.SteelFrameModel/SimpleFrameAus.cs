using IdeaRS.OpenModel;
using IdeaRS.OpenModel.Connection;
using IdeaRS.OpenModel.CrossSection;
using IdeaRS.OpenModel.Material;
using IdeaRS.OpenModel.Model;
using System.Collections.Generic;

namespace IOM.SteelFrameModel
{
	/// <summary>
	/// A small frame designed to the Australian code AS: a column and a beam, with a connection where they meet at
	/// node N2. The material and the cross-sections come from the IDEA StatiCa library, found by their names. The
	/// model has no load cases and no internal forces.
	/// </summary>
	public static class SimpleFrameAus
	{
		/// <summary>
		/// Builds the model of the frame.
		/// </summary>
		public static OpenModel CreateIOM()
		{
			var model = new OpenModel();
			model.OriginSettings = new OriginSettings
			{
				CrossSectionConversionTable = CrossSectionConversionTable.SCIA,
				CountryCode = CountryCode.Australia,
				ProjectName = "Project",
				Author = "IDEA StatiCa s.r.o.",
				ProjectDescription = "Simple frame AS",
			};

			IomHelpers.AddNode(model, 1, "N1", 0, 0, 0);
			IomHelpers.AddNode(model, 2, "N2", 0, 0, 3);
			IomHelpers.AddNode(model, 3, "N3", 0, 0, 6);
			IomHelpers.AddNode(model, 4, "N4", 3, 0, 3);

			// Only the name: the properties of the material come from the library.
			var material = new MatSteelAUS { Id = 1, Name = "C350", LoadFromLibrary = true };
			IomHelpers.Add(model, material);

			CrossSection columnSection = IomHelpers.CreateRolledI(1, "125TFB", material);
			CrossSection beamSection = IomHelpers.CreateRolledI(2, "100TFB", material);
			IomHelpers.Add(model, columnSection);
			IomHelpers.Add(model, beamSection);

			Member1D column = IomHelpers.AddMember(model, 1, Member1DType.Column, columnSection, "N1", "N2", "N3");
			Member1D beam = IomHelpers.AddMember(model, 2, Member1DType.Beam, beamSection, "N2", "N4");

			ConnectionPoint connectionPoint = IomHelpers.AddConnectionPoint(model, "N2", IomHelpers.Continuous(column), IomHelpers.Ended(beam));
			model.Connections.Add(new ConnectionData
			{
				ConnectionPoint = new ReferenceElement(connectionPoint),
				Beams = new List<BeamData> { IomHelpers.CreateBeamData(column), IomHelpers.CreateBeamData(beam) },
			});

			return model;
		}
	}
}
