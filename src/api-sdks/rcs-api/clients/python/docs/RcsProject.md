# RcsProject


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**sections** | [**List[RcsSection]**](RcsSection.md) |  | [optional] 
**check_members** | [**List[RcsCheckMember]**](RcsCheckMember.md) |  | [optional] 
**reinforced_cross_sections** | [**List[RcsReinforcedCrossSection]**](RcsReinforcedCrossSection.md) |  | [optional] 
**project_id** | **str** |  | [optional] 
**project_data** | [**RcsProjectData**](RcsProjectData.md) |  | [optional] 

## Example

```python
from ideastatica_rcs_api.models.rcs_project import RcsProject

# TODO update the JSON string below
json = "{}"
# create an instance of RcsProject from a JSON string
rcs_project_instance = RcsProject.from_json(json)
# print the JSON string representation of the object
print(RcsProject.to_json())

# convert the object into a dict
rcs_project_dict = rcs_project_instance.to_dict()
# create an instance of RcsProject from a dict
rcs_project_from_dict = RcsProject.from_dict(rcs_project_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


