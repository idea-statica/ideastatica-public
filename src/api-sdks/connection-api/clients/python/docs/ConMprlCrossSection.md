# ConMprlCrossSection


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**material_name** | **str** |  | [optional] 
**mprl_name** | **str** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_mprl_cross_section import ConMprlCrossSection

# TODO update the JSON string below
json = "{}"
# create an instance of ConMprlCrossSection from a JSON string
con_mprl_cross_section_instance = ConMprlCrossSection.from_json(json)
# print the JSON string representation of the object
print(ConMprlCrossSection.to_json())

# convert the object into a dict
con_mprl_cross_section_dict = con_mprl_cross_section_instance.to_dict()
# create an instance of ConMprlCrossSection from a dict
con_mprl_cross_section_from_dict = ConMprlCrossSection.from_dict(con_mprl_cross_section_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


