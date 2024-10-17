# DrawData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**groups** | [**List[IGroup]**](IGroup.md) |  | [optional] 
**vertices** | **List[float]** |  | [optional] 
**normals** | **List[float]** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.draw_data import DrawData

# TODO update the JSON string below
json = "{}"
# create an instance of DrawData from a JSON string
draw_data_instance = DrawData.from_json(json)
# print the JSON string representation of the object
print(DrawData.to_json())

# convert the object into a dict
draw_data_dict = draw_data_instance.to_dict()
# create an instance of DrawData from a dict
draw_data_from_dict = DrawData.from_dict(draw_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


