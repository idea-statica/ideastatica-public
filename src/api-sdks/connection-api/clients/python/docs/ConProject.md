# ConProject


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**project_id** | **str** |  | [optional] 
**project_info** | [**ConProjectData**](ConProjectData.md) |  | [optional] 
**connections** | [**List[ConConnection]**](ConConnection.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_project import ConProject

# TODO update the JSON string below
json = "{}"
# create an instance of ConProject from a JSON string
con_project_instance = ConProject.from_json(json)
# print the JSON string representation of the object
print(ConProject.to_json())

# convert the object into a dict
con_project_dict = con_project_instance.to_dict()
# create an instance of ConProject from a dict
con_project_from_dict = ConProject.from_dict(con_project_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


