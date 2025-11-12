namespace Kintone.Client.Api

open System
open Kintone.Client.Model

/// Base request interface marker
type IKintoneRequest = interface end

// Record API Requests

/// Get record request
type GetRecordRequest = {
    App: int64
    Id: int64
    Fields: string list option
} with interface IKintoneRequest

/// Get records request
type GetRecordsRequest = {
    App: int64
    Fields: string list option
    Query: string option
    TotalCount: bool option
} with interface IKintoneRequest

/// Add record request
type AddRecordRequest = {
    App: int64
    Record: Record
} with interface IKintoneRequest

/// Add records request
type AddRecordsRequest = {
    App: int64
    Records: Record list
} with interface IKintoneRequest

/// Update record request
type UpdateRecordRequest = {
    App: int64
    Id: int64 option
    UpdateKey: UpdateKey option
    Record: Map<string, FieldValue>
    Revision: int64 option
} with interface IKintoneRequest

/// Update records request
type UpdateRecordsRequest = {
    App: int64
    Records: RecordForUpdate list
} with interface IKintoneRequest

/// Delete records request
type DeleteRecordsRequest = {
    App: int64
    Ids: int64 list
    Revisions: int64 list option
} with interface IKintoneRequest

/// Create cursor request
type CreateCursorRequest = {
    App: int64
    Fields: string list option
    Query: string option
    Size: int option
} with interface IKintoneRequest

/// Get records by cursor request
type GetRecordsByCursorRequest = {
    Id: string
} with interface IKintoneRequest

/// Delete cursor request
type DeleteCursorRequest = {
    Id: string
} with interface IKintoneRequest

/// Add record comment request
type AddRecordCommentRequest = {
    App: int64
    Record: int64
    Comment: RecordComment
} with interface IKintoneRequest

/// Get record comments request
type GetRecordCommentsRequest = {
    App: int64
    Record: int64
    Order: Order option
    Offset: int option
    Limit: int option
} with interface IKintoneRequest

/// Delete record comment request
type DeleteRecordCommentRequest = {
    App: int64
    Record: int64
    Comment: int64
} with interface IKintoneRequest

/// Update record assignees request
type UpdateRecordAssigneesRequest = {
    App: int64
    Id: int64
    Assignees: string list
    Revision: int64 option
} with interface IKintoneRequest

/// Update record status request
type UpdateRecordStatusRequest = {
    App: int64
    Id: int64
    Action: string
    Assignee: string option
    Revision: int64 option
} with interface IKintoneRequest

/// Update record statuses request
type UpdateRecordStatusesRequest = {
    App: int64
    Records: RecordStatusUpdate list
} with interface IKintoneRequest

/// Record status update
and RecordStatusUpdate = {
    Id: int64
    Action: string
    Assignee: string option
    Revision: int64 option
}

// App API Requests

/// Get app request
type GetAppRequest = {
    Id: int64
} with interface IKintoneRequest

/// Get apps request
type GetAppsRequest = {
    Ids: int64 list option
    Codes: string list option
    Name: string option
    SpaceIds: int64 list option
    Limit: int option
    Offset: int option
} with interface IKintoneRequest

/// Add app request
type AddAppRequest = {
    Name: string
    Space: int64 option
    Thread: int64 option
} with interface IKintoneRequest

/// Get form fields request
type GetFormFieldsRequest = {
    App: int64
    Lang: string option
    Preview: bool option
} with interface IKintoneRequest

/// Add form fields request
type AddFormFieldsRequest = {
    App: int64
    Properties: Map<string, FieldProperty>
    Revision: int64 option
} with interface IKintoneRequest

/// Update form fields request
type UpdateFormFieldsRequest = {
    App: int64
    Properties: Map<string, FieldProperty>
    Revision: int64 option
} with interface IKintoneRequest

/// Delete form fields request
type DeleteFormFieldsRequest = {
    App: int64
    Fields: string list
    Revision: int64 option
} with interface IKintoneRequest

/// Get form layout request
type GetFormLayoutRequest = {
    App: int64
    Preview: bool option
} with interface IKintoneRequest

/// Update form layout request
type UpdateFormLayoutRequest = {
    App: int64
    Layout: Layout list
    Revision: int64 option
} with interface IKintoneRequest

/// Get views request
type GetViewsRequest = {
    App: int64
    Lang: string option
    Preview: bool option
} with interface IKintoneRequest

/// Update views request
type UpdateViewsRequest = {
    App: int64
    Views: Map<string, View>
    Revision: int64 option
} with interface IKintoneRequest

/// Get app settings request
type GetAppSettingsRequest = {
    App: int64
    Lang: string option
    Preview: bool option
} with interface IKintoneRequest

