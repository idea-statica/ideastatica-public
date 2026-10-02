# ConNegativePlateOperation


## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**material_id** | **int** |  | [optional] 
**thickness** | **float** |  | [optional] 
**shape** | [**ConPlateShape**](ConPlateShape.md) |  | [optional] 
**polygon_data** | **str** |  | [optional] 
**width** | **float** |  | [optional] 
**height** | **float** |  | [optional] 
**width2** | **float** |  | [optional] 
**height2** | **float** |  | [optional] 
**radius** | **float** |  | [optional] 
**positioning** | [**ConPlatePositioning**](ConPlatePositioning.md) |  | [optional] 
**is_imported** | **bool** |  | [optional] 
**operation_type** | **str** |  | [optional] 
**id** | **int** |  | [optional] 
**name** | **str** |  | [optional] 
**active** | **bool** |  | [optional] 

## Example

```python
from ideastatica_connection_api.models.con_negative_plate_operation import ConNegativePlateOperation

# TODO update the JSON string below
json = "{}"
# create an instance of ConNegativePlateOperation from a JSON string
con_negative_plate_operation_instance = ConNegativePlateOperation.from_json(json)
# print the JSON string representation of the object
print(con_negative_plate_operation_instance.to_json())

# convert the object into a dict
con_negative_plate_operation_dict = con_negative_plate_operation_instance.to_dict()
# create an instance of ConNegativePlateOperation from a dict
con_negative_plate_operation_from_dict = ConNegativePlateOperation.from_dict(con_negative_plate_operation_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


