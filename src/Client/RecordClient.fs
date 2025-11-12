namespace Kintone.Client

open System
open Kintone.Client.Model
open Kintone.Client.Api

/// Record client for record operations
type RecordClient(internalClient: InternalClient) =

    /// Add a single record to an app
    member this.AddRecord(app: int64, record: Record) : int64 =
        let request = {
            App = app
            Record = record
        }
        let response = internalClient.Call<AddRecordRequest, AddRecordResponseBody>(HttpMethod.Post, KintoneApi.AddRecord, Some request)
        response.Id

    /// Add multiple records to an app
    member this.AddRecords(app: int64, records: Record list) : int64 list =
        let request = {
            App = app
            Records = records
        }
        let response = internalClient.Call<AddRecordsRequest, AddRecordsResponseBody>(HttpMethod.Post, KintoneApi.AddRecords, Some request)
        response.Ids

    /// Get a single record by ID
    member this.GetRecord(app: int64, id: int64) : Record =
        let request = {
            App = app
            Id = id
            Fields = None
        }
        let response = internalClient.Call<GetRecordRequest, GetRecordResponseBody>(HttpMethod.Get, KintoneApi.GetRecord, Some request)
        response.Record

    /// Get a single record by ID with specific fields
    member this.GetRecordWithFields(app: int64, id: int64, fields: string list) : Record =
        let request = {
            App = app
            Id = id
            Fields = Some fields
        }
        let response = internalClient.Call<GetRecordRequest, GetRecordResponseBody>(HttpMethod.Get, KintoneApi.GetRecord, Some request)
        response.Record

    /// Get records with a query
    member this.GetRecords(app: int64, ?query: string, ?fields: string list, ?totalCount: bool) : Record list =
        let request = {
            App = app
            Fields = fields
            Query = query
            TotalCount = totalCount
        }
        let response = internalClient.Call<GetRecordsRequest, GetRecordsResponseBody>(HttpMethod.Get, KintoneApi.GetRecords, Some request)
        response.Records

    /// Get records with total count
    member this.GetRecordsWithTotalCount(app: int64, ?query: string, ?fields: string list) : Record list * int64 =
        let request = {
            App = app
            Fields = fields
            Query = query
            TotalCount = Some true
        }
        let response = internalClient.Call<GetRecordsRequest, GetRecordsResponseBody>(HttpMethod.Get, KintoneApi.GetRecords, Some request)
        (response.Records, response.TotalCount |> Option.defaultValue 0L)

    /// Update a single record
    member this.UpdateRecord(app: int64, id: int64, record: Map<string, FieldValue>, ?revision: int64) : int64 =
        let request = {
            App = app
            Id = Some id
            UpdateKey = None
            Record = record
            Revision = revision
        }
        let response = internalClient.Call<UpdateRecordRequest, UpdateRecordResponseBody>(HttpMethod.Put, KintoneApi.UpdateRecord, Some request)
        response.Revision

    /// Update a single record by update key
    member this.UpdateRecordByKey(app: int64, updateKey: UpdateKey, record: Map<string, FieldValue>, ?revision: int64) : int64 =
        let request = {
            App = app
            Id = None
            UpdateKey = Some updateKey
            Record = record
            Revision = revision
        }
        let response = internalClient.Call<UpdateRecordRequest, UpdateRecordResponseBody>(HttpMethod.Put, KintoneApi.UpdateRecord, Some request)
        response.Revision

    /// Update multiple records
    member this.UpdateRecords(app: int64, records: RecordForUpdate list) : RecordRevision list =
        let request = {
            App = app
            Records = records
        }
        let response = internalClient.Call<UpdateRecordsRequest, UpdateRecordsResponseBody>(HttpMethod.Put, KintoneApi.UpdateRecords, Some request)
        response.Records

    /// Delete records by IDs
    member this.DeleteRecords(app: int64, ids: int64 list, ?revisions: int64 list) : unit =
        let request = {
            App = app
            Ids = ids
            Revisions = revisions
        }
        let _ = internalClient.Call<DeleteRecordsRequest, DeleteRecordsResponseBody>(HttpMethod.Delete, KintoneApi.DeleteRecords, Some request)
        ()

    /// Create a cursor for large record sets
    member this.CreateCursor(app: int64, ?fields: string list, ?query: string, ?size: int) : string * int64 =
        let request = {
            App = app
            Fields = fields
            Query = query
            Size = size
        }
        let response = internalClient.Call<CreateCursorRequest, CreateCursorResponseBody>(HttpMethod.Post, KintoneApi.CreateCursor, Some request)
        (response.Id, response.TotalCount)

    /// Get records by cursor
    member this.GetRecordsByCursor(cursorId: string) : Record list * bool =
        let request = {
            Id = cursorId
        }
        let response = internalClient.Call<GetRecordsByCursorRequest, GetRecordsByCursorResponseBody>(HttpMethod.Get, KintoneApi.GetRecordsByCursor, Some request)
        (response.Records, response.Next)

    /// Delete a cursor
    member this.DeleteCursor(cursorId: string) : unit =
        let request = {
            Id = cursorId
        }
        let _ = internalClient.Call<DeleteCursorRequest, DeleteCursorResponseBody>(HttpMethod.Delete, KintoneApi.DeleteCursor, Some request)
        ()

    /// Add a comment to a record
    member this.AddRecordComment(app: int64, recordId: int64, comment: RecordComment) : int64 =
        let request = {
            App = app
            Record = recordId
            Comment = comment
        }
        let response = internalClient.Call<AddRecordCommentRequest, AddRecordCommentResponseBody>(HttpMethod.Post, KintoneApi.AddRecordComment, Some request)
        response.Id

    /// Get comments for a record
    member this.GetRecordComments(app: int64, recordId: int64, ?order: Order, ?offset: int, ?limit: int) : PostedRecordComment list =
        let request = {
            App = app
            Record = recordId
            Order = order
            Offset = offset
            Limit = limit
        }
        let response = internalClient.Call<GetRecordCommentsRequest, GetRecordCommentsResponseBody>(HttpMethod.Get, KintoneApi.GetRecordComments, Some request)
        response.Comments

    /// Delete a comment from a record
    member this.DeleteRecordComment(app: int64, recordId: int64, commentId: int64) : unit =
        let request = {
            App = app
            Record = recordId
            Comment = commentId
        }
        let _ = internalClient.Call<DeleteRecordCommentRequest, DeleteRecordCommentResponseBody>(HttpMethod.Delete, KintoneApi.DeleteRecordComment, Some request)
        ()

    /// Update record assignees
    member this.UpdateRecordAssignees(app: int64, id: int64, assignees: string list, ?revision: int64) : int64 =
        let request = {
            App = app
            Id = id
            Assignees = assignees
            Revision = revision
        }
        let response = internalClient.Call<UpdateRecordAssigneesRequest, UpdateRecordAssigneesResponseBody>(HttpMethod.Put, KintoneApi.UpdateRecordAssignees, Some request)
        response.Revision

    /// Update record status
    member this.UpdateRecordStatus(app: int64, id: int64, action: string, ?assignee: string, ?revision: int64) : int64 =
        let request = {
            App = app
            Id = id
            Action = action
            Assignee = assignee
            Revision = revision
        }
        let response = internalClient.Call<UpdateRecordStatusRequest, UpdateRecordStatusResponseBody>(HttpMethod.Put, KintoneApi.UpdateRecordStatus, Some request)
        response.Revision

    /// Update record statuses in bulk
    member this.UpdateRecordStatuses(app: int64, records: RecordStatusUpdate list) : RecordRevision list =
        let request = {
            App = app
            Records = records
        }
        let response = internalClient.Call<UpdateRecordStatusesRequest, UpdateRecordStatusesResponseBody>(HttpMethod.Put, KintoneApi.UpdateRecordStatuses, Some request)
        response.Records
