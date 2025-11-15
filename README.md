# Kintone F# Client

API client library for Kintone REST APIs on F#.

## Installation

Add the package reference to your `.fsproj` file:

```xml
<ItemGroup>
  <PackageReference Include="KintoneFSharpClient" Version="3.0.0" />
</ItemGroup>
```

Or using the .NET CLI:

```bash
dotnet add package KintoneFSharpClient
```

## Compatibility

.NET 8.0 and later versions are supported.

## Basic Usage

Here are some short examples of this library.

### Initialization

```fsharp
open Kintone.Client
open Kintone.Client.Model

// Creates a client that uses Password authentication
let client =
    KintoneClientBuilder
        .Create("https://{your Kintone domain}.cybozu.com")
        .AuthByPassword("username", "password")
        .Build()

// Or using API token authentication
let clientWithToken =
    KintoneClientBuilder
        .Create("https://{your Kintone domain}.cybozu.com")
        .AuthByApiToken("your-api-token")
        .Build()

// Shorthand for default clients
let defaultClient =
    KintoneClientBuilder.DefaultClient("https://{your Kintone domain}.cybozu.com", "username", "password")

let defaultClientWithToken =
    KintoneClientBuilder.DefaultClientWithToken("https://{your Kintone domain}.cybozu.com", "your-api-token")
```

### Getting Records

```fsharp
// Get a single record
let record = client.Record.GetRecord(1L, 100L)

// Get a field value
let textValue = Record.getSingleLineText "field1" record

// Get records with a query
let records = client.Record.GetRecords(1L, query = "number > 100")

// Get records with specific fields
let recordsWithFields =
    client.Record.GetRecords(1L, fields = ["field1"; "field2"], query = "number > 100")

// Get records with total count
let (records, totalCount) =
    client.Record.GetRecordsWithTotalCount(1L, query = "number > 100")
```

### Adding a Record

```fsharp
open Kintone.Client.Model

// Create a new record
let newRecord =
    Record.empty
    |> Record.putField "field1" (SingleLineText (Some "Hello World"))
    |> Record.putField "field2" (Number (Some 42M))

// Add the record
let recordId = client.Record.AddRecord(1L, newRecord)

// Add multiple records
let records = [newRecord; newRecord]
let recordIds = client.Record.AddRecords(1L, records)
```

### Updating Records

```fsharp
// Update a record
let updateFields =
    Map.empty
    |> Map.add "field1" (SingleLineText (Some "Updated Value"))
    |> Map.add "field2" (Number (Some 100M))

let revision = client.Record.UpdateRecord(1L, 100L, updateFields)

// Update with revision check
let revisionWithCheck =
    client.Record.UpdateRecord(1L, 100L, updateFields, revision = 5L)
```

### Deleting Records

```fsharp
// Delete records by IDs
client.Record.DeleteRecords(1L, [100L; 101L; 102L])

// Delete with revision check
client.Record.DeleteRecords(1L, [100L; 101L], revisions = [5L; 6L])
```

### Working with Cursors (for large datasets)

```fsharp
// Create a cursor
let (cursorId, totalCount) =
    client.Record.CreateCursor(1L, query = "number > 100", size = 500)

// Get records using cursor
let rec getAllRecords cursorId acc =
    let (records, hasNext) = client.Record.GetRecordsByCursor(cursorId)
    let newAcc = acc @ records
    if hasNext then
        getAllRecords cursorId newAcc
    else
        client.Record.DeleteCursor(cursorId)
        newAcc

let allRecords = getAllRecords cursorId []
```

## Keeping Up With The OpenAPI Spec

The upstream kintone REST API exposes an official OpenAPI specification. This repository now includes a helper tool that snapshots the spec and generates a simple summary module so that we can see what changed.

```
dotnet run --project tools/SpecSync
```

