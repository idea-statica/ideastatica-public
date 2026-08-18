# IdeaStatiCa.ConnectionApi.Model.ConCrossSectionDetailDefinition
Polymorphic root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**DefinitionType** | **string** |  | [optional] [readonly] 
**MaterialName** | **string** |  | [optional] 
**ShapeType** | **string** |  | [optional] 
**Parameters** | [**List&lt;ConCrossSectionParameter&gt;**](ConCrossSectionParameter.md) |  | [optional] 
**Type** | **string** |  | [optional] [default to "IdeaStatiCa.Api.Connection.Model.Material.ConCrossSectionCustomDefinition, IdeaStatiCa.Api"]
**Components** | [**List&lt;ConCrossSectionCustomComponent&gt;**](ConCrossSectionCustomComponent.md) |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

