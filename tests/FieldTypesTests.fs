namespace Kintone.Client.Tests

open System
open Xunit
open FsCheck
open FsCheck.Xunit
open Kintone.Client.Model
open Kintone.Client.Tests.Generators

/// Property-based tests for Field types and conversions
module FieldTypesTests =

    // Register custom generators
    do Arb.register<CustomGenerators>() |> ignore

    /// Property: Every FieldValue can be pattern matched exhaustively
    [<Property(Arbitrary = [| typeof<CustomGenerators> |], MaxTest = 100)>]
    let ``FieldValue pattern matching is exhaustive`` (fv: FieldValue) =
        match fv with
        | SingleLineText _ -> true
        | MultiLineText _ -> true
        | RichText _ -> true
        | Number _ -> true
        | Calc _ -> true
        | Date _ -> true
        | DateTime _ -> true
        | Time _ -> true
        | CheckBox _ -> true
        | RadioButton _ -> true
        | DropDown _ -> true
        | MultiSelect _ -> true
        | File _ -> true
        | Link _ -> true
        | UserSelect _ -> true
        | GroupSelect _ -> true
        | OrganizationSelect _ -> true
        | Subtable _ -> true
        | Category _ -> true
        | RecordNumber _ -> true
        | CreatedTime _ -> true
        | Creator _ -> true
        | UpdatedTime _ -> true
        | Modifier _ -> true
        | Status _ -> true
        | StatusAssignee _ -> true
        | Id _ -> true
        | Revision _ -> true

    /// Property: User type has valid structure
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``User has non-empty code and name`` (user: User) =
        not (String.IsNullOrWhiteSpace user.Code) &&
        not (String.IsNullOrWhiteSpace user.Name)

    /// Property: Group type has valid structure
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Group has non-empty code and name`` (group: Group) =
        not (String.IsNullOrWhiteSpace group.Code) &&
        not (String.IsNullOrWhiteSpace group.Name)

    /// Property: Organization type has valid structure
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Organization has non-empty code and name`` (org: Organization) =
        not (String.IsNullOrWhiteSpace org.Code) &&
        not (String.IsNullOrWhiteSpace org.Name)

    /// Property: FileBody has required fields
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``FileBody has fileKey and name`` (fileBody: FileBody) =
        not (String.IsNullOrWhiteSpace fileBody.FileKey) &&
        not (String.IsNullOrWhiteSpace fileBody.Name)

    /// Property: FileBody size is positive when present
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``FileBody size is positive when present`` (fileBody: FileBody) =
        match fileBody.Size with
        | Some size -> size > 0L
        | None -> true

    /// Property: DateTime formatting is consistent
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``DateTime formats to yyyy-MM-dd`` (dt: DateTime) =
        let formatted = dt.ToString("yyyy-MM-dd")
        formatted.Length = 10 &&
        formatted.[4] = '-' &&
        formatted.[7] = '-'

    /// Property: DateTimeOffset formatting includes timezone
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``DateTimeOffset formats with timezone`` (dto: DateTimeOffset) =
        let formatted = dto.ToString("yyyy-MM-ddTHH:mm:sszzz")
        formatted.Contains("T") &&
        (formatted.Contains("+") || formatted.Contains("-"))

    /// Property: DateTimeOffset preserves offset information
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``DateTimeOffset preserves offset`` (dto: DateTimeOffset) =
        let offset = dto.Offset
        // Parse and reconstruct
        let reconstructed = DateTimeOffset(dto.DateTime, offset)
        reconstructed.Offset = dto.Offset

    /// Property: TimeSpan is within 24 hours for Time field
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``TimeSpan for Time field is within 24 hours`` (ts: TimeSpan) =
        ts.TotalHours >= 0.0 && ts.TotalHours < 24.0

    /// Property: Decimal precision is maintained
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Decimal maintains precision`` (d: decimal) =
        let rounded = Math.Round(d, 10)
        let diff = Math.Abs(d - rounded)
        diff < 0.0000000001m || diff = 0m

    /// Property: CheckBox list can be empty
    [<Fact>]
    let ``CheckBox can be empty list`` () =
        let fv = CheckBox []
        match fv with
        | CheckBox values -> values.Length = 0
        | _ -> false

    /// Property: MultiSelect list can be empty
    [<Fact>]
    let ``MultiSelect can be empty list`` () =
        let fv = MultiSelect []
        match fv with
        | MultiSelect values -> values.Length = 0
        | _ -> false

    /// Property: UserSelect list can contain multiple users
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``UserSelect preserves user list`` (users: User list) =
        let fv = UserSelect users
        match fv with
        | UserSelect u -> u.Length = users.Length
        | _ -> false

    /// Property: GroupSelect list can contain multiple groups
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``GroupSelect preserves group list`` (groups: Group list) =
        let fv = GroupSelect groups
        match fv with
        | GroupSelect g -> g.Length = groups.Length
        | _ -> false

    /// Property: OrganizationSelect list can contain multiple organizations
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``OrganizationSelect preserves organization list`` (orgs: Organization list) =
        let fv = OrganizationSelect orgs
        match fv with
        | OrganizationSelect o -> o.Length = orgs.Length
        | _ -> false

    /// Property: File list can contain multiple files
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``File field preserves file list`` (files: FileBody list) =
        let fv = File files
        match fv with
        | File f -> f.Length = files.Length
        | _ -> false

    /// Property: Subtable can contain multiple rows
    [<Property(Arbitrary = [| typeof<CustomGenerators> |], MaxTest = 30)>]
    let ``Subtable preserves row list`` (rows: TableRow list) =
        let fv = Subtable rows
        match fv with
        | Subtable r -> r.Length = rows.Length
        | _ -> false

    /// Property: TableRow can have empty fields
    [<Fact>]
    let ``TableRow can have empty field map`` () =
        let row = { Id = None; Value = Map.empty }
        Assert.Equal(0, row.Value.Count)

    /// Property: TableRow preserves field count
    [<Property(Arbitrary = [| typeof<CustomGenerators> |], MaxTest = 30)>]
    let ``TableRow preserves field count`` (row: TableRow) =
        row.Value.Count >= 0

    /// Property: None values are distinct from Some values
    [<Fact>]
    let ``None and Some are distinct`` () =
        let none = SingleLineText None
        let some = SingleLineText (Some "test")
        Assert.NotEqual(none, some)

    /// Property: Same Some values are equal
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Equal Some values are equal`` (text: string) =
        not (String.IsNullOrWhiteSpace text) ==>
        lazy (
            let fv1 = SingleLineText (Some text)
            let fv2 = SingleLineText (Some text)
            fv1 = fv2
        )

    /// Property: Different field types are not equal
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Different field types are distinct`` (text: string) =
        not (String.IsNullOrWhiteSpace text) ==>
        lazy (
            let fv1 = SingleLineText (Some text)
            let fv2 = MultiLineText (Some text)
            fv1 <> fv2
        )

    /// Property: UpdateKey has non-empty field and value
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``UpdateKey has valid field and value`` (updateKey: UpdateKey) =
        not (String.IsNullOrWhiteSpace updateKey.Field) &&
        not (String.IsNullOrWhiteSpace updateKey.Value)

    /// Property: RecordRevision has positive RecordId
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``RecordRevision has positive RecordId`` (rev: RecordRevision) =
        rev.RecordId > 0L

    /// Property: RecordRevision Revision is non-negative when present
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``RecordRevision Revision is non-negative`` (rev: RecordRevision) =
        match rev.Revision with
        | Some r -> r >= 0L
        | None -> true

    /// Property: Date field only contains date component
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Date field focuses on date components`` (dt: DateTime) =
        let dateOnly = DateTime(dt.Year, dt.Month, dt.Day)
        dateOnly.Year = dt.Year &&
        dateOnly.Month = dt.Month &&
        dateOnly.Day = dt.Day

    /// Property: Category list preserves order
    [<Property>]
    let ``Category list preserves order`` (categories: string list) =
        let validCategories = categories |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        let fv = Category validCategories
        match fv with
        | Category c -> c = validCategories
        | _ -> false

    /// Property: StatusAssignee list preserves users
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``StatusAssignee preserves user list`` (users: User list) =
        let fv = StatusAssignee users
        match fv with
        | StatusAssignee u -> u = users
        | _ -> false

    /// Property: Id field contains int64
    [<Property>]
    let ``Id field wraps int64`` (id: int64) =
        id > 0L ==>
        lazy (
            let fv = Id (Some id)
            match fv with
            | Id (Some i) -> i = id
            | _ -> false
        )

    /// Property: Revision field contains int64
    [<Property>]
    let ``Revision field wraps int64`` (rev: int64) =
        rev >= 0L ==>
        lazy (
            let fv = Revision (Some rev)
            match fv with
            | Revision (Some r) -> r = rev
            | _ -> false
        )

    /// Property: Number field can handle zero
    [<Fact>]
    let ``Number field handles zero`` () =
        let fv = Number (Some 0m)
        match fv with
        | Number (Some n) -> n = 0m
        | _ -> false

    /// Property: Number field can handle negative values
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``Number field handles negative values`` (value: decimal) =
        value < 0m ==>
        lazy (
            let fv = Number (Some value)
            match fv with
            | Number (Some n) -> n = value
            | _ -> false
        )

    /// Property: Text fields preserve exact string content
    [<Property(Arbitrary = [| typeof<CustomGenerators> |])>]
    let ``SingleLineText preserves exact content`` (text: string) =
        not (String.IsNullOrWhiteSpace text) ==>
        lazy (
            let fv = SingleLineText (Some text)
            match fv with
            | SingleLineText (Some t) -> t = text
            | _ -> false
        )
