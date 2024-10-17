# ConProjectData


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**project_number** | **str** |  | [optional] 
**author** | **str** |  | [optional] 
**design_code** | **str** |  | [optional] 
**var_date** | **datetime** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_project_data import ConProjectData

# TODO update the JSON string below
json = "{}"
# create an instance of ConProjectData from a JSON string
con_project_data_instance = ConProjectData.from_json(json)
# print the JSON string representation of the object
print(ConProjectData.to_json())

# convert the object into a dict
con_project_data_dict = con_project_data_instance.to_dict()
# create an instance of ConProjectData from a dict
con_project_data_from_dict = ConProjectData.from_dict(con_project_data_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


