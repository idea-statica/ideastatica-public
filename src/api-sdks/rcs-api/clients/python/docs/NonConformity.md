# NonConformity

Non-conformity

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**guid** | **str** | Description of the nonconformity | [optional] 
**severity** | [**NonConformitySeverity**](NonConformitySeverity.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.non_conformity import NonConformity

# TODO update the JSON string below
json = "{}"
# create an instance of NonConformity from a JSON string
non_conformity_instance = NonConformity.from_json(json)
# print the JSON string representation of the object
print(NonConformity.to_json())

# convert the object into a dict
non_conformity_dict = non_conformity_instance.to_dict()
# create an instance of NonConformity from a dict
non_conformity_from_dict = NonConformity.from_dict(non_conformity_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


