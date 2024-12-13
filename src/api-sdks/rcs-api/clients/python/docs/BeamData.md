# BeamData

Provides data of the connected beam

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of the beam | [optional] 
**plates** | [**List[PlateData]**](PlateData.md) | Plates of the beam | [optional] 
**cross_section_type** | **str** | Type of cross section | [optional] 
**mprl_name** | **str** | MPRL name of beam | [optional] 
**original_model_id** | **str** | Get or set the identification in the original model  In the case of the imported connection from another application | [optional] 
**cuts** | [**List[CutData]**](CutData.md) | Cuts on the beam | [optional] 
**is_added** | **bool** | Is added beam | [optional] 
**added_member_length** | **float** | Added beam lenght | [optional] 
**is_negative_object** | **bool** | Is negative object | [optional] 
**added_member** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**mirror_y** | **bool** | Mirror by Y | [optional] 
**ref_line_in_center_of_gravity** | **bool** | The reference line of the member is in the center of gravity of the cross-section | [optional] 
**is_bearing_member** | **bool** | Is beam bearing member | [optional] 
**auto_add_cut_by_workplane** | **bool** | Automaticali add cut by workplane if it not defined | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.beam_data import BeamData

# TODO update the JSON string below
json = "{}"
# create an instance of BeamData from a JSON string
beam_data_instance = BeamData.from_json(json)
# print the JSON string representation of the object
print(BeamData.to_json())

# convert the object into a dict
beam_data_dict = beam_data_instance.to_dict()
# create an instance of BeamData from a dict
beam_data_from_dict = BeamData.from_dict(beam_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


