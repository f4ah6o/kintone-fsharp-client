namespace Kintone.Client.Api

open System
open Kintone.Client.Model

/// Base response interface marker
type IKintoneResponseBody = interface end

/// Kintone response wrapper
type KintoneResponse<'T when 'T :> IKintoneResponseBody> = {
    StatusCode: int
    Headers: Map<string, obj>
    Body: 'T option
    ErrorBody: string option
}

// Record API Responses

/// Get record response
type GetRecordResponseBody = {
    Record: Record
} with interface IKintoneResponseBody

/// Get records response
type GetRecordsResponseBody = {
    Records: Record list
    TotalCount: int64 option
} with interface IKintoneResponseBody

/// Add record response
type AddRecordResponseBody = {
    Id: int64
    Revision: int64
} with interface IKintoneResponseBody

/// Add records response
type AddRecordsResponseBody = {
    Ids: int64 list
    Revisions: int64 list
} with interface IKintoneResponseBody

/// Update record response
type UpdateRecordResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Update records response
type UpdateRecordsResponseBody = {
    Records: RecordRevision list
} with interface IKintoneResponseBody

/// Delete records response
type DeleteRecordsResponseBody = {
} with interface IKintoneResponseBody

/// Create cursor response
type CreateCursorResponseBody = {
    Id: string
    TotalCount: int64
} with interface IKintoneResponseBody

/// Get records by cursor response
type GetRecordsByCursorResponseBody = {
    Records: Record list
    Next: bool
} with interface IKintoneResponseBody

/// Delete cursor response
type DeleteCursorResponseBody = {
} with interface IKintoneResponseBody

/// Add record comment response
type AddRecordCommentResponseBody = {
    Id: int64
} with interface IKintoneResponseBody

/// Get record comments response
type GetRecordCommentsResponseBody = {
    Comments: PostedRecordComment list
    Older: bool option
    Newer: bool option
} with interface IKintoneResponseBody

/// Delete record comment response
type DeleteRecordCommentResponseBody = {
} with interface IKintoneResponseBody

/// Update record assignees response
type UpdateRecordAssigneesResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Update record status response
type UpdateRecordStatusResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Update record statuses response
type UpdateRecordStatusesResponseBody = {
    Records: RecordRevision list
} with interface IKintoneResponseBody

// App API Responses

/// Get app response
type GetAppResponseBody = {
    AppId: int64
    Code: string
    Name: string
    Description: string
    SpaceId: int64 option
    ThreadId: int64 option
    CreatedAt: DateTimeOffset
    Creator: User
    ModifiedAt: DateTimeOffset
    Modifier: User
} with interface IKintoneResponseBody

/// Get apps response
type GetAppsResponseBody = {
    Apps: App list
} with interface IKintoneResponseBody

/// Add app response
type AddAppResponseBody = {
    App: int64
    Revision: int64
} with interface IKintoneResponseBody

/// Get form fields response
type GetFormFieldsResponseBody = {
    Properties: Map<string, FieldProperty>
    Revision: int64 option
} with interface IKintoneResponseBody

/// Add form fields response
type AddFormFieldsResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Update form fields response
type UpdateFormFieldsResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Delete form fields response
type DeleteFormFieldsResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Get form layout response
type GetFormLayoutResponseBody = {
    Layout: Layout list
    Revision: int64 option
} with interface IKintoneResponseBody

/// Update form layout response
type UpdateFormLayoutResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Get views response
type GetViewsResponseBody = {
    Views: Map<string, View>
    Revision: int64 option
} with interface IKintoneResponseBody

/// Update views response
type UpdateViewsResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Get app settings response
type GetAppSettingsResponseBody = {
    Name: string
    Description: string
    Icon: AppIcon
    Theme: string
    Revision: int64 option
} with interface IKintoneResponseBody

/// Update app settings response
type UpdateAppSettingsResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Deploy app response
type DeployAppResponseBody = {
} with interface IKintoneResponseBody

/// Get deploy status response
type GetDeployStatusResponseBody = {
    Apps: AppDeployStatus list
} with interface IKintoneResponseBody

