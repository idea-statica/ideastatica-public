# Pin

Pin

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of pin | [optional] 
**material** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**diameter** | **float** | Pin diameter | [optional] 
**hole_diameter** | **float** | Size of hole Diameter | [optional] 
**has_pin_cap** | **bool** | Has Pin Cap | [optional] 
**pin_cap_diameter** | **float** | Pin Cap Diameter | [optional] 
**pin_cap_thickness** | **float** | Thickness of Pin Cap | [optional] 
**pin_overlap** | **float** | Pin Overlap | [optional] 
**load_from_library** | **bool** | Load from library - try override properties from library find Pin by name | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.pin import Pin

# TODO update the JSON string below
json = "{}"
# create an instance of Pin from a JSON string
pin_instance = Pin.from_json(json)
# print the JSON string representation of the object
print(Pin.to_json())

# convert the object into a dict
pin_dict = pin_instance.to_dict()
# create an instance of Pin from a dict
pin_from_dict = Pin.from_dict(pin_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


