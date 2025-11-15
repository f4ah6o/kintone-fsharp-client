module SpecSync

open System
open System.IO
open System.Net.Http
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open System.Threading.Tasks
open YamlDotNet.RepresentationModel

let utf8NoBom = new UTF8Encoding(false)

let dropBom (content: string) =
    if String.IsNullOrEmpty(content) then
        content
    elif content.[0] = '\uFEFF' then
        content.Substring(1)
    else
        content

let usage () =
    """
SpecSync - Sync Kintone OpenAPI spec and generate helper module.

Usage:
  dotnet run --project tools/SpecSync [--version 20240101000000]
"""

let failWithUsage message =
    eprintfn "%s" message
    eprintfn "%s" (usage ())
    Environment.Exit(1)
    Unchecked.defaultof<_>

let parseArgs (args: string array) =
    let rec loop idx selected =
        if idx >= args.Length then
            selected
        else
            match args.[idx] with
            | "--version" | "-v" ->
                if idx + 1 >= args.Length then
                    failWithUsage "Missing value for --version"
                else
                    loop (idx + 2) (Some args.[idx + 1])
            | "--help" | "-h" ->
                printfn "%s" (usage ())
                Environment.Exit(0)
                selected
            | unknown ->
                failWithUsage (sprintf "Unknown argument: %s" unknown)
    loop 0 None

let httpClient =
    let client = new HttpClient()
    client.DefaultRequestHeaders.UserAgent.ParseAdd("kintone-fsharp-client-spec-sync/1.0")
    client

let fetchAvailableVersions () = task {
    use! stream = httpClient.GetStreamAsync("https://api.github.com/repos/kintone/rest-api-spec/contents/kintone")
    use doc = JsonDocument.Parse(stream)
    let versions =
        doc.RootElement.EnumerateArray()
        |> Seq.choose (fun element ->
            if element.GetProperty("type").GetString() = "dir" then
                element.GetProperty("name").GetString() |> Option.ofObj
            else
                None)
        |> Seq.toList
    return versions
}

let fetchLatestVersion () = task {
    let! versions = fetchAvailableVersions ()
    match versions with
    | [] -> return failwith "No versions found in upstream repository"
    | list -> return (list |> List.max)
}

let downloadSpecFile version relativePath = task {
    let url = sprintf "https://raw.githubusercontent.com/kintone/rest-api-spec/main/kintone/%s/%s" version relativePath
    return! httpClient.GetStringAsync(url)
}

type Operation =
    { OperationId: string
      Method: string
      Path: string
      Summary: string option }

type OperationCoverageEntry =
    { OperationId: string
      Method: string
      Path: string
      Status: string
      Notes: string option }

let tryGetScalar (mapping: YamlMappingNode) (key: string) =
    mapping.Children
    |> Seq.tryPick (fun kvp ->
        match kvp.Key, kvp.Value with
        | :? YamlScalarNode as scalarKey, (:? YamlScalarNode as scalarValue) when scalarKey.Value = key ->
            scalarValue.Value |> Option.ofObj
        | _ -> None)

let tryGetMapping (mapping: YamlMappingNode) (key: string) =
    mapping.Children
    |> Seq.tryPick (fun kvp ->
        match kvp.Key, kvp.Value with
        | :? YamlScalarNode as scalarKey, (:? YamlMappingNode as mappingValue) when scalarKey.Value = key ->
            Some mappingValue
        | _ -> None)

let tryGetScalarValue (node: YamlNode) =
    match node with
    | :? YamlScalarNode as scalar -> scalar.Value |> Option.ofObj
    | _ -> None

let toOperations (yamlContent: string) =
    use reader = new StringReader(yamlContent)
    let yaml = YamlStream()
    yaml.Load(reader)
    let root = yaml.Documents.[0].RootNode :?> YamlMappingNode
    let httpMethods = set [ "get"; "put"; "post"; "delete"; "patch" ]
    match tryGetMapping root "paths" with
    | None -> []
    | Some pathsMapping ->
        [ for pathEntry in pathsMapping.Children do
              match tryGetScalarValue pathEntry.Key, pathEntry.Value with
              | Some path, (:? YamlMappingNode as methodMapping) ->
                  for methodEntry in methodMapping.Children do
                      match tryGetScalarValue methodEntry.Key with
                      | Some methodName when httpMethods.Contains(methodName.ToLowerInvariant()) ->
                          match methodEntry.Value with
                          | :? YamlMappingNode as opNode ->
                              match tryGetScalar opNode "operationId" with
                              | Some opId when not (String.IsNullOrWhiteSpace(opId)) ->
                                  let summary = tryGetScalar opNode "summary"
                                  yield
                                      { OperationId = opId
                                        Method = methodName.ToUpperInvariant()
                                        Path = path
                                        Summary = summary }
                              | _ -> ()
                          | _ -> ()
                      | _ -> ()
              | _ -> () ]
        |> List.sortBy (fun op -> op.Path, op.Method)

let escapeString (value: string) =
    value
        .Replace("\\", "\\\\")
        .Replace("\"", "\\\"")
        .Replace("\r", "\\r")
        .Replace("\n", "\\n")
        .Replace("\t", "\\t")