The command downloads the latest timestamp from [`kintone/rest-api-spec`](https://github.com/kintone/rest-api-spec), saves it under `openapi/kintone/<version>/`, refreshes `src/Generated/RestApiSpec.fs`, and updates `openapi/operation-coverage.json` (where each `operationId` is tagged as `implemented`, `not-implemented`, `planned`, or `deprecated`). See `docs/spec-sync.md` for the detailed workflow.

`dotnet test` includes `OperationCoverageTests`, which fails if we forget to classify a new operation or if the coverage file drifts from the bundled spec.

There is also a scheduled "Spec Watch" workflow that checks upstream once per day and files an Issue whenever a newer spec timestamp appears, so we get a reminder even if nobody runs the sync locally.

### App Settings

```fsharp
// Get general information of an App
let app = client.App.GetApp(1L)

// Create a new App
let appId = client.App.AddApp("New App")

// Get form fields
let fields = client.App.GetFormFields(appId)

// Add form fields
let newFields =
    Map.empty
    |> Map.add "number" (NumberProperty {
        Base = { Code = "number"; Label = "Number"; NoLabel = None }
        Required = Some true
        Unique = None
        MinValue = None
        MaxValue = None
        DefaultValue = None
        Digit = Some true
        DisplayScale = Some 0
        Unit = None
        UnitPosition = None
    })

let revision = client.App.AddFormFields(appId, newFields)

// Deploy an App
client.App.DeployApp(appId)

// Check deployment status
let statuses = client.App.GetDeployStatus([appId])
```

### Working with Comments

```fsharp
// Add a comment to a record
let comment = {
    Text = "This is a comment"
    Mentions = None
}
let commentId = client.Record.AddRecordComment(1L, 100L, comment)

// Get comments
let comments = client.Record.GetRecordComments(1L, 100L)

// Delete a comment
client.Record.DeleteRecordComment(1L, 100L, commentId)
```

### File Operations

```fsharp
// Upload a file
let fileData = System.IO.File.ReadAllBytes("path/to/file.pdf")
let fileKey = client.File.UploadFile("file.pdf", "application/pdf", fileData)

// Download a file
let (downloadedData, contentType) = client.File.DownloadFile(fileKey)

// Use file in a record
let recordWithFile =
    Record.empty
    |> Record.putField "attachmentField" (File [{
        ContentType = Some contentType
        FileKey = fileKey
        Name = "file.pdf"
        Size = Some (int64 fileData.Length)
    }])
```

### Space Operations

```fsharp
// Get space information
let space = client.Space.GetSpace(10L)

// Get space members
let members = client.Space.GetSpaceMembers(10L)

// Add a thread to a space
let threadId = client.Space.AddThread(10L, "New Thread", body = "Thread body")
```

### Cleanup

The client holds some resources inside it.
Be sure to release them by calling `Dispose()` or using `use` keyword:

```fsharp
// Using 'use' keyword (recommended)
use client =
    KintoneClientBuilder
        .Create("https://{your Kintone domain}.cybozu.com")
        .AuthByPassword("username", "password")
        .Build()

// Perform operations
let records = client.Record.GetRecords(1L)

// Client is automatically disposed when out of scope

// Or manually dispose
client.Dispose()
```

### Guest Space Operations

To operate with a Guest Space, set the Guest Space ID when building the client:

```fsharp
let guestClient =
    KintoneClientBuilder
        .Create("https://{your Kintone domain}.cybozu.com")
        .AuthByPassword("username", "password")
        .SetGuestSpaceId(100L)
        .Build()
```

### Error Handling

The client throws exceptions when API calls fail. Use try-catch or Result types:

```fsharp
try
    let record = client.Record.GetRecord(1L, 100L)
    printfn "Record retrieved successfully"
with
| ex -> printfn "Error: %s" ex.Message
```

## Building

This library is built using .NET SDK.

Run the following command to build the library:

```bash
dotnet build
```

To run tests:

```bash
dotnet test
```

To create a NuGet package:

```bash
dotnet pack
```

## Features

### Implemented Features

- ✅ Password authentication
- ✅ API token authentication
- ✅ Record operations (CRUD)
- ✅ Cursor operations for large datasets
- ✅ Record comments
- ✅ App management
- ✅ Form fields management
- ✅ Views management
- ✅ App settings
- ✅ Access control (ACL)
- ✅ Space operations
- ✅ File upload/download
- ✅ Plugin management
- ✅ Guest Space support
- ✅ Basic authentication support
- ✅ Custom user agent support

### F# Specific Features

- **Immutable Records**: All model types use F# immutable records
- **Discriminated Unions**: Field types and other enums are represented as discriminated unions
- **Option Types**: Nullable fields use F# Option types instead of null
- **Type Safety**: Strong typing throughout the API
- **Functional API**: Pipeline-friendly functions for record manipulation

## Migration from Java Client

This F# client provides a similar API to the Java client but with F# idioms:

| Java | F# |
|------|-----|
| `new Record()` | `Record.empty` |
| `record.putField(...)` | `Record.putField ... record` |
| `record.getField(...)` | `Record.getField ... record` |
| `Optional<T>` | `Option<'T>` |
| `null` | `None` |
| `List<T>` | `'T list` |
| `Map<K,V>` | `Map<'K,'V>` |

## Contribution Guide

- [CONTRIBUTING.md](CONTRIBUTING.md)

## License

- [MIT License](LICENSE)

## Author

Cybozu, Inc.

## Contributors

- AaronJRubin [@AaronJRubin](https://github.com/AaronJRubin)
- chikamura [@chikamura](https://github.com/chikamura)
