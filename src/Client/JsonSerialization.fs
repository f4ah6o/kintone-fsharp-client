namespace Kintone.Client

open System
open System.Text.Json
open System.Text.Json.Serialization
open Kintone.Client.Model
open Kintone.Client.Api

/// JSON serialization utilities
module JsonSerialization =

    /// JSON serializer options
    let private options =
        let opts = JsonSerializerOptions()
        opts.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
        opts.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
        opts.Converters.Add(JsonFSharpConverter())
        opts

    /// Serialize object to JSON string
    let serialize<'T> (value: 'T) : string =
        JsonSerializer.Serialize(value, options)

    /// Deserialize JSON string to object
    let deserialize<'T> (json: string) : 'T =
        JsonSerializer.Deserialize<'T>(json, options)

    /// Serialize record to JSON
    let serializeRecord (record: Record) : string =
        // Custom serialization for Record fields
        let fieldsJson =
            record.Fields
            |> Map.toList
            |> List.map (fun (fieldCode, fieldValue) ->
                let valueJson =
                    match fieldValue with
                    | SingleLineText (Some v) -> sprintf """{"value":"%s"}""" v
                    | SingleLineText None -> """{"value":null}"""
                    | MultiLineText (Some v) -> sprintf """{"value":"%s"}""" v
                    | MultiLineText None -> """{"value":null}"""
                    | Number (Some v) -> sprintf """{"value":%M}""" v
                    | Number None -> """{"value":null}"""
                    | Date (Some v) -> sprintf """{"value":"%s"}""" (v.ToString("yyyy-MM-dd"))
                    | Date None -> """{"value":null}"""
                    | DateTime (Some v) -> sprintf """{"value":"%s"}""" (v.ToString("yyyy-MM-ddTHH:mm:sszzz"))
                    | DateTime None -> """{"value":null}"""
                    | CheckBox values -> sprintf """{"value":%s}""" (serialize values)
                    | UserSelect users -> sprintf """{"value":%s}""" (serialize users)
                    | _ -> """{"value":null}"""
                sprintf """"%s":%s""" fieldCode valueJson
            )
            |> String.concat ","

        sprintf """{%s}""" fieldsJson

    /// Deserialize record from JSON
    let deserializeRecord (json: string) : Record =
        // Simplified deserialization - in production, would need proper field type handling
        let doc = JsonDocument.Parse(json)
        let root = doc.RootElement

        let fields =
            root.EnumerateObject()
            |> Seq.map (fun prop ->
                let fieldCode = prop.Name
                let fieldValue =
                    if prop.Value.TryGetProperty("value", &_) then
                        let value = prop.Value.GetProperty("value")
                        match prop.Value.TryGetProperty("type", &_) with
                        | true ->
                            let fieldType = prop.Value.GetProperty("type").GetString()
                            match fieldType with
                            | "SINGLE_LINE_TEXT" ->
                                if value.ValueKind = JsonValueKind.String then
                                    SingleLineText (Some (value.GetString()))
                                else
                                    SingleLineText None
                            | "NUMBER" ->
                                if value.ValueKind = JsonValueKind.Number then
                                    Number (Some (value.GetDecimal()))
                                elif value.ValueKind = JsonValueKind.String then
                                    Number (Some (Decimal.Parse(value.GetString())))
                                else
                                    Number None
                            | _ -> SingleLineText None
                        | false ->
                            // Infer type from value
                            if value.ValueKind = JsonValueKind.String then
                                SingleLineText (Some (value.GetString()))
                            elif value.ValueKind = JsonValueKind.Number then
                                Number (Some (value.GetDecimal()))
                            else
                                SingleLineText None
                    else
                        SingleLineText None
                (fieldCode, fieldValue)
            )
            |> Map.ofSeq

        let id =
            if root.TryGetProperty("$id", &_) then
                let idProp = root.GetProperty("$id")
                if idProp.TryGetProperty("value", &_) then
                    Some (idProp.GetProperty("value").GetInt64())
                else
                    None
            else
                None

        let revision =
            if root.TryGetProperty("$revision", &_) then
                let revProp = root.GetProperty("$revision")
                if revProp.TryGetProperty("value", &_) then
                    Some (revProp.GetProperty("value").GetInt64())
                else
                    None
            else
                None

        {
            Id = id
            Revision = revision
            Fields = fields
        }

/// Custom JSON converter for F# discriminated unions
and JsonFSharpConverter() =
    inherit JsonConverterFactory()

    override _.CanConvert(typeToConvert: Type) =
        // This is a simplified implementation
        // In production, would need proper F# type detection
        false

    override _.CreateConverter(typeToConvert: Type, options: JsonSerializerOptions) =
        null
