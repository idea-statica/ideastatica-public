"""Builds the steel frame of the tutorial in IOM, the IDEA Open Model, and saves it as SteelFrame.xml.

Run it with the folder IDEA StatiCa is installed in as the first argument, or without one for the default folder:

    python steel_frame.py "C:\\Program Files\\IDEA StatiCa\\StatiCa 26.1"
"""

import sys
from pathlib import Path

import clr

DEFAULT_IDEA_FOLDER = r"C:\Program Files\IDEA StatiCa\StatiCa 26.1"

idea_folder = Path(sys.argv[1] if len(sys.argv) > 1 else DEFAULT_IDEA_FOLDER)
open_model_dll = idea_folder / "IdeaRS.OpenModel.dll"
if not open_model_dll.exists():
	sys.exit(f"IdeaRS.OpenModel.dll is not in '{idea_folder}'. Give the folder IDEA StatiCa is installed in as the first argument.")
clr.AddReference(str(open_model_dll))

from System.Collections.Generic import List
from IdeaRS.OpenModel import (
	CountryCode, CrossSectionConversionTable, OpenModel, OpenModelContainer, OriginSettings, ReferenceElement, Tools,
)
from IdeaRS.OpenModel.Connection import BeamData, ConnectedMember, ConnectionData, ConnectionPoint
from IdeaRS.OpenModel.CrossSection import CrossSectionParameter, CrossSectionType, ParameterString
from IdeaRS.OpenModel.Geometry3D import CoordSystemByPoint, LineSegment3D, Plane, Point3D, PolyLine3D
from IdeaRS.OpenModel.Loading import (
	CombiInputEC, CombiItem, LoadCase, LoadCaseSubType, LoadCaseType, LoadGroupEC, LoadGroupType, Relation,
	TypeCalculationCombiEC, TypeOfCombiEC, VariableType,
)
from IdeaRS.OpenModel.Material import (
	MatSteelEc2, SteelDiagramType, ThermalConductivityState, ThermalExpansionState, ThermalSpecificHeatState,
	ThermalStrainState, ThermalStressStrainState,
)
from IdeaRS.OpenModel.Model import Element1D, Member1D, Member1DType


def set_properties(obj, **properties):
	"""Sets the properties of a .NET object.

	Python would add a property the object does not have, such as a misspelt one, to the Python object only, and the
	model would not get the value. This stops the script instead.
	"""
	for name, value in properties.items():
		if not hasattr(obj, name):
			raise AttributeError(f"{type(obj).__name__} has no property '{name}'.")
		setattr(obj, name, value)
	return obj


def create(net_type, **properties):
	return set_properties(net_type(), **properties)


def find_by_name(objects, name):
	"""The object with the name. Stops the script when there is none, so a wrong name cannot leave a reference empty."""
	for obj in objects:
		if obj.Name == name:
			return obj
	raise LookupError(f"There is no object named '{name}'.")


def add(model, obj):
	"""Adds the object to the model.

	The model refuses an object with an Id it already has, and an object of a type it has no list for, without an
	error. This stops the script instead.
	"""
	result = model.AddObject(obj)
	if result != 0:
		raise ValueError(f"The model did not add the {type(obj).__name__} with Id {obj.Id}: AddObject returned {result}.")


def add_settings(model):
	model.OriginSettings = create(
		OriginSettings,
		CrossSectionConversionTable=CrossSectionConversionTable.SCIA,
		CountryCode=CountryCode.ECEN,
		ProjectName="Project",
		Author="IDEA StatiCa s.r.o.",
		ProjectDescription="Training example",
		CheckEquilibrium=True,
	)


def add_nodes(model):
	nodes = {
		"N1": (-2, 3, 0), "N2": (-2, 3, 3), "N3": (2, 3, 0),
		"N4": (2, 3, 3), "N5": (6, 3, 0), "N6": (6, 3, 3),
		"N7": (-2, 3, 6), "N8": (2, 3, 6), "N9": (6, 3, 6),
	}
	for id, (name, (x, y, z)) in enumerate(nodes.items(), start=1):
		add(model, create(Point3D, Id=id, Name=name, X=x, Y=y, Z=z))


