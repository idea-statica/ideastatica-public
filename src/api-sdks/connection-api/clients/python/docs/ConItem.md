# ConItem


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_item import ConItem

# TODO update the JSON string below
json = "{}"
# create an instance of ConItem from a JSON string
con_item_instance = ConItem.from_json(json)
# print the JSON string representation of the object
print(con_item_instance.to_json())

# convert the object into a dict
con_item_dict = con_item_instance.to_dict()
# create an instance of ConItem from a dict
con_item_from_dict = ConItem.from_dict(con_item_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


