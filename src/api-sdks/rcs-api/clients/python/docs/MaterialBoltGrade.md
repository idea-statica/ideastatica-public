# MaterialBoltGrade

Material bolt grade

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**fub** | **float** | Ultimate tensile strength | [optional] 
**fyb** | **float** | Yield strength | [optional] 
**elongation** | **float** | Elongation after fracture - for 20% elongation use value 0.2 | [optional] 
**code** | [**CountryCode**](CountryCode.md) |  | [optional] 
**mprl_element_id** | **str** | Name fo the bolt grade in MPRL database | [optional] 
**mprl_table_id** | **str** | Unique ID of MPRL database table where the bolt grade was taken from. | [optional] 
**name** | **str** | Name of material | [optional] 
**load_from_library** | **bool** | Load from library - try override properties from library find material by name | [optional] 
**e** | **float** | Young&#39;s modulus | [optional] 
**g** | **float** | Shear modulus | [optional] 
**poisson** | **float** | Poisson&#39;s ratio | [optional] 
**unit_mass** | **float** | Unit weight | [optional] 
**specific_heat** | **float** | Specific heat capacity | [optional] 
**thermal_expansion** | **float** | Thermal expansion | [optional] 
**thermal_conductivity** | **float** | Thermal conductivity | [optional] 
**is_default_material** | **bool** | True if material is default material from the code | [optional] 
**order_in_code** | **int** | Order of this material in the code | [optional] 
**state_of_thermal_expansion** | [**ThermalExpansionState**](ThermalExpansionState.md) |  | [optional] 
**state_of_thermal_conductivity** | [**ThermalConductivityState**](ThermalConductivityState.md) |  | [optional] 
**state_of_thermal_specific_heat** | [**ThermalSpecificHeatState**](ThermalSpecificHeatState.md) |  | [optional] 
**state_of_thermal_stress_strain** | [**ThermalStressStrainState**](ThermalStressStrainState.md) |  | [optional] 
**state_of_thermal_strain** | [**ThermalStrainState**](ThermalStrainState.md) |  | [optional] 
**user_thermal_specific_heat_curvature** | [**Polygon2D**](Polygon2D.md) |  | [optional] 
**user_thermal_conductivity_curvature** | [**Polygon2D**](Polygon2D.md) |  | [optional] 
**user_thermal_expansion_curvature** | [**Polygon2D**](Polygon2D.md) |  | [optional] 
**user_thermal_strain_curvature** | [**Polygon2D**](Polygon2D.md) |  | [optional] 
**user_thermal_stress_strain_curvature** | [**List[TemperatureCurve2D]**](TemperatureCurve2D.md) | User-defined curvature for thermal stress,strain { Temperature &#x3D; Θ[K], {x &#x3D; ε[-], y &#x3D; σ[Pa]}} | [optional] 
**id** | **int** | Element Id | [optional] 

## Example

```python
from ideastatica_rcs_api.models.material_bolt_grade import MaterialBoltGrade

# TODO update the JSON string below
json = "{}"
# create an instance of MaterialBoltGrade from a JSON string
material_bolt_grade_instance = MaterialBoltGrade.from_json(json)
# print the JSON string representation of the object
print(MaterialBoltGrade.to_json())

# convert the object into a dict
material_bolt_grade_dict = material_bolt_grade_instance.to_dict()
# create an instance of MaterialBoltGrade from a dict
material_bolt_grade_from_dict = MaterialBoltGrade.from_dict(material_bolt_grade_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


