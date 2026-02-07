# BeamData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**plates** | [**List[PlateData]**](PlateData.md) |  | [optional] 
**cross_section_type** | **str** |  | [optional] 
**mprl_name** | **str** |  | [optional] 
**original_model_id** | **str** |  | [optional] 
**cuts** | [**List[CutData]**](CutData.md) |  | [optional] 
**is_added** | **bool** |  | [optional] 
**added_member_length** | **float** |  | [optional] 
**is_negative_object** | **bool** |  | [optional] 
**added_member** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**mirror_y** | **bool** |  | [optional] 
**ref_line_in_center_of_gravity** | **bool** |  | [optional] 
**is_bearing_member** | **bool** |  | [optional] 
**auto_add_cut_by_workplane** | **bool** |  | [optional] 
**id** | **int** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.beam_data import BeamData

# TODO update the JSON string below
json = "{}"
# create an instance of BeamData from a JSON string
beam_data_instance = BeamData.from_json(json)
# print the JSON string representation of the object
print(beam_data_instance.to_json())

# convert the object into a dict
beam_data_dict = beam_data_instance.to_dict()
# create an instance of BeamData from a dict
beam_data_from_dict = BeamData.from_dict(beam_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