def add_material(model):
	material = create(
		MatSteelEc2,
		Id=1,
		Name="S355",
		E=210e9,
		Poisson=0.3,
		UnitMass=7850,
		SpecificHeat=0.6,
		ThermalExpansion=0.000012,
		ThermalConductivity=45,
		IsDefaultMaterial=False,
		OrderInCode=0,
		StateOfThermalExpansion=ThermalExpansionState.Code,
		StateOfThermalConductivity=ThermalConductivityState.Code,
		StateOfThermalSpecificHeat=ThermalSpecificHeatState.Code,
		StateOfThermalStressStrain=ThermalStressStrainState.Code,
		StateOfThermalStrain=ThermalStrainState.Code,
		fy=355e6,
		fu=510e6,
		fy40=335e6,
		fu40=470e6,
		DiagramType=SteelDiagramType.Bilinear,
	)
	set_properties(material, G=material.E / (2 * (1 + material.Poisson)))
	add(model, material)


def create_rolled_i(id, name, material):
	"""A rolled I cross-section, which IDEA StatiCa finds in its library by the name."""
	cross_section = create(
		CrossSectionParameter,
		Id=id,
		Name=name,
		CrossSectionRotation=0,
		CrossSectionType=CrossSectionType.RolledI,
		Material=ReferenceElement(material),
	)
	cross_section.Parameters.Add(create(ParameterString, Name="UniqueName", Value=name))
	return cross_section


def add_cross_sections(model):
	material = find_by_name(model.MatSteel, "S355")
	add(model, create_rolled_i(1, "HE200B", material))
	add(model, create_rolled_i(2, "HE240B", material))


def add_member(model, id, member_type, cross_section, *nodes):
	polyline = PolyLine3D()
	member = create(
		Member1D,
		Id=id,
		Name=f"M{id}",
		Member1DType=member_type,
		CrossSection=ReferenceElement(cross_section),
	)

	for start, end in zip(nodes, nodes[1:]):
		segment = create(
			LineSegment3D,
			StartPoint=ReferenceElement(find_by_name(model.Point3D, start)),
			EndPoint=ReferenceElement(find_by_name(model.Point3D, end)),
		)
		set_properties(segment, Id=model.GetMaxId(segment) + 1)
		if member_type == Member1DType.Column:
			# The local z axis of a column lies in the plane of its axis and a point far along global X.
			set_properties(segment, LocalCoordinateSystem=create(
				CoordSystemByPoint,
				Point=create(Point3D, X=100000, Y=0, Z=0),
				InPlane=Plane.ZX,
			))
		add(model, segment)
		polyline.Segments.Add(ReferenceElement(segment))

		element = create(Element1D, Segment=ReferenceElement(segment))
		element_id = model.GetMaxId(element) + 1
		set_properties(element, Id=element_id, Name=f"E{element_id}")
		add(model, element)
		member.Elements1D.Add(ReferenceElement(element))

	set_properties(polyline, Id=model.GetMaxId(polyline) + 1)
	add(model, polyline)
	add(model, member)


def add_members(model):
	beam = find_by_name(model.CrossSection, "HE200B")
	column = find_by_name(model.CrossSection, "HE240B")

	add_member(model, 1, Member1DType.Beam, beam, "N2", "N4")            # left floor beam
	add_member(model, 2, Member1DType.Beam, beam, "N4", "N6")            # right floor beam
	add_member(model, 3, Member1DType.Column, column, "N1", "N2", "N7")  # left column
	add_member(model, 4, Member1DType.Column, column, "N3", "N4", "N8")  # middle column
	add_member(model, 5, Member1DType.Column, column, "N5", "N6", "N9")  # right column
	add_member(model, 6, Member1DType.Beam, beam, "N7", "N8", "N9")      # roof beam


