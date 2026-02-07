# CheckResAnchor


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**unity_check** | **float** |  | [optional] 
**check_status** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.check_res_anchor import CheckResAnchor

# TODO update the JSON string below
json = "{}"
# create an instance of CheckResAnchor from a JSON string
check_res_anchor_instance = CheckResAnchor.from_json(json)
# print the JSON string representation of the object
print(check_res_anchor_instance.to_json())

# convert the object into a dict
check_res_anchor_dict = check_res_anchor_instance.to_dict()
# create an instance of CheckResAnchor from a dict
check_res_anchor_from_dict = CheckResAnchor.from_dict(check_res_anchor_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