/// Get app acl response
type GetAppAclResponseBody = {
    Rights: AppRightEntity list
    Revision: int64 option
} with interface IKintoneResponseBody

/// Update app acl response
type UpdateAppAclResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Get record acl response
type GetRecordAclResponseBody = {
    Rights: RecordRightEntity list
    Revision: int64 option
} with interface IKintoneResponseBody

/// Update record acl response
type UpdateRecordAclResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Evaluate record acl response
type EvaluateRecordAclResponseBody = {
    Rights: Map<int64, EvaluatedRecordRight>
} with interface IKintoneResponseBody

/// Get field acl response
type GetFieldAclResponseBody = {
    Rights: FieldRight list
    Revision: int64 option
} with interface IKintoneResponseBody

/// Update field acl response
type UpdateFieldAclResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Get app customize response
type GetAppCustomizeResponseBody = {
    Scope: CustomizeScope
    Desktop: CustomizeBody
    Mobile: CustomizeBody
    Revision: int64 option
} with interface IKintoneResponseBody

/// Update app customize response
type UpdateAppCustomizeResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Get process management response
type GetProcessManagementResponseBody = {
    Enable: bool
    States: Map<string, ProcessState>
    Actions: ProcessAction list
    Revision: int64 option
} with interface IKintoneResponseBody

/// Update process management response
type UpdateProcessManagementResponseBody = {
    Revision: int64
} with interface IKintoneResponseBody

/// Get app plugins response
type GetAppPluginsResponseBody = {
    Plugins: AppPlugin list
} with interface IKintoneResponseBody

/// Add app plugins response
type AddAppPluginsResponseBody = {
} with interface IKintoneResponseBody

// Space API Responses

/// Get space response
type GetSpaceResponseBody = {
    Id: int64
    Name: string
    DefaultThread: int64
    IsPrivate: bool
    Creator: User
    Modifier: User
    MemberCount: int
    CoverType: string option
    CoverKey: string option
    CoverUrl: string option
    Body: string option
    UseMultiThread: bool option
    IsGuest: bool option
    Attachments: SpaceAttachment list option
} with interface IKintoneResponseBody

/// Add space from template response
type AddSpaceFromTemplateResponseBody = {
    Id: int64
} with interface IKintoneResponseBody

/// Update space response
type UpdateSpaceResponseBody = {
} with interface IKintoneResponseBody

/// Get space members response
type GetSpaceMembersResponseBody = {
    Members: SpaceMember list
} with interface IKintoneResponseBody

/// Update space members response
type UpdateSpaceMembersResponseBody = {
} with interface IKintoneResponseBody

/// Add thread response
type AddThreadResponseBody = {
    Id: int64
} with interface IKintoneResponseBody

/// Update thread response
type UpdateThreadResponseBody = {
} with interface IKintoneResponseBody

/// Add guests response
type AddGuestsResponseBody = {
} with interface IKintoneResponseBody

// File API Responses

/// Upload file response
type UploadFileResponseBody = {
    FileKey: string
} with interface IKintoneResponseBody

/// Download file response
type DownloadFileResponseBody = {
    Data: byte array
    ContentType: string
} with interface IKintoneResponseBody

// Plugin API Responses

/// Get plugins response
type GetPluginsResponseBody = {
    Plugins: Plugin list
} with interface IKintoneResponseBody

/// Install plugin response
type InstallPluginResponseBody = {
    PluginId: string
} with interface IKintoneResponseBody

/// Uninstall plugin response
type UninstallPluginResponseBody = {
} with interface IKintoneResponseBody

/// Update plugin response
type UpdatePluginResponseBody = {
    PluginId: string
} with interface IKintoneResponseBody

// Schema API Responses

/// Get API list response
type GetApiListResponseBody = {
    Apis: Map<string, ApiInfo>
} with interface IKintoneResponseBody

/// API info
and ApiInfo = {
    Link: string
}

/// Get API schema response
type GetApiSchemaResponseBody = {
    Id: string
    BaseUrl: string
} with interface IKintoneResponseBody

// Bulk requests response

/// Bulk requests response
type BulkRequestsResponseBody = {
    Results: IKintoneResponseBody list
} with interface IKintoneResponseBody