def add_load_case(model, id, name, load_type, subtype, load_group):
	add(model, create(
		LoadCase,
		Id=id,
		Name=name,
		LoadType=load_type,
		Type=subtype,
		Variable=VariableType.Standard,
		LoadGroup=ReferenceElement(find_by_name(model.LoadGroup, load_group)),
	))


def add_load_cases(model):
	add(model, create(
		LoadGroupEC,
		Id=1,
		Name="PERM1",
		Relation=Relation.Standard,
		GroupType=LoadGroupType.Permanent,
		GammaQ=1.35,
		Dzeta=0.85,
		GammaGInf=1,
		GammaGSup=1.35,
	))
	add(model, create(
		LoadGroupEC,
		Id=2,
		Name="VAR1",
		Relation=Relation.Exclusive,
		GroupType=LoadGroupType.Variable,
		GammaQ=1.5,
		Dzeta=0.85,
		GammaGInf=0,
		GammaGSup=1.5,
		Psi0=0.7,
		Psi1=0.5,
		Psi2=0.3,
	))

	add_load_case(model, 1, "SelfWeight", LoadCaseType.Permanent, LoadCaseSubType.PermanentStandard, "PERM1")
	add_load_case(model, 2, "PermanentLoading", LoadCaseType.Permanent, LoadCaseSubType.PermanentStandard, "PERM1")
	add_load_case(model, 3, "LiveLoad", LoadCaseType.Variable, LoadCaseSubType.VariableStatic, "VAR1")


def add_uls_combination(model, name, *load_cases):
	combination = create(
		CombiInputEC,
		Name=name,
		Description=" + ".join(load_cases),
		TypeCombiEC=TypeOfCombiEC.ULS,
		TypeCalculationCombi=TypeCalculationCombiEC.Linear,
	)
	set_properties(combination, Id=model.GetMaxId(combination) + 1)

	for id, load_case in enumerate(load_cases, start=1):
		combination.Items.Add(create(
			CombiItem,
			Id=id,
			Coeff=1,
			LoadCase=ReferenceElement(find_by_name(model.LoadCase, load_case)),
		))

	add(model, combination)


def add_combinations(model):
	add_uls_combination(model, "Co.#1", "SelfWeight", "PermanentLoading", "LiveLoad")
	add_uls_combination(model, "Co.#2", "SelfWeight")


def connected_member(member, is_continuous):
	return create(ConnectedMember, Id=member.Id, MemberId=ReferenceElement(member), IsContinuous=is_continuous)


def create_beam_data(member):
	return create(BeamData, Id=member.Id, Name=member.Name, OriginalModelId=str(member.Id))


def add_connection_n2(model):
	"""The connection at node N2, with the members only.

	The left floor beam M1 ends there, and the left column M3 runs through. The engineer designs the connection in
	IDEA StatiCa Connection.
	"""
	beam = find_by_name(model.Member1D, "M1")
	column = find_by_name(model.Member1D, "M3")

	connection_point = create(ConnectionPoint, Name="N2", Node=ReferenceElement(find_by_name(model.Point3D, "N2")))
	set_properties(connection_point, Id=model.GetMaxId(connection_point) + 1)
	connection_point.ConnectedMembers.Add(connected_member(beam, is_continuous=False))
	connection_point.ConnectedMembers.Add(connected_member(column, is_continuous=True))
	add(model, connection_point)

	beams = List[BeamData]()
	beams.Add(create_beam_data(beam))
	beams.Add(create_beam_data(column))
	model.Connections.Add(create(ConnectionData, ConnectionPoint=ReferenceElement(connection_point), Beams=beams))


def create_iom():
	model = OpenModel()
	add_settings(model)
	add_nodes(model)
	add_material(model)
	add_cross_sections(model)
	add_members(model)
	add_load_cases(model)
	add_combinations(model)
	add_connection_n2(model)
	return model


if __name__ == "__main__":
	iom_path = Path("SteelFrame.xml").resolve()
	Tools.OpenModelContainerToFile(create(OpenModelContainer, OpenModel=create_iom()), str(iom_path))
	print(f"The IOM is saved in {iom_path}")
