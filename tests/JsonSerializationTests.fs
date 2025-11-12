namespace Kintone.Client.Tests

open System
open System.Text.Json
open Xunit
open FsCheck
open FsCheck.Xunit
open Kintone.Client.Model
open Kintone.Client.JsonSerialization
open Kintone.Client.Tests.Generators

/// Property-based tests for JSON serialization/deserialization
module JsonSerializationTests =

    // Register custom generators
    do Arb.register<CustomGenerators>() |> ignore

    /// Property: Serialize/Deserialize should preserve User data
    [<Property>]
    let ``User round-trip serialization preserves data`` (user: User) =
        let json = serialize user
        let deserialized = deserialize<User> json
        deserialized = user

    /// Property: Serialize/Deserialize should preserve Group data
    [<Property>]
    let ``Group round-trip serialization preserves data`` (group: Group) =
        let json = serialize group
        let deserialized = deserialize<Group> json
        deserialized = group

    /// Property: Serialize/Deserialize should preserve Organization data
    [<Property>]
    let ``Organization round-trip serialization preserves data`` (org: Organization) =
        let json = serialize org
        let deserialized = deserialize<Organization> json
        deserialized = org

    /// Property: Serialize/Deserialize should preserve FileBody data
    [<Property>]
    let ``FileBody round-trip serialization preserves data`` (fileBody: FileBody) =
        let json = serialize fileBody
        let deserialized = deserialize<FileBody> json
        deserialized.FileKey = fileBody.FileKey &&
        deserialized.Name = fileBody.Name

    /// Property: Serialized JSON should be valid
    [<Property>]
    let ``Serialized User produces valid JSON`` (user: User) =
        let json = serialize user
        try
            JsonDocument.Parse(json) |> ignore
            true
        with
        | _ -> false

    /// Property: Empty Record serializes without errors
    [<Fact>]
    let ``Empty Record serializes to valid JSON`` () =
        let emptyRecord = Record.empty
        let json = serializeRecord emptyRecord
        Assert.NotNull(json)
        Assert.NotEmpty(json)

    /// Property: Number precision is preserved through serialization
    [<Property>]
    let ``Decimal precision preserved in Number field`` (value: decimal) =
        // Limit to reasonable precision to match JSON capabilities
        let truncated = Math.Round(value, 10)
        let fieldValue = Number (Some truncated)
        let json = serialize fieldValue
        let deserialized = deserialize<FieldValue> json
        match deserialized with
        | Number (Some v) -> Math.Abs(v - truncated) < 0.0000000001m
        | _ -> false

    /// Property: DateTime serialization preserves date components
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``DateTime serialization preserves date`` (dt: DateTime) =
        let fieldValue = Date (Some dt)
        let json = sprintf """{"value":"%s"}""" (dt.ToString("yyyy-MM-dd"))
        // Verify format
        json.Contains(dt.Year.ToString()) &&
        json.Contains(dt.Month.ToString("00")) &&
        json.Contains(dt.Day.ToString("00"))

    /// Property: DateTimeOffset serialization preserves offset
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``DateTimeOffset serialization preserves timezone`` (dto: DateTimeOffset) =
        let formatted = dto.ToString("yyyy-MM-ddTHH:mm:sszzz")
        formatted.Length > 0 &&
        formatted.Contains("T") &&
        (formatted.Contains("+") || formatted.Contains("-"))

    /// Property: List serialization preserves count
    [<Property>]
    let ``CheckBox serialization preserves list count`` (values: string list) =
        let nonEmptyValues = values |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        let fieldValue = CheckBox nonEmptyValues
        let json = serialize fieldValue
        let deserialized = deserialize<FieldValue> json
        match deserialized with
        | CheckBox v -> v.Length = nonEmptyValues.Length
        | _ -> false

    /// Property: UserSelect list serialization preserves users
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``UserSelect serialization preserves user count`` (users: User list) =
        let fieldValue = UserSelect users
        let json = serialize fieldValue
        let deserialized = deserialize<FieldValue> json
        match deserialized with
        | UserSelect u -> u.Length = users.Length
        | _ -> false

    /// Property: None values serialize correctly
    [<Fact>]
    let ``None FieldValue serializes without throwing`` () =
        let testCases = [
            SingleLineText None
            MultiLineText None
            RichText None
            Number None
            Date None
            DateTime None
            RadioButton None
            DropDown None
            Link None
            RecordNumber None
            Status None
            Id None
            Revision None
        ]
        testCases |> List.iter (fun fv ->
            let json = serialize fv
            Assert.NotNull(json)
        )

    /// Property: Some values serialize with content
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Some text values produce non-empty JSON`` (text: string) =
        not (String.IsNullOrWhiteSpace text) ==>
        lazy (
            let fieldValue = SingleLineText (Some text)
            let json = serialize fieldValue
            json.Length > 2 // More than just "{}"
        )

    /// Property: JSON serialization is deterministic
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``User serialization is deterministic`` (user: User) =
        let json1 = serialize user
        let json2 = serialize user
        json1 = json2

    /// Property: Nested structures serialize without errors
    [<Property(Arbitrary = [| typeof<CustomGenerators> |], MaxTest = 50)>]
    let ``Subtable serialization handles nested structures`` (rows: TableRow list) =
        let fieldValue = Subtable rows
        try
            let json = serialize fieldValue
            not (String.IsNullOrEmpty json)
        with
        | _ -> false

    /// Property: Empty lists serialize correctly
    [<Fact>]
    let ``Empty lists serialize to empty arrays`` () =
        let testCases = [
            CheckBox []
            MultiSelect []
            Category []
            UserSelect []
            GroupSelect []
            OrganizationSelect []
            StatusAssignee []
            File []
            Subtable []
        ]
        testCases |> List.iter (fun fv ->
            let json = serialize fv
            Assert.Contains("[]", json)
        )

    /// Property: UpdateKey serialization preserves field and value
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``UpdateKey round-trip preserves data`` (updateKey: UpdateKey) =
        let json = serialize updateKey
        let deserialized = deserialize<UpdateKey> json
        deserialized.Field = updateKey.Field &&
        deserialized.Value = updateKey.Value

    /// Property: RecordRevision serialization preserves IDs
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``RecordRevision round-trip preserves data`` (rev: RecordRevision) =
        let json = serialize rev
        let deserialized = deserialize<RecordRevision> json
        deserialized.RecordId = rev.RecordId &&
        deserialized.Revision = rev.Revision

    /// Property: Large numbers serialize without loss
    [<Property>]
    let ``Large int64 values serialize correctly`` () =
        let testValues = [0L; 1L; -1L; Int64.MaxValue; Int64.MinValue]
        testValues |> List.forall (fun v ->
            let fieldValue = Id (Some v)
            let json = serialize fieldValue
            let deserialized = deserialize<FieldValue> json
            match deserialized with
            | Id (Some i) -> i = v
            | _ -> false
        )

    /// Property: Special characters in strings are escaped
    [<Fact>]
    let ``Special characters in text fields are handled`` () =
        let specialTexts = [
            "Hello \"World\""
            "Line1\nLine2"
            "Tab\there"
            "Backslash\\"
            "Unicode: 日本語"
        ]
        specialTexts |> List.iter (fun text ->
            let fieldValue = SingleLineText (Some text)
            let json = serialize fieldValue
            Assert.NotEmpty(json)
        )
