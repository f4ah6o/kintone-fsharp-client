namespace Kintone.Client.Model

open System

/// Entity type enumeration
type EntityType =
    | User
    | Group
    | Organization

/// User representation
type User = {
    Code: string
    Name: string
}

/// Group representation
type Group = {
    Code: string
    Name: string
}

/// Organization/Department representation
type Organization = {
    Code: string
    Name: string
}

/// Entity (User, Group, or Organization)
type Entity =
    | UserEntity of User
    | GroupEntity of Group
    | OrganizationEntity of Organization

/// Order enumeration
type Order =
    | Asc
    | Desc

/// Basic authentication
type BasicAuth = {
    Username: string
    Password: string
}

/// Password authentication
type PasswordAuth = {
    Username: string
    Password: string
}

/// API token authentication
type ApiTokenAuth = {
    Tokens: string list
}

/// Authentication types
type Auth =
    | Password of PasswordAuth
    | ApiToken of ApiTokenAuth

/// File body metadata
type FileBody = {
    ContentType: string option
    FileKey: string
    Name: string
    Size: int64 option
}

/// Update key for record updates
type UpdateKey = {
    Field: string
    Value: string
}

/// Record revision information
type RecordRevision = {
    RecordId: int64
    Revision: int64 option
}

/// Status action for process management
type StatusAction = {
    Action: string
}

/// Configuration properties
module Config =
    let mutable BaseUrl = ""
    let mutable ConnectionTimeout = 60000 // ms
    let mutable SocketTimeout = 60000 // ms
    let mutable ConnectionRequestTimeout = 60000 // ms
