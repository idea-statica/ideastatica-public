# ConAlignedPlate


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**plate_side** | [**ConAlignedPlateSideCodeEnum**](ConAlignedPlateSideCodeEnum.md) |  | [optional] 
**member_id** | **int** |  | [optional] 
**part_type** | [**ConMemberPlatePartTypeEnum**](ConMemberPlatePartTypeEnum.md) |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_aligned_plate import ConAlignedPlate

# TODO update the JSON string below
json = "{}"
# create an instance of ConAlignedPlate from a JSON string
con_aligned_plate_instance = ConAlignedPlate.from_json(json)
# print the JSON string representation of the object
print(ConAlignedPlate.to_json())

# convert the object into a dict
con_aligned_plate_dict = con_aligned_plate_instance.to_dict()
# create an instance of ConAlignedPlate from a dict
con_aligned_plate_from_dict = ConAlignedPlate.from_dict(con_aligned_plate_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


