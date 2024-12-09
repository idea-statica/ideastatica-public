# CutBeamByBeamData

Provides data of the cut objec by object

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of the cut | [optional] 
**modified_object** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**cutting_object** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**is_weld** | **bool** | is cut welded | [optional] 
**weld_thickness** | **float** | Thickness of the weld - value 0 &#x3D; recommended size | [optional] 
**weld_type** | [**WeldType**](WeldType.md) |  | [optional] 
**offset** | **float** | Offset | [optional] 
**method** | [**CutMethod**](CutMethod.md) |  | [optional] 
**orientation** | [**CutOrientation**](CutOrientation.md) |  | [optional] 
**plane_on_cutting_object** | [**DistanceComparison**](DistanceComparison.md) |  | [optional] 
**cut_part** | [**CutPart**](CutPart.md) |  | [optional] 
**extend_before_cut** | **bool** | Extend before cut - for cuts where user can decide if modified beam will be extended or not | [optional] 

## Example

```python
from ideastatica_rcs_api.models.cut_beam_by_beam_data import CutBeamByBeamData

# TODO update the JSON string below
json = "{}"
# create an instance of CutBeamByBeamData from a JSON string
cut_beam_by_beam_data_instance = CutBeamByBeamData.from_json(json)
# print the JSON string representation of the object
print(CutBeamByBeamData.to_json())

# convert the object into a dict
cut_beam_by_beam_data_dict = cut_beam_by_beam_data_instance.to_dict()
# create an instance of CutBeamByBeamData from a dict
cut_beam_by_beam_data_from_dict = CutBeamByBeamData.from_dict(cut_beam_by_beam_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


