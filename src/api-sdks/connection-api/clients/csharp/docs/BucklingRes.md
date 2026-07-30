# IdeaStatiCa.ConnectionApi.Model.BucklingRes
Results of the buckling analysis

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**LoadCaseId** | **int** |  | [optional] 
**Shape** | **int** | Index of the buckling mode within its load case. Mode indices restart for every load case  and the same index in two load cases is not guaranteed to be the same physical buckling  shape - matching shapes across load cases requires a visual inspection in the application | [optional] 
**Factor** | **double** | Buckling factor | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

