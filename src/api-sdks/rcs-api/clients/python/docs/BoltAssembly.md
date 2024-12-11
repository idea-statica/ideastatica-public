# BoltAssembly

Bolt assembly

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **str** | Name of bolt assembly | [optional] 
**bolt_grade** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**diameter** | **float** | Bolt diameter | [optional] 
**borehole** | **float** | Size of bore Hole | [optional] 
**head_diameter** | **float** | Diameter of the head | [optional] 
**diagonal_head_diameter** | **float** | Second diameter of the head | [optional] 
**head_height** | **float** | Thickness of head | [optional] 
**gross_area** | **float** | Gross cross-section area | [optional] 
**tensile_stress_area** | **float** | Tensile stress area | [optional] 
**nut_thickness** | **float** | Thickness of Nut | [optional] 
**washer_thickness** | **float** | Thickness of washer | [optional] 
**washer_at_head** | **bool** | Washer at head side of bolt assembly | [optional] 
**washer_at_nut** | **bool** | Is washer at Nut side of bolt assembly | [optional] 
**load_from_library** | **bool** | Load from library - try override properties from library find BoltAssembly by name | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.bolt_assembly import BoltAssembly

# TODO update the JSON string below
json = "{}"
# create an instance of BoltAssembly from a JSON string
bolt_assembly_instance = BoltAssembly.from_json(json)
# print the JSON string representation of the object
print(BoltAssembly.to_json())

# convert the object into a dict
bolt_assembly_dict = bolt_assembly_instance.to_dict()
# create an instance of BoltAssembly from a dict
bolt_assembly_from_dict = BoltAssembly.from_dict(bolt_assembly_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


