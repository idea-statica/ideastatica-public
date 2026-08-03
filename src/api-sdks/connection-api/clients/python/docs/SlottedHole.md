# SlottedHole

Slotted hole of one fastener position in one connected plate

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**position_id** | **int** | Id of the fastener position - matches the Id of the corresponding item in IdeaRS.OpenModel.Connection.FastenerGridBase.Positions | [optional] 
**plate** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**size_factor** | **float** | Ratio of the slot length to the borehole diameter - slot length &#x3D; borehole * SizeFactor | [optional] 
**angle** | **float** | Direction of the slot in the plate LCS [rad] | [optional] 

## Example

```python
from ideastatica_connection_api.models.slotted_hole import SlottedHole

# TODO update the JSON string below
json = "{}"
# create an instance of SlottedHole from a JSON string
slotted_hole_instance = SlottedHole.from_json(json)
# print the JSON string representation of the object
print(slotted_hole_instance.to_json())

# convert the object into a dict
slotted_hole_dict = slotted_hole_instance.to_dict()
# create an instance of SlottedHole from a dict
slotted_hole_from_dict = SlottedHole.from_dict(slotted_hole_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


