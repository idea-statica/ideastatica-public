# ConversionMapping


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**source_value** | **str** |  | [optional] 
**target_value** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.conversion_mapping import ConversionMapping

# TODO update the JSON string below
json = "{}"
# create an instance of ConversionMapping from a JSON string
conversion_mapping_instance = ConversionMapping.from_json(json)
# print the JSON string representation of the object
print(ConversionMapping.to_json())

# convert the object into a dict
conversion_mapping_dict = conversion_mapping_instance.to_dict()
# create an instance of ConversionMapping from a dict
conversion_mapping_from_dict = ConversionMapping.from_dict(conversion_mapping_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


