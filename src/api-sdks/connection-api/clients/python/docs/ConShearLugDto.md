# ConShearLugDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**cross_section_id** | **int** |  | [optional] 
**length** | **float** |  | [optional] 
**position_x** | **float** |  | [optional] 
**position_y** | **float** |  | [optional] 
**rotation** | **float** |  | [optional] 
**flanges_weld** | [**ConWeldData**](ConWeldData.md) |  | [optional] 
**webs_weld** | [**ConWeldData**](ConWeldData.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_shear_lug_dto import ConShearLugDto

# TODO update the JSON string below
json = "{}"
# create an instance of ConShearLugDto from a JSON string
con_shear_lug_dto_instance = ConShearLugDto.from_json(json)
# print the JSON string representation of the object
print(con_shear_lug_dto_instance.to_json())

# convert the object into a dict
con_shear_lug_dto_dict = con_shear_lug_dto_instance.to_dict()
# create an instance of ConShearLugDto from a dict
con_shear_lug_dto_from_dict = ConShearLugDto.from_dict(con_shear_lug_dto_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


