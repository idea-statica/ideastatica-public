# OpenModel

Open model

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**version** | **str** | Data format version | [optional] 
**origin_settings** | **object** | OriginProject | [optional] 
**point3_d** | [**List[Point3D]**](Point3D.md) | List of Point3D | [optional] 
**line_segment3_d** | [**List[LineSegment3D]**](LineSegment3D.md) | List of LineSegment3D | [optional] 
**arc_segment3_d** | [**List[ArcSegment3D]**](ArcSegment3D.md) | List of ArcSegment3D | [optional] 
**poly_line3_d** | [**List[PolyLine3D]**](PolyLine3D.md) | List of PolyLine3D | [optional] 
**region3_d** | [**List[Region3D]**](Region3D.md) | List of Region3D | [optional] 
**mat_concrete** | [**List[MatConcrete]**](MatConcrete.md) | List of MatConcrete | [optional] 
**mat_reinforcement** | [**List[MatReinforcement]**](MatReinforcement.md) | List of MatReinforcement | [optional] 
**mat_steel** | [**List[MatSteel]**](MatSteel.md) | List of MatSteel | [optional] 
**mat_prestress_steel** | [**List[MatPrestressSteel]**](MatPrestressSteel.md) | List of MatPrestressSteel | [optional] 
**mat_welding** | [**List[MatWelding]**](MatWelding.md) | List of MatWelding | [optional] 
**mat_bolt_grade** | [**List[MaterialBoltGrade]**](MaterialBoltGrade.md) | List of BoltGrades | [optional] 
**cross_section** | [**List[CrossSection]**](CrossSection.md) | List of CrossSection | [optional] 
**bolt_assembly** | [**List[BoltAssembly]**](BoltAssembly.md) | List of BoltAssemblys | [optional] 
**pin** | [**List[Pin]**](Pin.md) | List of Pins | [optional] 
**reinforced_cross_section** | [**List[ReinforcedCrossSection]**](ReinforcedCrossSection.md) | List of Reinforced CrossSection | [optional] 
**hinge_element1_d** | [**List[HingeElement1D]**](HingeElement1D.md) | List of hinge elements 1D | [optional] 
**opening** | [**List[Opening]**](Opening.md) | List of openings for Detail | [optional] 
**dapped_end** | [**List[DappedEnd]**](DappedEnd.md) | List of dapped ends in Detail | [optional] 
**patch_device** | [**List[PatchDevice]**](PatchDevice.md) | List of dapped ends in Detail | [optional] 
**element1_d** | [**List[Element1D]**](Element1D.md) | List of Elements 1D | [optional] 
**beam** | [**List[Beam]**](Beam.md) | List of Elements 1D | [optional] 
**member1_d** | [**List[Member1D]**](Member1D.md) | List of Member 1D | [optional] 
**element2_d** | [**List[Element2D]**](Element2D.md) | List of Elements 2D | [optional] 
**wall** | [**List[Wall]**](Wall.md) | List of Elements 2D | [optional] 
**member2_d** | [**List[Member2D]**](Member2D.md) | List of Member 2D | [optional] 
**rigid_link** | [**List[RigidLink]**](RigidLink.md) | List of Rigid link | [optional] 
**point_on_line3_d** | [**List[PointOnLine3D]**](PointOnLine3D.md) | List of Point on line 3D | [optional] 
**point_support_node** | [**List[PointSupportNode]**](PointSupportNode.md) | List of Point support in node | [optional] 
**line_support_segment** | [**List[LineSupportSegment]**](LineSupportSegment.md) | List of Line support on segment | [optional] 
**loads_in_point** | [**List[LoadInPoint]**](LoadInPoint.md) | List of point load impulses in this load case | [optional] 
**loads_on_line** | [**List[LoadOnLine]**](LoadOnLine.md) | List of line load impulses in this load case | [optional] 
**strain_loads_on_line** | [**List[StrainLoadOnLine]**](StrainLoadOnLine.md) | List of generalized strain load impulses along the line in this load case. | [optional] 
**point_loads_on_line** | [**List[PointLoadOnLine]**](PointLoadOnLine.md) | List of point load impulses in this load case | [optional] 
**loads_on_surface** | [**List[LoadOnSurface]**](LoadOnSurface.md) | List surafce load in this load case | [optional] 
**settlements** | [**List[Settlement]**](Settlement.md) | Settlements in this load case | [optional] 
**temperature_loads_on_line** | [**List[TemperatureLoadOnLine]**](TemperatureLoadOnLine.md) | List of temperature load in this load case | [optional] 
**load_group** | [**List[LoadGroup]**](LoadGroup.md) | List of Load groups | [optional] 
**load_case** | [**List[LoadCase]**](LoadCase.md) | List of Load cases | [optional] 
**combi_input** | [**List[CombiInput]**](CombiInput.md) | List of Combinations | [optional] 
**attribute** | **List[object]** | List of attributes | [optional] 
**connection_point** | [**List[ConnectionPoint]**](ConnectionPoint.md) | List of Connection Points | [optional] 
**connections** | [**List[ConnectionData]**](ConnectionData.md) | List of Connection data | [optional] 
**reinforcement** | [**List[Reinforcement]**](Reinforcement.md) | List of reinforcement in IDEA StatiCa Detail | [optional] 
**isd_model** | [**List[ISDModel]**](ISDModel.md) | List of Details | [optional] 
**initial_imperfection_of_point** | [**List[InitialImperfectionOfPoint]**](InitialImperfectionOfPoint.md) | List of InitialmperfectionOfPoint | [optional] 
**tendon** | [**List[Tendon]**](Tendon.md) | Tendon | [optional] 
**result_class** | [**List[ResultClass]**](ResultClass.md) | Result Class | [optional] 
**design_member** | [**List[DesignMember]**](DesignMember.md) | Design Member | [optional] 
**sub_structure** | [**List[SubStructure]**](SubStructure.md) | Design Member | [optional] 
**connection_setup** | [**ConnectionSetup**](ConnectionSetup.md) |  | [optional] 
**project_data** | **object** | Defines certain data about user project. | [optional] 
**check_member** | [**List[CheckMember]**](CheckMember.md) | List of the Check members | [optional] 
**concrete_check_section** | [**List[CheckSection]**](CheckSection.md) | List of the concrete check section | [optional] 
**concrete_setup** | [**ConcreteSetup**](ConcreteSetup.md) |  | [optional] 
**project_data_concrete** | **object** | Project data concrete | [optional] 
**rebar_shape** | [**List[RebarShape]**](RebarShape.md) | Gets or sets the rebars shapes | [optional] 
**rebar_general** | [**List[RebarGeneral]**](RebarGeneral.md) | Gets or sets the rebar General collection | [optional] 
**rebar_single** | [**List[RebarSingle]**](RebarSingle.md) | Gets or sets the rebar single collection | [optional] 
**rebar_stirrups** | [**List[RebarStirrups]**](RebarStirrups.md) | Gets or sets the rebar group (stirrups) collection | [optional] 
**taper** | [**List[Taper]**](Taper.md) |  | [optional] 
**span** | [**List[Span]**](Span.md) |  | [optional] 
**solid_blocks3_d** | [**List[SolidBlock3D]**](SolidBlock3D.md) | List of Solid Blocks 3D | [optional] 
**surface_supports3_d** | [**List[SurfaceSupport3D]**](SurfaceSupport3D.md) | List of Surface Supports 3D | [optional] 
**base_plates3_d** | [**List[BasePlate3D]**](BasePlate3D.md) | List of Base Plates 3D | [optional] 
**anchors3_d** | [**List[Anchor3D]**](Anchor3D.md) | List of Anchors 3D | [optional] 
**detail_load_case** | [**List[DetailLoadCase]**](DetailLoadCase.md) | List of Load cases | [optional] 
**detail_combination** | [**List[DetailCombination]**](DetailCombination.md) | List of Combinations | [optional] 

## Example

```python
from ideastatica_rcs_api.models.open_model import OpenModel

# TODO update the JSON string below
json = "{}"
# create an instance of OpenModel from a JSON string
open_model_instance = OpenModel.from_json(json)
# print the JSON string representation of the object
print(OpenModel.to_json())

# convert the object into a dict
open_model_dict = open_model_instance.to_dict()
# create an instance of OpenModel from a dict
open_model_from_dict = OpenModel.from_dict(open_model_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


