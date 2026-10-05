# IdeaStatiCa.ConnectionApi.Model.ConCrossSectionDefinition
Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**MprlName** | **string** |  | [optional] 
**MaterialName** | **string** |  | [optional] 
**MirrorY** | **bool** |  | [optional] 
**MirrorZ** | **bool** |  | [optional] 
**Type** | **string** |  | [default to "IdeaStatiCa.Api.Connection.Model.Material.ConCrossSectionCustomDefinition, IdeaStatiCa.Api"]
**ShapeType** | **string** |  | [optional] 
**Dimensions** | [**List&lt;ConCrossSectionParametricDefinitionDimensionsInner&gt;**](ConCrossSectionParametricDefinitionDimensionsInner.md) |  | [optional] 
**Components** | [**List&lt;ConCrossSectionCustomComponent&gt;**](ConCrossSectionCustomComponent.md) |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

