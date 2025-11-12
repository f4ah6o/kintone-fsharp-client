namespace Kintone.Client

open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Text
open System.Threading.Tasks
open Kintone.Client.Model
open Kintone.Client.Api

/// HTTP method types
type HttpMethod =
    | Get
    | Post
    | Put
    | Delete

module HttpMethod =
    let toSystemHttpMethod = function
        | Get -> System.Net.Http.HttpMethod.Get
        | Post -> System.Net.Http.HttpMethod.Post
        | Put -> System.Net.Http.HttpMethod.Put
        | Delete -> System.Net.Http.HttpMethod.Delete

/// Kintone API endpoints
module KintoneApi =
    // Record APIs
    let AddRecord = "/k/v1/record.json"
    let AddRecords = "/k/v1/records.json"
    let GetRecord = "/k/v1/record.json"
    let GetRecords = "/k/v1/records.json"
    let UpdateRecord = "/k/v1/record.json"
    let UpdateRecords = "/k/v1/records.json"
    let DeleteRecords = "/k/v1/records.json"
    let CreateCursor = "/k/v1/records/cursor.json"
    let GetRecordsByCursor = "/k/v1/records/cursor.json"
    let DeleteCursor = "/k/v1/records/cursor.json"
    let AddRecordComment = "/k/v1/record/comment.json"
    let GetRecordComments = "/k/v1/record/comments.json"
    let DeleteRecordComment = "/k/v1/record/comment.json"
    let UpdateRecordAssignees = "/k/v1/record/assignees.json"
    let UpdateRecordStatus = "/k/v1/record/status.json"
    let UpdateRecordStatuses = "/k/v1/records/status.json"

    // App APIs
    let GetApp = "/k/v1/app.json"
    let GetApps = "/k/v1/apps.json"
    let AddApp = "/k/v1/preview/app.json"
    let GetFormFields = "/k/v1/app/form/fields.json"
    let AddFormFields = "/k/v1/preview/app/form/fields.json"
    let UpdateFormFields = "/k/v1/preview/app/form/fields.json"
    let DeleteFormFields = "/k/v1/preview/app/form/fields.json"
    let GetFormLayout = "/k/v1/app/form/layout.json"
    let UpdateFormLayout = "/k/v1/preview/app/form/layout.json"
    let GetViews = "/k/v1/app/views.json"
    let UpdateViews = "/k/v1/preview/app/views.json"
    let GetAppSettings = "/k/v1/app/settings.json"
    let UpdateAppSettings = "/k/v1/preview/app/settings.json"
    let DeployApp = "/k/v1/preview/app/deploy.json"
    let GetDeployStatus = "/k/v1/preview/app/deploy.json"
    let GetAppAcl = "/k/v1/app/acl.json"
    let UpdateAppAcl = "/k/v1/preview/app/acl.json"
    let GetRecordAcl = "/k/v1/record/acl.json"
    let UpdateRecordAcl = "/k/v1/preview/record/acl.json"
    let EvaluateRecordAcl = "/k/v1/record/acl/evaluate.json"
    let GetFieldAcl = "/k/v1/field/acl.json"
    let UpdateFieldAcl = "/k/v1/preview/field/acl.json"
    let GetAppCustomize = "/k/v1/app/customize.json"
    let UpdateAppCustomize = "/k/v1/preview/app/customize.json"
    let GetProcessManagement = "/k/v1/app/status.json"
    let UpdateProcessManagement = "/k/v1/preview/app/status.json"
    let GetAppPlugins = "/k/v1/app/plugins.json"
    let AddAppPlugins = "/k/v1/preview/app/plugins.json"

    // Space APIs
    let GetSpace = "/k/v1/space.json"
    let AddSpaceFromTemplate = "/k/v1/template/space.json"
    let UpdateSpace = "/k/v1/space/body.json"
    let GetSpaceMembers = "/k/v1/space/members.json"
    let UpdateSpaceMembers = "/k/v1/space/members.json"
    let AddThread = "/k/v1/space/thread.json"
    let UpdateThread = "/k/v1/space/thread.json"
    let AddGuests = "/k/v1/guests.json"

    // File APIs
    let UploadFile = "/k/v1/file.json"
    let DownloadFile = "/k/v1/file.json"

    // Plugin APIs
    let GetPlugins = "/k/v1/plugins.json"
    let InstallPlugin = "/k/v1/plugin.json"
    let UninstallPlugin = "/k/v1/plugin.json"
    let UpdatePlugin = "/k/v1/plugin.json"

    // Schema API
    let GetApiList = "/k/v1/apis.json"
    let GetApiSchema = "/k/v1/apis/{api}.json"

    // Bulk requests
    let BulkRequests = "/k/v1/bulkRequest.json"

