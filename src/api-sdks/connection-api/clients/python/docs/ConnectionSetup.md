# ConnectionSetup

ConnectionSetup

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**steel_setup** | **object** | ISteelSetup | [optional] 
**concrete_setup** | [**ConcreteSetup**](ConcreteSetup.md) |  | [optional] 
**stop_at_limit_strain** | **bool** | Stop analysis when the limit strain is reached. | [optional] 
**weld_evaluation_data** | [**WeldEvaluation**](WeldEvaluation.md) |  | [optional] 
**check_detailing** | **bool** | Perform check of bolt positions | [optional] 
**apply_cone_breakout_check** | [**ConeBreakoutCheckType**](ConeBreakoutCheckType.md) |  | [optional] 
**pretension_force_fpc** | **float** | Pretension force fpc &#x3D; k * fub * As | [optional] 
**gamma_inst** | **float** | Partial safety factor of instalation safety | [optional] 
**gamma_c** | **float** | Partial safety factor of concrete | [optional] 
**gamma_m3** | **float** | Preloaded bolts safety factor | [optional] 
**anchor_length_for_stiffness** | **int** | Length of anchor to define the anchor stiffness in analysis model, as a multiple of anchor diameter (E A /n * [d]) | [optional] 
**joint_beta_factor** | **float** | Joint coefficient βj - Used for Fjd calculation | [optional] 
**effective_area_stress_coeff** | **float** | Effective area is taken from intersection of stress area and area of joined items according to EN1993-1-8 art. 6.2.5 | [optional] 
**effective_area_stress_coeff_aisc** | **float** | Effective area stress coefficient - Concrete loaded area: Stress cut-off is set for AISC | [optional] 
**friction_coefficient** | **float** | Coefficient of friction between base plate and concrete block | [optional] 
**limit_plastic_strain** | **float** | Limit of plastic strain used in 2D plate element check | [optional] 
**limit_deformation** | **float** | Limit deformation on closed sections | [optional] 
**limit_deformation_check** | **bool** | Limit deformation on closed sections check or not | [optional] 
**analysis_gnl** | **bool** | Analysis with GNL | [optional] 
**analysis_all_gnl** | **bool** | Analysis with All GNL | [optional] 
**warn_plastic_strain** | **float** | Warning plastic strain | [optional] 
**warn_check_level** | **float** | Warning check level | [optional] 
**optimal_check_level** | **float** | Optimal check level | [optional] 
**distance_between_bolts** | **float** | Limit distance between bolts as a multiple of bolt diameter | [optional] 
**distance_diameter_between_bp** | **float** | Anchor pitch | [optional] 
**distance_between_bolts_edge** | **float** | Limit distance between bolt and plate edge as a multiple of bolt diameter | [optional] 
**bearing_angle** | **float** | Load distribution angle of concrete block in calculation of factor Kj | [optional] 
**decreasing_ftrd** | **float** | Decreasing Ftrd of anchors. Worse quality influence | [optional] 
**braced_system** | **bool** | Consider the frame system as braced for stiffness calculation. Braced system reduces horizontal displacements. | [optional] 
**bearing_check** | **bool** | Apply bearing check including αb | [optional] 
**apply_betap_influence** | **bool** | Apply βp influence in bolt shear resistance. ΕΝ 1993-1-8 chapter 3.6.1 (12) | [optional] 
**member_length_ratio** | **float** | A multiple of cross-section height to determine the default length of member | [optional] 
**division_of_surface_of_chs** | **int** | Number of straight lines to substitute circle of circular tube in analysis model | [optional] 
**division_of_arcs_of_rhs** | **int** | Number of straight lines to substitute corner arc of rectangular tubes in analysis model | [optional] 
**num_element** | **int** | Ratio of length of decisive plate edge and Elements on edge count determines the average size of mesh element | [optional] 
**number_iterations** | **int** | More iterations helps to find better solutions in contact elements but increases calculation time | [optional] 
**mdiv** | **int** | Number of iteration steps to evaluate analysis divergence | [optional] 
**min_size** | **float** | Minimal size of generated finite mesh element | [optional] 
**max_size** | **float** | Maximal size of generated finite mesh element | [optional] 
**num_element_rhs** | **int** | Number of mesh elements in RHS height | [optional] 
**num_element_plate** | **int** | Number of mesh elements on plates | [optional] 
**rigid_bp** | **bool** | True if rigid base plate is considered | [optional] 
**alpha_cc** | **float** | Long-term effect on fcd | [optional] 
**cracked_concrete** | **bool** | True if cracked concrete is considered | [optional] 
**developed_fillers** | **bool** | True if developed fillers is considered | [optional] 
**deformation_bolt_hole** | **bool** | True if bolt hole deformation is considered | [optional] 
**extension_length_ration_open_sections** | **float** | ExtensionLengthRationOpenSections | [optional] 
**extension_length_ration_close_sections** | **float** | ExtensionLengthRationCloseSections | [optional] 
**factor_preload_bolt** | **float** | FactorPreloadBolt | [optional] 
**base_metal_capacity** | **bool** | BaseMetalCapacity | [optional] 
**apply_bearing_check** | **bool** | ApplyBearingCheck | [optional] 
**friction_coefficient_pbolt** | **float** | Friction factor of slip-resistant joint | [optional] 
**crt_comp_check_is** | [**CrtCompCheckIS**](CrtCompCheckIS.md) |  | [optional] 
**bolt_max_grip_length_coeff** | **float** | Max value of bolt grip IND | [optional] 
**fatigue_section_offset** | **float** | Fatigue section Offset &#x3D; FatigueSectionOffset x Legsize | [optional] 
**condensed_element_length_factor** | **float** | Condensed element length factor (CEF). Condensed beam legth &#x3D; maxCssSize * CEF | [optional] 
**gamma_mu** | **float** | Partial safety factor for Horizontal tying | [optional] 
**hss_limit_plastic_strain** | **float** | Limit plastic strain for high strength steel | [optional] 

## Example

```python
from ideastatica_connection_api.models.connection_setup import ConnectionSetup

# TODO update the JSON string below
json = "{}"
# create an instance of ConnectionSetup from a JSON string
connection_setup_instance = ConnectionSetup.from_json(json)
# print the JSON string representation of the object
print(ConnectionSetup.to_json())

# convert the object into a dict
connection_setup_dict = connection_setup_instance.to_dict()
# create an instance of ConnectionSetup from a dict
connection_setup_from_dict = ConnectionSetup.from_dict(connection_setup_dict)
```
[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)


