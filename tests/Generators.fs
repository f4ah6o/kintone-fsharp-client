namespace Kintone.Client.Tests

open System
open FsCheck
open Kintone.Client.Model

/// Custom FsCheck generators for Kintone domain types
module Generators =

    /// Generate non-null, non-empty strings
    let genNonEmptyString =
        Arb.generate<string>
        |> Gen.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        |> Gen.map (fun s -> s.Trim())

    /// Generate valid field codes (alphanumeric + underscore, no spaces)
    let genFieldCode =
        gen {
            let! firstChar = Gen.elements ['a'..'z']
            let! rest = Gen.arrayOf (Gen.elements (['a'..'z'] @ ['A'..'Z'] @ ['0'..'9'] @ ['_']))
            let code = string firstChar + String(rest)
            return code
        }
        |> Gen.filter (fun s -> s.Length > 0 && s.Length <= 64)

    /// Generate User
    let genUser : Gen<User> =
        gen {
            let! code = genNonEmptyString
            let! name = genNonEmptyString
            return { Code = code; Name = name }
        }

    /// Generate Group
    let genGroup : Gen<Group> =
        gen {
            let! code = genNonEmptyString
            let! name = genNonEmptyString
            return { Code = code; Name = name }
        }

    /// Generate Organization
    let genOrganization : Gen<Organization> =
        gen {
            let! code = genNonEmptyString
            let! name = genNonEmptyString
            return { Code = code; Name = name }
        }

    /// Generate FileBody
    let genFileBody =
        gen {
            let! contentType = Gen.oneof [Gen.constant None; genNonEmptyString |> Gen.map Some]
            let! fileKey = genNonEmptyString
            let! name = genNonEmptyString
            let! size = Gen.oneof [Gen.constant None; Arb.generate<int64> |> Gen.filter (fun x -> x > 0L) |> Gen.map Some]
            return {
                ContentType = contentType
                FileKey = fileKey
                Name = name
                Size = size
            }
        }

    /// Generate DateTime (valid dates only)
    let genDateTime : Gen<System.DateTime> =
        gen {
            let! year = Gen.choose(2000, 2030)
            let! month = Gen.choose(1, 12)
            let! day = Gen.choose(1, 28) // Safe range to avoid invalid dates
            let! hour = Gen.choose(0, 23)
            let! minute = Gen.choose(0, 59)
            let! second = Gen.choose(0, 59)
            return System.DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc)
        }

    /// Generate DateTimeOffset
    let genDateTimeOffset : Gen<DateTimeOffset> =
        gen {
            let! year = Gen.choose(2000, 2030)
            let! month = Gen.choose(1, 12)
            let! day = Gen.choose(1, 28) // Safe range to avoid invalid dates
            let! hour = Gen.choose(0, 23)
            let! minute = Gen.choose(0, 59)
            let! second = Gen.choose(0, 59)
            let! offsetHours = Gen.choose(-12, 12)
            // Use DateTimeKind.Unspecified to allow custom offsets
            let dt = System.DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified)
            return DateTimeOffset(dt, TimeSpan.FromHours(float offsetHours))
        }

    /// Generate TimeSpan (for Time fields)
    let genTimeSpan =
        gen {
            let! hours = Gen.choose(0, 23)
            let! minutes = Gen.choose(0, 59)
            let! seconds = Gen.choose(0, 59)
            return TimeSpan(hours, minutes, seconds)
        }

    /// Generate decimal with reasonable precision
    let genDecimal =
        gen {
            let! intPart = Gen.choose(-1000000, 1000000)
            let! fracPart = Gen.choose(0, 99999)
            return decimal intPart + (decimal fracPart / 100000m)
        }

    /// Generate TableRow (for Subtable) - mutually recursive with genFieldValue
    let rec genTableRow size =
        gen {
            let! id = Gen.oneof [Gen.constant None; genNonEmptyString |> Gen.map Some]
            let! fieldCount = Gen.choose(0, 3) // Limit subtable complexity
            let! fields =
                Gen.listOfLength fieldCount (gen {
                    let! code = genFieldCode
                    let! value = genFieldValue (size - 1)
                    return (code, value)
                })
                |> Gen.map (fun pairs -> Map.ofList pairs)
            return { Id = id; Value = fields }
        }

    /// Generate FieldValue (all 32+ cases) - uses sized to handle recursion
    and genFieldValue size : Gen<FieldValue> =
        if size <= 0 then
            // Base cases only when size is 0
            Gen.oneof [
                Gen.constant (SingleLineText None)
                Gen.constant (MultiLineText None)
                Gen.constant (Number None)
                Gen.constant (Date None)
            ]
        else
            Gen.oneof [
                // Simple text fields
                genNonEmptyString |> Gen.map (Some >> SingleLineText)
                Gen.constant (SingleLineText None)
                genNonEmptyString |> Gen.map (Some >> MultiLineText)
                Gen.constant (MultiLineText None)
                genNonEmptyString |> Gen.map (Some >> RichText)
                Gen.constant (RichText None)

                // Numeric fields
                genDecimal |> Gen.map (Some >> Number)
                Gen.constant (Number None)
                genNonEmptyString |> Gen.map (Some >> Calc)
                Gen.constant (Calc None)

                // Date/Time fields
                genDateTime |> Gen.map (Some >> Date)
                Gen.constant (Date None)
                genDateTimeOffset |> Gen.map (Some >> DateTime)
                Gen.constant (DateTime None)
                genTimeSpan |> Gen.map (Some >> Time)
                Gen.constant (Time None)
                genDateTimeOffset |> Gen.map (Some >> CreatedTime)
                Gen.constant (CreatedTime None)
                genDateTimeOffset |> Gen.map (Some >> UpdatedTime)
                Gen.constant (UpdatedTime None)

                // Selection fields
                Gen.listOf genNonEmptyString |> Gen.map CheckBox
                genNonEmptyString |> Gen.map (Some >> RadioButton)
                Gen.constant (RadioButton None)
                genNonEmptyString |> Gen.map (Some >> DropDown)
                Gen.constant (DropDown None)
                Gen.listOf genNonEmptyString |> Gen.map MultiSelect
                Gen.listOf genNonEmptyString |> Gen.map Category

                // User/Group/Org fields
                Gen.listOf genUser |> Gen.map (fun users -> UserSelect users)
                genUser |> Gen.map (fun user -> Creator (Some user))
                Gen.constant (Creator None)
                genUser |> Gen.map (fun user -> Modifier (Some user))
                Gen.constant (Modifier None)
                Gen.listOf genUser |> Gen.map (fun users -> StatusAssignee users)
                Gen.listOf genGroup |> Gen.map (fun groups -> GroupSelect groups)
                Gen.listOf genOrganization |> Gen.map (fun orgs -> OrganizationSelect orgs)

                // File fields
                Gen.listOf genFileBody |> Gen.map File

                // Link field
                genNonEmptyString |> Gen.map (Some >> Link)
                Gen.constant (Link None)

                // Subtable (limited depth to avoid infinite recursion)
                Gen.listOfLength 2 (genTableRow (size - 1)) |> Gen.map Subtable

                // System fields
                genNonEmptyString |> Gen.map (Some >> RecordNumber)
                Gen.constant (RecordNumber None)
                genNonEmptyString |> Gen.map (Some >> Status)
                Gen.constant (Status None)
                Arb.generate<int64> |> Gen.filter (fun x -> x > 0L) |> Gen.map (Some >> Id)
                Gen.constant (Id None)
                Arb.generate<int64> |> Gen.filter (fun x -> x >= 0L) |> Gen.map (Some >> Revision)
                Gen.constant (Revision None)
            ]

    /// Generate Record with random fields
    let genRecord =
        gen {
            let! id = Gen.oneof [Gen.constant None; Arb.generate<int64> |> Gen.filter (fun x -> x > 0L) |> Gen.map Some]
            let! revision = Gen.oneof [Gen.constant None; Arb.generate<int64> |> Gen.filter (fun x -> x >= 0L) |> Gen.map Some]
            let! fieldCount = Gen.choose(0, 10)
            let! fields =
                Gen.listOfLength fieldCount (gen {
                    let! code = genFieldCode
                    let! value = genFieldValue 2  // Use size of 2 for field values
                    return (code, value)
                })
                |> Gen.map (fun pairs ->
                    // Ensure unique field codes
                    pairs
                    |> List.distinctBy fst
                    |> Map.ofList)
            return {
                Id = id
                Revision = revision
                Fields = fields
            }
        }

    /// Generate UpdateKey
    let genUpdateKey =
        gen {
            let! field = genFieldCode
            let! value = genNonEmptyString
            return { Field = field; Value = value }
        }

    /// Generate RecordRevision
    let genRecordRevision =
        gen {
            let! recordId = Arb.generate<int64> |> Gen.filter (fun x -> x > 0L)
            let! revision = Gen.oneof [Gen.constant None; Arb.generate<int64> |> Gen.filter (fun x -> x >= 0L) |> Gen.map Some]
            return { RecordId = recordId; Revision = revision }
        }

    /// Register all custom generators
    type CustomGenerators =
        static member User() = Arb.fromGen genUser
        static member Group() = Arb.fromGen genGroup
        static member Organization() = Arb.fromGen genOrganization
        static member FileBody() = Arb.fromGen genFileBody
        static member TableRow() = Arb.fromGen (Gen.sized genTableRow)
        static member FieldValue() = Arb.fromGen (Gen.sized genFieldValue)
        static member Record() = Arb.fromGen genRecord
        static member UpdateKey() = Arb.fromGen genUpdateKey
        static member RecordRevision() = Arb.fromGen genRecordRevision
        static member DateTime() = Arb.fromGen genDateTime
        static member DateTimeOffset() = Arb.fromGen genDateTimeOffset
        static member TimeSpan() = Arb.fromGen genTimeSpan
        static member Decimal() = Arb.fromGen genDecimal