/// HTTP client configuration
type HttpClientConfig = {
    BaseUrl: string
    Auth: Auth
    BasicAuth: BasicAuth option
    ProxyHost: Uri option
    ProxyAuth: BasicAuth option
    GuestSpaceId: int64 option
    AppendixUserAgent: string option
    ConnectionTimeout: int
    SocketTimeout: int
    ConnectionRequestTimeout: int
}

/// Internal HTTP client
type InternalClient(config: HttpClientConfig) =
    let httpClient = new HttpClient()

    do
        httpClient.BaseAddress <- Uri(config.BaseUrl)
        httpClient.Timeout <- TimeSpan.FromMilliseconds(float config.SocketTimeout)

        // Set User-Agent
        let userAgent =
            match config.AppendixUserAgent with
            | Some ua -> sprintf "kintone-fsharp-client/3.0.0 %s" ua
            | None -> "kintone-fsharp-client/3.0.0"
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent)

        // Set authentication headers
        match config.Auth with
        | Password auth ->
            let authBytes = Encoding.ASCII.GetBytes(sprintf "%s:%s" auth.Username auth.Password)
            let authHeader = Convert.ToBase64String(authBytes)
            httpClient.DefaultRequestHeaders.Add("X-Cybozu-Authorization", authHeader)
        | ApiToken auth ->
            httpClient.DefaultRequestHeaders.Add("X-Cybozu-API-Token", String.concat "," auth.Tokens)

        // Set basic auth if configured
        match config.BasicAuth with
        | Some basicAuth ->
            let authBytes = Encoding.ASCII.GetBytes(sprintf "%s:%s" basicAuth.Username basicAuth.Password)
            let authHeader = Convert.ToBase64String(authBytes)
            httpClient.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Basic", authHeader)
        | None -> ()

    /// Build the full API URL with guest space support
    member private this.BuildUrl(endpoint: string) =
        match config.GuestSpaceId with
        | Some guestSpaceId ->
            endpoint.Replace("/k/v1/", sprintf "/k/guest/%d/v1/" guestSpaceId)
        | None -> endpoint

    /// Execute HTTP request
    member this.ExecuteAsync<'TRequest, 'TResponse when 'TRequest :> Api.IKintoneRequest and 'TResponse :> Api.IKintoneResponseBody>
        (httpMethod: HttpMethod, endpoint: string, request: 'TRequest option) : Task<Api.KintoneResponse<'TResponse>> =
        task {
            let url = this.BuildUrl(endpoint)
            let httpRequestMessage = new HttpRequestMessage(HttpMethod.toSystemHttpMethod httpMethod, url)

            // Serialize request body for POST/PUT
            match httpMethod, request with
            | (Post | Put), Some req ->
                let json = JsonSerialization.serialize req
                httpRequestMessage.Content <- new StringContent(json, Encoding.UTF8, "application/json")
            | _ -> ()

            let! response = httpClient.SendAsync(httpRequestMessage)
            let! responseBody = response.Content.ReadAsStringAsync()

            let kintoneResponse =
                if response.IsSuccessStatusCode then
                    let body = JsonSerialization.deserialize<'TResponse> responseBody
                    {
                        StatusCode = int response.StatusCode
                        Headers = Map.empty // TODO: Extract headers
                        Body = Some body
                        ErrorBody = None
                    }
                else
                    {
                        StatusCode = int response.StatusCode
                        Headers = Map.empty
                        Body = None
                        ErrorBody = Some responseBody
                    }

            return kintoneResponse
        }

    /// Call API with error handling
    member this.Call<'TRequest, 'TResponse when 'TRequest :> Api.IKintoneRequest and 'TResponse :> Api.IKintoneResponseBody>
        (httpMethod: HttpMethod, endpoint: string, request: 'TRequest option) : 'TResponse =
        let response = this.ExecuteAsync<'TRequest, 'TResponse>(httpMethod, endpoint, request) |> Async.AwaitTask |> Async.RunSynchronously

        match response.Body with
        | Some body -> body
        | None ->
            let errorMessage =
                match response.ErrorBody with
                | Some err -> err
                | None -> sprintf "HTTP %d" response.StatusCode
            failwith (sprintf "Kintone API error: %s" errorMessage)

    interface IDisposable with
        member this.Dispose() =
            httpClient.Dispose()
