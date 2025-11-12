namespace Kintone.Client.Model

open System

/// Record representation
type Record = {
    Id: int64 option
    Revision: int64 option
    Fields: Map<string, FieldValue>
}

module Record =
    /// Create a new empty record
    let empty = {
        Id = None
        Revision = None
        Fields = Map.empty
    }

    /// Add or update a field in the record
    let putField fieldCode fieldValue (record: Record) : Record =
        { record with Fields = Map.add fieldCode fieldValue record.Fields }

    /// Get a field value by field code
    let getField fieldCode record =
        Map.tryFind fieldCode record.Fields

    /// Get single line text field value
    let getSingleLineText fieldCode record =
        match getField fieldCode record with
        | Some (SingleLineText value) -> value
        | _ -> None

    /// Get number field value
    let getNumber fieldCode record =
        match getField fieldCode record with
        | Some (Number value) -> value
        | _ -> None

    /// Get date field value
    let getDate fieldCode record =
        match getField fieldCode record with
        | Some (Date value) -> value
        | _ -> None

    /// Get datetime field value
    let getDateTime fieldCode record =
        match getField fieldCode record with
        | Some (DateTime value) -> value
        | _ -> None

    /// Create a copy of record without built-in fields
    let newFrom record =
        let builtInFields = Set.ofList ["$id"; "$revision"; "レコード番号"; "作成日時"; "作成者"; "更新日時"; "更新者"]
        let filteredFields =
            record.Fields
            |> Map.filter (fun key _ -> not (Set.contains key builtInFields))
        { empty with Fields = filteredFields }

/// Record for update operations
type RecordForUpdate = {
    Id: int64 option
    UpdateKey: UpdateKey option
    Record: Map<string, FieldValue>
    Revision: int64 option
}

/// Record comment
type RecordComment = {
    Text: string
    Mentions: CommentMention list option
}

/// Comment mention
and CommentMention = {
    Code: string
    Type: string
}

/// Posted record comment
type PostedRecordComment = {
    Id: int64
    Text: string
    CreatedAt: DateTimeOffset
    Creator: User
    Mentions: CommentMention list
}

/// Cursor information
type Cursor = {
    Id: string
    TotalCount: int64 option
}
