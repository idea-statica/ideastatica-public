# RcsProjectData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**project_name** | **str** |  | [optional] 
**date_of_create** | **datetime** |  | [optional] 
**description** | **str** |  | [optional] 
**author** | **str** |  | [optional] 
**code** | **str** |  | [optional] 
**project_no** | **str** |  | [optional] 
**type_bridge** | **str** |  | [optional] 
**type_of_structure** | **str** |  | [optional] 
**fire_resistance_check** | **bool** |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_project_data import RcsProjectData

# TODO update the JSON string below
json = "{}"
# create an instance of RcsProjectData from a JSON string
rcs_project_data_instance = RcsProjectData.from_json(json)
# print the JSON string representation of the object
print(RcsProjectData.to_json())

# convert the object into a dict
rcs_project_data_dict = rcs_project_data_instance.to_dict()
# create an instance of RcsProjectData from a dict
rcs_project_data_from_dict = RcsProjectData.from_dict(rcs_project_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


