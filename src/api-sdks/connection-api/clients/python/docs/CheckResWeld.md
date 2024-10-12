# CheckResWeld

Check value for Weld

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of Weld | [optional] 
**id** | **int** | Unique id of weld | [optional] 
**unity_check** | **float** | Unity Check Stress | [optional] 
**check_status** | **bool** | Status of the Check | [optional] 
**load_case_id** | **int** | Id of Load Case | [optional] 
**items** | **List[int]** | In case of presentation of groups plates (uncoiled beams) | [optional] 

## Example

```python
from ideastatica_connection_api.models.check_res_weld import CheckResWeld

# TODO update the JSON string below
json = "{}"
# create an instance of CheckResWeld from a JSON string
check_res_weld_instance = CheckResWeld.from_json(json)
# print the JSON string representation of the object
print(CheckResWeld.to_json())

# convert the object into a dict
check_res_weld_dict = check_res_weld_instance.to_dict()
# create an instance of CheckResWeld from a dict
check_res_weld_from_dict = CheckResWeld.from_dict(check_res_weld_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


