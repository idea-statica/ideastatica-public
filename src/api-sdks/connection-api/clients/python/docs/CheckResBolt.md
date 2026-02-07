# CheckResBolt


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**unity_check** | **float** |  | [optional] 
**check_status** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.check_res_bolt import CheckResBolt

# TODO update the JSON string below
json = "{}"
# create an instance of CheckResBolt from a JSON string
check_res_bolt_instance = CheckResBolt.from_json(json)
# print the JSON string representation of the object
print(check_res_bolt_instance.to_json())

# convert the object into a dict
check_res_bolt_dict = check_res_bolt_instance.to_dict()
# create an instance of CheckResBolt from a dict
check_res_bolt_from_dict = CheckResBolt.from_dict(check_res_bolt_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


