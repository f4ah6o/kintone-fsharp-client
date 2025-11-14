namespace Kintone.Client.Tests

open System
open Xunit
open FsCheck
open FsCheck.Xunit
open Kintone.Client.Model
open Kintone.Client.Tests.Generators

/// Property-based tests for Record operations
module RecordOperationsTests =

    // Register custom generators
    do Arb.register<CustomGenerators>() |> ignore

    /// Property: putField then getField returns the same value
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``putField and getField are inverse operations`` (fieldCode: string) (fieldValue: FieldValue) (record: Record) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            let updated = Record.putField fieldCode fieldValue record
            let retrieved = Record.getField fieldCode updated
            retrieved = Some fieldValue
        )

    /// Property: putField preserves Record ID
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``putField preserves Record ID`` (fieldCode: string) (fieldValue: FieldValue) (record: Record) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            let updated = Record.putField fieldCode fieldValue record
            updated.Id = record.Id
        )

    /// Property: putField preserves Record Revision
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``putField preserves Record Revision`` (fieldCode: string) (fieldValue: FieldValue) (record: Record) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            let updated = Record.putField fieldCode fieldValue record
            updated.Revision = record.Revision
        )

    /// Property: Multiple putField operations preserve all fields
    [<Property(Arbitrary = [| typeof<CustomGenerators> |], MaxTest = 50)>]
    let ``Multiple putField operations preserve all values`` (record: Record) (updates: (string * FieldValue) list) =
        let validUpdates = updates |> List.filter (fun (code, _) -> not (String.IsNullOrWhiteSpace code))
        validUpdates.Length > 0 ==>
        lazy (
            let final = validUpdates |> List.fold (fun acc (code, fv) ->
                Record.putField code fv acc) record
            let expected =
                validUpdates
                |> List.fold (fun acc (code, fv) -> Map.add code fv acc) Map.empty
            expected |> Map.forall (fun code fv ->
                Record.getField code final = Some fv
            )
        )

    /// Property: getField returns None for non-existent field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``getField returns None for non-existent field`` (record: Record) =
        let nonExistentField = "___nonexistent_field_xyz_123___"
        Record.getField nonExistentField record = None

    /// Property: getSingleLineText returns correct value for SingleLineText field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``getSingleLineText returns value for SingleLineText field`` (fieldCode: string) (text: string) =
        (not (String.IsNullOrWhiteSpace fieldCode) && not (String.IsNullOrWhiteSpace text)) ==>
        lazy (
            let record = Record.empty |> Record.putField fieldCode (SingleLineText (Some text))
            Record.getSingleLineText fieldCode record = Some text
        )

    /// Property: getSingleLineText returns None for non-text field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``getSingleLineText returns None for Number field`` (fieldCode: string) (value: decimal) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            let record = Record.empty |> Record.putField fieldCode (Number (Some value))
            Record.getSingleLineText fieldCode record = None
        )

    /// Property: getNumber returns correct value for Number field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``getNumber returns value for Number field`` (fieldCode: string) (value: decimal) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            let record = Record.empty |> Record.putField fieldCode (Number (Some value))
            Record.getNumber fieldCode record = Some value
        )

    /// Property: getDate returns correct value for Date field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``getDate returns value for Date field`` (fieldCode: string) (date: DateTime) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            let record = Record.empty |> Record.putField fieldCode (Date (Some date))
            Record.getDate fieldCode record = Some date
        )

    /// Property: getDateTime returns correct value for DateTime field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``getDateTime returns value for DateTime field`` (fieldCode: string) (dateTime: DateTimeOffset) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            let record = Record.empty |> Record.putField fieldCode (DateTime (Some dateTime))
            Record.getDateTime fieldCode record = Some dateTime
        )

    /// Property: newFrom filters out built-in fields
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``newFrom filters out built-in fields`` (record: Record) =
        let builtInFields = ["$id"; "$revision"; "レコード番号"; "作成日時"; "作成者"; "更新日時"; "更新者"]
        let withBuiltIns =
            builtInFields
            |> List.fold (fun acc field ->
                Record.putField field (SingleLineText (Some "test")) acc
            ) record
        let copied = Record.newFrom withBuiltIns
        builtInFields |> List.forall (fun field ->
            not (copied.Fields |> Map.containsKey field)
        )

    /// Property: newFrom preserves non-built-in fields
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``newFrom preserves custom fields`` (record: Record) =
        let builtInFields = Set.ofList ["$id"; "$revision"; "レコード番号"; "作成日時"; "作成者"; "更新日時"; "更新者"]
        let customFields =
            record.Fields
            |> Map.filter (fun key _ -> not (Set.contains key builtInFields))
        let copied = Record.newFrom record
        customFields |> Map.forall (fun key value ->
            Record.getField key copied = Some value
        )

    /// Property: newFrom creates record with empty ID and Revision
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``newFrom clears ID and Revision`` (record: Record) =
        let copied = Record.newFrom record
        copied.Id = None && copied.Revision = None

    /// Property: Empty record has no fields
    [<Fact>]
    let ``Empty record has zero fields`` () =
        let empty = Record.empty
        Assert.Equal(0, empty.Fields.Count)
        Assert.True(empty.Id.IsNone)
        Assert.True(empty.Revision.IsNone)

    /// Property: putField with same field code overwrites previous value
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``putField overwrites existing field`` (fieldCode: string) (value1: FieldValue) (value2: FieldValue) =
        (not (String.IsNullOrWhiteSpace fieldCode) && value1 <> value2) ==>
        lazy (
            let record = Record.empty
                         |> Record.putField fieldCode value1
                         |> Record.putField fieldCode value2
            Record.getField fieldCode record = Some value2
        )

    /// Property: Record fields count increases with unique putField operations
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``putField increases field count for new fields`` (record: Record) (newField: string) (value: FieldValue) =
        (not (String.IsNullOrWhiteSpace newField) &&
        not (record.Fields |> Map.containsKey newField)) ==>
        lazy (
            let updated = Record.putField newField value record
            updated.Fields.Count = record.Fields.Count + 1
        )

    /// Property: Record fields count unchanged when updating existing field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``putField doesn't change count for existing fields`` (record: Record) (newValue: FieldValue) =
        record.Fields.Count > 0 ==>
        lazy (
            let existingField = record.Fields |> Map.toList |> List.head |> fst
            let updated = Record.putField existingField newValue record
            updated.Fields.Count = record.Fields.Count
        )

    /// Property: Type-safe getter returns None when field type doesn't match
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Type-safe getters enforce type safety`` (fieldCode: string) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            // Put a Number field
            let record = Record.empty |> Record.putField fieldCode (Number (Some 42m))
            // Try to get as SingleLineText - should return None
            Record.getSingleLineText fieldCode record = None &&
            // But getNumber should work
            Record.getNumber fieldCode record = Some 42m
        )

    /// Property: Putting None value still stores the field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``putField with None value stores field`` (fieldCode: string) =
        not (String.IsNullOrWhiteSpace fieldCode) ==>
        lazy (
            let record = Record.empty |> Record.putField fieldCode (SingleLineText None)
            record.Fields |> Map.containsKey fieldCode
        )

    /// Property: Fields are case-sensitive
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Field codes are case-sensitive`` (value: FieldValue) =
        let record = Record.empty
                     |> Record.putField "myField" value
        Record.getField "MyField" record = None &&
        Record.getField "myField" record = Some value

    /// Property: Multiple operations maintain map invariants
    [<Property(Arbitrary = [| typeof<CustomGenerators> |], MaxTest = 50)>]
    let ``Record maintains map invariants`` (record: Record) =
        // All keys in Fields map should be retrievable
        record.Fields
        |> Map.toList
        |> List.forall (fun (key, value) ->
            Record.getField key record = Some value
        )

    /// Property: Record ID is always positive when present
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Record ID is positive when present`` (record: Record) =
        match record.Id with
        | Some id -> id > 0L
        | None -> true

    /// Property: Record Revision is non-negative when present
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Record Revision is non-negative when present`` (record: Record) =
        match record.Revision with
        | Some rev -> rev >= 0L
        | None -> true
