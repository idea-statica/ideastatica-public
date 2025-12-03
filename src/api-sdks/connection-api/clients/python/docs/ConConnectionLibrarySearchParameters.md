# ConConnectionLibrarySearchParameters


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**members** | **List[int]** |  | [optional] 
**in_predefined_set** | **bool** |  | [optional] 
**in_company_set** | **bool** |  | [optional] 
**in_personal_set** | **bool** |  | [optional] 
**has_bolts** | [**SearchOption**](SearchOption.md) |  | [optional] 
**has_welds** | [**SearchOption**](SearchOption.md) |  | [optional] 
**has_anchor** | [**SearchOption**](SearchOption.md) |  | [optional] 
**has_clip_angles** | [**SearchOption**](SearchOption.md) |  | [optional] 
**is_moment** | [**SearchOption**](SearchOption.md) |  | [optional] 
**is_shear** | [**SearchOption**](SearchOption.md) |  | [optional] 
**is_truss** | [**SearchOption**](SearchOption.md) |  | [optional] 
**is_parametric** | [**SearchOption**](SearchOption.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_connection_library_search_parameters import ConConnectionLibrarySearchParameters

# TODO update the JSON string below
json = "{}"
# create an instance of ConConnectionLibrarySearchParameters from a JSON string
con_connection_library_search_parameters_instance = ConConnectionLibrarySearchParameters.from_json(json)
# print the JSON string representation of the object
print(con_connection_library_search_parameters_instance.to_json())

# convert the object into a dict
con_connection_library_search_parameters_dict = con_connection_library_search_parameters_instance.to_dict()
# create an instance of ConConnectionLibrarySearchParameters from a dict
con_connection_library_search_parameters_from_dict = ConConnectionLibrarySearchParameters.from_dict(con_connection_library_search_parameters_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


