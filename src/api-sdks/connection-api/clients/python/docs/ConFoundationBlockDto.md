# ConFoundationBlockDto


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**concrete_material_id** | **int** |  | [optional] 
**offset_top** | **float** |  | [optional] 
**offset_bottom** | **float** |  | [optional] 
**offset_left** | **float** |  | [optional] 
**offset_right** | **float** |  | [optional] 
**height** | **float** |  | [optional] 
**shear_force_transfer** | [**ConShearForceTransferMethod**](ConShearForceTransferMethod.md) |  | [optional] 
**contact_type** | [**ConBasePlateContactType**](ConBasePlateContactType.md) |  | [optional] 
**mortar_thickness** | **float** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_foundation_block_dto import ConFoundationBlockDto

# TODO update the JSON string below
json = "{}"
# create an instance of ConFoundationBlockDto from a JSON string
con_foundation_block_dto_instance = ConFoundationBlockDto.from_json(json)
# print the JSON string representation of the object
print(con_foundation_block_dto_instance.to_json())

# convert the object into a dict
con_foundation_block_dto_dict = con_foundation_block_dto_instance.to_dict()
# create an instance of ConFoundationBlockDto from a dict
con_foundation_block_dto_from_dict = ConFoundationBlockDto.from_dict(con_foundation_block_dto_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


