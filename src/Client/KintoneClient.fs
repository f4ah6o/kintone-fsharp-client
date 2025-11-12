namespace Kintone.Client

open System
open Kintone.Client.Model

/// Kintone client builder
type KintoneClientBuilder() =
    let mutable baseUrl = ""
    let mutable auth = None
    let mutable basicAuth = None
    let mutable proxyHost = None
    let mutable proxyAuth = None
    let mutable guestSpaceId = None
    let mutable appendixUserAgent = None
    let mutable connectionTimeout = 60000
    let mutable socketTimeout = 60000
    let mutable connectionRequestTimeout = 60000

    /// Create a new builder
    static member Create(url: string) =
        let builder = KintoneClientBuilder()
        builder.SetBaseUrl(url)

    /// Set base URL
    member this.SetBaseUrl(url: string) =
        baseUrl <- url
        this

    /// Set password authentication
    member this.AuthByPassword(username: string, password: string) =
        auth <- Some (Password { Username = username; Password = password })
        this

    /// Set API token authentication
    member this.AuthByApiToken(token: string) =
        auth <- Some (ApiToken { Tokens = [token] })
        this

    /// Set API token authentication with multiple tokens
    member this.AuthByApiTokens(tokens: string list) =
        auth <- Some (ApiToken { Tokens = tokens })
        this

    /// Set basic authentication
    member this.WithBasicAuth(username: string, password: string) =
        basicAuth <- Some ({ Username = username; Password = password } : BasicAuth)
        this

    /// Set guest space ID
    member this.SetGuestSpaceId(id: int64) =
        guestSpaceId <- Some id
        this

    /// Set additional user agent
    member this.SetAppendixUserAgent(userAgent: string) =
        appendixUserAgent <- Some userAgent
        this

    /// Set connection timeout
    member this.SetConnectionTimeout(timeout: int) =
        connectionTimeout <- timeout
        this

    /// Set socket timeout
    member this.SetSocketTimeout(timeout: int) =
        socketTimeout <- timeout
        this

    /// Set connection request timeout
    member this.SetConnectionRequestTimeout(timeout: int) =
        connectionRequestTimeout <- timeout
        this

    /// Build the client
    member this.Build() =
        match auth with
        | None -> failwith "Authentication must be configured"
        | Some authConfig ->
            let config = {
                BaseUrl = baseUrl
                Auth = authConfig
                BasicAuth = basicAuth
                ProxyHost = proxyHost
                ProxyAuth = proxyAuth
                GuestSpaceId = guestSpaceId
                AppendixUserAgent = appendixUserAgent
                ConnectionTimeout = connectionTimeout
                SocketTimeout = socketTimeout
                ConnectionRequestTimeout = connectionRequestTimeout
            }
            new KintoneClient(new InternalClient(config))

    /// Create a default client with password authentication
    static member DefaultClient(baseUrl: string, username: string, password: string) =
        KintoneClientBuilder.Create(baseUrl)
            .AuthByPassword(username, password)
            .Build()

    /// Create a default client with API token authentication
    static member DefaultClientWithToken(baseUrl: string, token: string) =
        KintoneClientBuilder.Create(baseUrl)
            .AuthByApiToken(token)
            .Build()

/// Main Kintone client
and KintoneClient(internalClient: InternalClient) =
    let recordClient = RecordClient(internalClient)
    let appClient = AppClient(internalClient)
    let spaceClient = SpaceClient(internalClient)
    let fileClient = FileClient(internalClient)
    let schemaClient = SchemaClient(internalClient)
    let pluginClient = PluginClient(internalClient)

    /// Get record client
    member this.Record = recordClient

    /// Get app client
    member this.App = appClient

    /// Get space client
    member this.Space = spaceClient

    /// Get file client
    member this.File = fileClient

    /// Get schema client
    member this.Schema = schemaClient

    /// Get plugin client
    member this.Plugin = pluginClient

    interface IDisposable with
        member this.Dispose() =
            (internalClient :> IDisposable).Dispose()