let generateModule version operations =
    let builder = StringBuilder()
    builder.AppendLine("// <auto-generated>" ) |> ignore
    builder.AppendLine("// This file is generated by tools/SpecSync.") |> ignore
    builder.AppendLine("// Do not edit manually." ) |> ignore
    builder.AppendLine("namespace Kintone.Client.Generated") |> ignore
    builder.AppendLine() |> ignore
    builder.AppendLine("module RestApiSpec =") |> ignore
    builder.AppendLine("    [<Literal>]" ) |> ignore
    builder.AppendLine(sprintf "    let SpecVersion = \"%s\"" version) |> ignore
    builder.AppendLine() |> ignore
    builder.AppendLine("    type Operation =") |> ignore
    builder.AppendLine("        { OperationId: string" ) |> ignore
    builder.AppendLine("          Method: string" ) |> ignore
    builder.AppendLine("          Path: string" ) |> ignore
    builder.AppendLine("          Summary: string option }" ) |> ignore
    builder.AppendLine() |> ignore
    builder.AppendLine("    let AllOperations: Operation list = [") |> ignore
    operations
    |> List.iter (fun op ->
        let summaryValue =
            match op.Summary with
            | Some s -> sprintf "Some \"%s\"" (escapeString s)
            | None -> "None"
        builder.AppendLine(
            sprintf "        { OperationId = \"%s\"; Method = \"%s\"; Path = \"%s\"; Summary = %s }"
                (escapeString op.OperationId)
                (escapeString op.Method)
                (escapeString op.Path)
                summaryValue)
        |> ignore)
    builder.AppendLine("    ]") |> ignore
    builder.ToString()

let jsonOptions =
    let options = JsonSerializerOptions(WriteIndented = true)
    options.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    options

let tryReadCoverage path =
    if File.Exists(path) then
        let json = File.ReadAllText(path)
        JsonSerializer.Deserialize<OperationCoverageEntry list>(json, jsonOptions)
        |> Option.ofObj
        |> Option.defaultValue []
    else
        []

let mergeCoverage (operations: Operation list) (entries: OperationCoverageEntry list) =
    let existing =
        entries
        |> List.map (fun entry -> entry.OperationId, entry)
        |> Map.ofList
    operations
    |> List.map (fun op ->
        match Map.tryFind op.OperationId existing with
        | Some entry ->
            { entry with
                Method = op.Method
                Path = op.Path }
        | None ->
            { OperationId = op.OperationId
              Method = op.Method
              Path = op.Path
              Status = "unknown"
              Notes = None })
    |> List.sortBy (fun entry -> entry.OperationId)

[<EntryPoint>]
let main argv =
    let argsVersion = parseArgs argv
    let work = task {
        let! version =
            match argsVersion with
            | Some v -> Task.FromResult v
            | None -> fetchLatestVersion ()
        printfn "Using spec version %s" version
        let! rootSpecRaw = downloadSpecFile version "openapi.yaml"
        let! bundledSpecRaw = downloadSpecFile version "bundled/openapi.yaml"
        let rootSpec = dropBom rootSpecRaw
        let bundledSpec = dropBom bundledSpecRaw
        let operations = toOperations bundledSpec
        printfn "Extracted %d operations" operations.Length
        let repoRoot = Directory.GetCurrentDirectory()
        let specDir = Path.Combine(repoRoot, "openapi", "kintone", version)
        Directory.CreateDirectory(specDir) |> ignore
        let specPath = Path.Combine(specDir, "openapi.yaml")
        File.WriteAllText(specPath, rootSpec, utf8NoBom)
        let bundledDir = Path.Combine(specDir, "bundled")
        Directory.CreateDirectory(bundledDir) |> ignore
        let bundledPath = Path.Combine(bundledDir, "openapi.yaml")
        File.WriteAllText(bundledPath, bundledSpec, utf8NoBom)
        File.WriteAllText(Path.Combine(repoRoot, "openapi", "kintone", "version.txt"), version, utf8NoBom)
        let generated = generateModule version operations
        let generatedPath = Path.Combine(repoRoot, "src", "Generated", "RestApiSpec.fs")
        File.WriteAllText(generatedPath, generated, utf8NoBom)
        printfn "Updated %s" (Path.GetRelativePath(repoRoot, generatedPath))
        let coveragePath = Path.Combine(repoRoot, "openapi", "operation-coverage.json")
        let coverageEntries = tryReadCoverage coveragePath
        let mergedCoverage = mergeCoverage operations coverageEntries
        let coverageJson = JsonSerializer.Serialize(mergedCoverage, jsonOptions)
        File.WriteAllText(coveragePath, coverageJson, utf8NoBom)
        printfn "Updated %s" (Path.GetRelativePath(repoRoot, coveragePath))
        printfn "Stored spec at %s (root)" (Path.GetRelativePath(repoRoot, specPath))
        printfn "Stored spec at %s (bundled)" (Path.GetRelativePath(repoRoot, bundledPath))
        return 0
    }
    try
        work.GetAwaiter().GetResult()
    with ex ->
        eprintfn "Spec sync failed: %s" ex.Message
        1
