# IdeaStatiCa.ConnectionApi.Model.BeamData
Provides data of the connected beam

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Name** | **string** | Name of the beam | [optional] 
**Plates** | [**List&lt;PlateData&gt;**](PlateData.md) | Plates of the beam | [optional] 
**CrossSectionType** | **string** | Type of cross section | [optional] 
**MprlName** | **string** | MPRL name of beam | [optional] 
**OriginalModelId** | **string** | Get or set the identification in the original model  In the case of the imported connection from another application | [optional] 
**Cuts** | [**List&lt;CutData&gt;**](CutData.md) | Cuts on the beam | [optional] 
**IsAdded** | **bool** | Is added beam | [optional] 
**AddedMemberLength** | **double** | Added beam lenght | [optional] 
**IsNegativeObject** | **bool** | Is negative object | [optional] 
**AddedMember** | [**ReferenceElement**](ReferenceElement.md) |  | [optional] 
**MirrorY** | **bool** | Mirror by Y | [optional] 
**RefLineInCenterOfGravity** | **bool** | The reference line of the member is in the center of gravity of the cross-section | [optional] 
**IsBearingMember** | **bool** | Is beam bearing member | [optional] 
**AutoAddCutByWorkplane** | **bool** | Automaticali add cut by workplane if it not defined | [optional] 
**Id** | **int** | Element Id | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

