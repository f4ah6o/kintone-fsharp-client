namespace Kintone.Client.Tests

open System
open System.IO
open System.Text.Json
open Xunit
open Kintone.Client.Generated

module OperationCoverageTests =

    type CoverageEntry =
        { OperationId: string
          Status: string }

    module private OperationCoverage =
        let load () =
            let path =
                Path.Combine(__SOURCE_DIRECTORY__, "..", "openapi", "operation-coverage.json")
                |> Path.GetFullPath
            if not (File.Exists(path)) then
                failwithf "Coverage file is missing: %s" path
            use document = JsonDocument.Parse(File.ReadAllText(path))
            document.RootElement.EnumerateArray()
            |> Seq.map (fun element ->
                let getString (propertyName: string) =
                    let mutable prop = Unchecked.defaultof<JsonElement>
                    if element.TryGetProperty(propertyName, &prop) then
                        prop.GetString() |> Option.ofObj |> Option.defaultValue ""
                    else
                        ""
                { OperationId = getString "operationId"
                  Status = getString "status" })
            |> Seq.toList

    [<Fact>]
    let ``spec operations are all categorized`` () =
        let coverage = OperationCoverage.load ()
        if List.isEmpty coverage then
            failwith "Coverage entries are missing"
        let blankOperationIds =
            coverage
            |> List.filter (fun entry -> String.IsNullOrWhiteSpace entry.OperationId)
        Assert.True(List.isEmpty blankOperationIds, "Coverage entries contain blank operationIds")
        let coverageIds =
            coverage
            |> List.map (fun entry -> entry.OperationId)
            |> Set.ofList

        let specOperations = RestApiSpec.AllOperations
        let specIds = specOperations |> List.map (fun op -> op.OperationId)
        let missingIds =
            specIds
            |> List.filter (fun opId -> Set.contains opId coverageIds |> not)
        let missingMessage =
            missingIds
            |> String.concat ", "
        Assert.True(List.isEmpty missingIds, "Missing coverage entries: " + missingMessage)

        let extra =
            coverage
            |> List.map (fun entry -> entry.OperationId)
            |> List.filter (fun opId -> specIds |> List.contains opId |> not)
        let extraMessage =
            extra
            |> String.concat ", "
        Assert.True(List.isEmpty extra, "Coverage entries for removed operations: " + extraMessage)

        let allowedStatuses =
            set [ "implemented"; "not-implemented"; "planned"; "deprecated" ]

        let invalidStatuses =
            coverage
            |> List.filter (fun entry -> not (allowedStatuses.Contains entry.Status))
        let invalidMessage =
            invalidStatuses
            |> List.map (fun entry -> sprintf "%s=%s" entry.OperationId entry.Status)
            |> String.concat ", "
        Assert.True(List.isEmpty invalidStatuses, "Invalid statuses detected: " + invalidMessage)
