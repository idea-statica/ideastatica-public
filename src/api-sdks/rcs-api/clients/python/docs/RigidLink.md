# RigidLink

Rigid link between nodes

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rigid_link import RigidLink

# TODO update the JSON string below
json = "{}"
# create an instance of RigidLink from a JSON string
rigid_link_instance = RigidLink.from_json(json)
# print the JSON string representation of the object
print(RigidLink.to_json())

# convert the object into a dict
rigid_link_dict = rigid_link_instance.to_dict()
# create an instance of RigidLink from a dict
rigid_link_from_dict = RigidLink.from_dict(rigid_link_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


