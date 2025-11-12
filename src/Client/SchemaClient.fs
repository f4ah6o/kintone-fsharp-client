namespace Kintone.Client

open System
open Kintone.Client.Model
open Kintone.Client.Api

/// Schema client for API schema operations
type SchemaClient(internalClient: InternalClient) =

    /// Get list of available APIs
    member this.GetApiList() : Map<string, ApiInfo> =
        let response = internalClient.Call<EmptyRequest, GetApiListResponseBody>(HttpMethod.Get, KintoneApi.GetApiList, None)
        response.Apis

    /// Get API schema
    member this.GetApiSchema(apiName: string) : string * string =
        let endpoint = KintoneApi.GetApiSchema.Replace("{api}", apiName)
        let response = internalClient.Call<EmptyRequest, GetApiSchemaResponseBody>(HttpMethod.Get, endpoint, None)
        (response.Id, response.BaseUrl)
