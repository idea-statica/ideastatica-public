# CheckResPlate


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** |  | [optional] 
**check_status** | **bool** |  | [optional] 
**load_case_id** | **int** |  | [optional] 
**max_strain** | **float** |  | [optional] 
**max_stress** | **float** |  | [optional] 
**items** | **List[int]** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.check_res_plate import CheckResPlate

# TODO update the JSON string below
json = "{}"
# create an instance of CheckResPlate from a JSON string
check_res_plate_instance = CheckResPlate.from_json(json)
# print the JSON string representation of the object
print(check_res_plate_instance.to_json())

# convert the object into a dict
check_res_plate_dict = check_res_plate_instance.to_dict()
# create an instance of CheckResPlate from a dict
check_res_plate_from_dict = CheckResPlate.from_dict(check_res_plate_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


