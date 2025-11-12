namespace Kintone.Client.Model

open System

/// Space type
type SpaceType =
    | NormalSpace
    | GuestSpace

/// Space body
type SpaceBody = {
    Id: int64
    Name: string
    DefaultThread: int64
    IsPrivate: bool
    Creator: User
    Modifier: User
    MemberCount: int
    CoverType: string option
    CoverKey: string option
    CoverUrl: string option
    Body: string option
    UseMultiThread: bool option
    IsGuest: bool option
    Attachments: SpaceAttachment list option
}

/// Space attachment
and SpaceAttachment = {
    Type: string
    Name: string
    Size: int64 option
}

/// Space member
type SpaceMember = {
    Entity: Entity
    IsAdmin: bool option
    IsImplicit: bool option
}

/// Space template
type SpaceTemplate = {
    Id: int64
    Name: string
}

/// Thread
type Thread = {
    Id: int64
    Name: string
}

/// Guest user
type Guest = {
    Code: string
    Password: string option
    Timezone: string option
    Locale: string option
    Name: string option
    Surnamme: string option
    GivenName: string option
    SurnameReading: string option
    GivenNameReading: string option
    Company: string option
    Division: string option
    Phone: string option
    MobilePhone: string option
    ExtensionNumber: string option
    Email: string option
    CalltoName: string option
    Url: string option
}