/// Update app settings request
type UpdateAppSettingsRequest = {
    App: int64
    Name: string option
    Description: string option
    Icon: AppIcon option
    Theme: string option
    Revision: int64 option
} with interface IKintoneRequest

/// Deploy app request
type DeployAppRequest = {
    Apps: DeployApp list
    Revert: bool option
} with interface IKintoneRequest

/// Deploy app
and DeployApp = {
    App: int64
    Revision: int64 option
}

/// Get deploy status request
type GetDeployStatusRequest = {
    Apps: int64 list
} with interface IKintoneRequest

/// Get app acl request
type GetAppAclRequest = {
    App: int64
    Preview: bool option
} with interface IKintoneRequest

/// Update app acl request
type UpdateAppAclRequest = {
    App: int64
    Rights: AppRightEntity list
    Revision: int64 option
} with interface IKintoneRequest

/// Get record acl request
type GetRecordAclRequest = {
    App: int64
    Lang: string option
    Preview: bool option
} with interface IKintoneRequest

/// Update record acl request
type UpdateRecordAclRequest = {
    App: int64
    Rights: RecordRightEntity list
    Revision: int64 option
} with interface IKintoneRequest

/// Evaluate record acl request
type EvaluateRecordAclRequest = {
    App: int64
    Ids: int64 list
} with interface IKintoneRequest

/// Get field acl request
type GetFieldAclRequest = {
    App: int64
    Preview: bool option
} with interface IKintoneRequest

/// Update field acl request
type UpdateFieldAclRequest = {
    App: int64
    Rights: FieldRight list
    Revision: int64 option
} with interface IKintoneRequest

/// Get app customize request
type GetAppCustomizeRequest = {
    App: int64
    Preview: bool option
} with interface IKintoneRequest

/// Update app customize request
type UpdateAppCustomizeRequest = {
    App: int64
    Scope: CustomizeScope option
    Desktop: CustomizeBody option
    Mobile: CustomizeBody option
    Revision: int64 option
} with interface IKintoneRequest

/// Get process management request
type GetProcessManagementRequest = {
    App: int64
    Lang: string option
    Preview: bool option
} with interface IKintoneRequest

/// Update process management request
type UpdateProcessManagementRequest = {
    App: int64
    Enable: bool option
    States: Map<string, ProcessState> option
    Actions: ProcessAction list option
    Revision: int64 option
} with interface IKintoneRequest

/// Get app plugins request
type GetAppPluginsRequest = {
    App: int64
} with interface IKintoneRequest

/// Add app plugins request
type AddAppPluginsRequest = {
    App: int64
    PluginIds: string list
} with interface IKintoneRequest

// Space API Requests

/// Get space request
type GetSpaceRequest = {
    Id: int64
} with interface IKintoneRequest

/// Add space from template request
type AddSpaceFromTemplateRequest = {
    Id: int64
    Name: string
    Members: SpaceMember list option
    IsPrivate: bool option
    IsGuest: bool option
} with interface IKintoneRequest

/// Update space request
type UpdateSpaceRequest = {
    Id: int64
    Body: string option
    Name: string option
    Attachments: SpaceAttachment list option
} with interface IKintoneRequest

/// Get space members request
type GetSpaceMembersRequest = {
    Id: int64
} with interface IKintoneRequest

/// Update space members request
type UpdateSpaceMembersRequest = {
    Id: int64
    Members: SpaceMember list
} with interface IKintoneRequest

/// Add thread request
type AddThreadRequest = {
    Space: int64
    Name: string
    Body: string option
} with interface IKintoneRequest

/// Update thread request
type UpdateThreadRequest = {
    Id: int64
    Name: string option
    Body: string option
} with interface IKintoneRequest

/// Add guests request
type AddGuestsRequest = {
    Guests: Guest list
} with interface IKintoneRequest

// File API Requests

/// Upload file request
type UploadFileRequest = {
    FileName: string
    ContentType: string
    Data: byte array
} with interface IKintoneRequest

/// Download file request
type DownloadFileRequest = {
    FileKey: string
} with interface IKintoneRequest

// Plugin API Requests

/// Get plugins request
type GetPluginsRequest = {
    Limit: int option
    Offset: int option
} with interface IKintoneRequest

/// Install plugin request
type InstallPluginRequest = {
    PluginZip: byte array
} with interface IKintoneRequest

/// Uninstall plugin request
type UninstallPluginRequest = {
    PluginId: string
} with interface IKintoneRequest

/// Update plugin request
type UpdatePluginRequest = {
    PluginId: string
    PluginZip: byte array
} with interface IKintoneRequest

// Schema API Requests

/// Empty request for APIs that don't need parameters
type EmptyRequest = {
    _dummy: unit option
} with interface IKintoneRequest

// Bulk requests

/// Bulk requests request
type BulkRequestsRequest = {
    Requests: IKintoneRequest list
} with interface IKintoneRequest
