# IdeaStatiCa.ConnectionApi.Model.TemplateConversionsConversionsInner
Polymorphic conversion root. Every element on the wire is one of the concrete subtypes listed in the discriminator mapping and carries the $type discriminator; $type is deliberately declared on each subtype schema (with its exact wire value as default) rather than here.

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**OriginalValue** | **string** |  | [optional] 
**OriginalTemplateId** | **string** |  | [optional] 
**NewValue** | **string** |  | [optional] 
**Description** | **string** |  | [optional] 
**NewTemplateId** | **string** |  | [optional] 
**Type** | **string** |  | [optional] [default to "IdeaStatiCa.Api.Connection.Model.PinTemplateConversion, IdeaStatiCa.Api"]
**IsBearing** | **bool** |  | [optional] 
**OriginalMemberName** | **string** |  | [optional] 
**NewMemberName** | **string** |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

