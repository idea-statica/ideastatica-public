# ConDesignSet


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **str** |  | [optional] 
**name** | **str** |  | [optional] 
**description** | **str** |  | [optional] 
**owner_id** | **str** |  | [optional] 
**type** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_design_set import ConDesignSet

# TODO update the JSON string below
json = "{}"
# create an instance of ConDesignSet from a JSON string
con_design_set_instance = ConDesignSet.from_json(json)
# print the JSON string representation of the object
print(con_design_set_instance.to_json())

# convert the object into a dict
con_design_set_dict = con_design_set_instance.to_dict()
# create an instance of ConDesignSet from a dict
con_design_set_from_dict = ConDesignSet.from_dict(con_design_set_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


