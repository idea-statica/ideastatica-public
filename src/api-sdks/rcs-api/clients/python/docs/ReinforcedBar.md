# ReinforcedBar

Reinforced bar

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**point** | [**Point2D**](Point2D.md) |  | [optional] 
**diameter** | **float** | Diameter | [optional] 
**material** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.reinforced_bar import ReinforcedBar

# TODO update the JSON string below
json = "{}"
# create an instance of ReinforcedBar from a JSON string
reinforced_bar_instance = ReinforcedBar.from_json(json)
# print the JSON string representation of the object
print(ReinforcedBar.to_json())

# convert the object into a dict
reinforced_bar_dict = reinforced_bar_instance.to_dict()
# create an instance of ReinforcedBar from a dict
reinforced_bar_from_dict = ReinforcedBar.from_dict(reinforced_bar_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


