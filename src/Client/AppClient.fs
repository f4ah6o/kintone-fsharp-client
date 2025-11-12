namespace Kintone.Client

open System
open Kintone.Client.Model
open Kintone.Client.Api

/// App client for app operations
type AppClient(internalClient: InternalClient) =

    /// Get an app by ID
    member this.GetApp(appId: int64) : App =
        let request = { Id = appId }
        let response = internalClient.Call<GetAppRequest, GetAppResponseBody>(HttpMethod.Get, KintoneApi.GetApp, Some request)
        {
            AppId = response.AppId
            Code = Some response.Code
            Name = response.Name
            Description = Some response.Description
            SpaceId = response.SpaceId
            ThreadId = response.ThreadId
            CreatedAt = Some response.CreatedAt
            Creator = Some response.Creator
            ModifiedAt = Some response.ModifiedAt
            Modifier = Some response.Modifier
        }

    /// Get multiple apps
    member this.GetApps(?ids: int64 list, ?codes: string list, ?name: string, ?spaceIds: int64 list, ?limit: int, ?offset: int) : App list =
        let request = {
            Ids = ids
            Codes = codes
            Name = name
            SpaceIds = spaceIds
            Limit = Option.defaultValue 100 limit
            Offset = Option.defaultValue 0 offset
        }
        let response = internalClient.Call<GetAppsRequest, GetAppsResponseBody>(HttpMethod.Get, KintoneApi.GetApps, Some request)
        response.Apps

    /// Add a new app
    member this.AddApp(name: string, ?space: int64, ?thread: int64) : int64 =
        let request = {
            Name = name
            Space = space
            Thread = thread
        }
        let response = internalClient.Call<AddAppRequest, AddAppResponseBody>(HttpMethod.Post, KintoneApi.AddApp, Some request)
        response.App

    /// Get form fields
    member this.GetFormFields(app: int64, ?lang: string, ?preview: bool) : Map<string, FieldProperty> =
        let request = {
            App = app
            Lang = lang
            Preview = preview
        }
        let response = internalClient.Call<GetFormFieldsRequest, GetFormFieldsResponseBody>(HttpMethod.Get, KintoneApi.GetFormFields, Some request)
        response.Properties

    /// Add form fields
    member this.AddFormFields(app: int64, properties: Map<string, FieldProperty>, ?revision: int64) : int64 =
        let request = {
            App = app
            Properties = properties
            Revision = revision
        }
        let response = internalClient.Call<AddFormFieldsRequest, AddFormFieldsResponseBody>(HttpMethod.Post, KintoneApi.AddFormFields, Some request)
        response.Revision

    /// Update form fields
    member this.UpdateFormFields(app: int64, properties: Map<string, FieldProperty>, ?revision: int64) : int64 =
        let request = {
            App = app
            Properties = properties
            Revision = revision
        }
        let response = internalClient.Call<UpdateFormFieldsRequest, UpdateFormFieldsResponseBody>(HttpMethod.Put, KintoneApi.UpdateFormFields, Some request)
        response.Revision

    /// Delete form fields
    member this.DeleteFormFields(app: int64, fields: string list, ?revision: int64) : int64 =
        let request = {
            App = app
            Fields = fields
            Revision = revision
        }
        let response = internalClient.Call<DeleteFormFieldsRequest, DeleteFormFieldsResponseBody>(HttpMethod.Delete, KintoneApi.DeleteFormFields, Some request)
        response.Revision

    /// Get form layout
    member this.GetFormLayout(app: int64, ?preview: bool) : Layout list =
        let request = {
            App = app
            Preview = preview
        }
        let response = internalClient.Call<GetFormLayoutRequest, GetFormLayoutResponseBody>(HttpMethod.Get, KintoneApi.GetFormLayout, Some request)
        response.Layout

    /// Update form layout
    member this.UpdateFormLayout(app: int64, layout: Layout list, ?revision: int64) : int64 =
        let request = {
            App = app
            Layout = layout
            Revision = revision
        }
        let response = internalClient.Call<UpdateFormLayoutRequest, UpdateFormLayoutResponseBody>(HttpMethod.Put, KintoneApi.UpdateFormLayout, Some request)
        response.Revision

    /// Get views
    member this.GetViews(app: int64, ?lang: string, ?preview: bool) : Map<string, View> =
        let request = {
            App = app
            Lang = lang
            Preview = preview
        }
        let response = internalClient.Call<GetViewsRequest, GetViewsResponseBody>(HttpMethod.Get, KintoneApi.GetViews, Some request)
        response.Views

    /// Update views
    member this.UpdateViews(app: int64, views: Map<string, View>, ?revision: int64) : int64 =
        let request = {
            App = app
            Views = views
            Revision = revision
        }
        let response = internalClient.Call<UpdateViewsRequest, UpdateViewsResponseBody>(HttpMethod.Put, KintoneApi.UpdateViews, Some request)
        response.Revision

    /// Get app settings
    member this.GetAppSettings(app: int64, ?lang: string, ?preview: bool) : AppSettings =
        let request = {
            App = app
            Lang = lang
            Preview = preview
        }
        let response = internalClient.Call<GetAppSettingsRequest, GetAppSettingsResponseBody>(HttpMethod.Get, KintoneApi.GetAppSettings, Some request)
        {
            Name = response.Name
            Description = Some response.Description
            Icon = Some response.Icon
            Theme = Some response.Theme
        }

    /// Update app settings
    member this.UpdateAppSettings(app: int64, ?name: string, ?description: string, ?icon: AppIcon, ?theme: string, ?revision: int64) : int64 =
        let request = {
            App = app
            Name = name
            Description = description
            Icon = icon
            Theme = theme
            Revision = revision
        }
        let response = internalClient.Call<UpdateAppSettingsRequest, UpdateAppSettingsResponseBody>(HttpMethod.Put, KintoneApi.UpdateAppSettings, Some request)
        response.Revision

    /// Deploy an app
    member this.DeployApp(appId: int64, ?revision: int64, ?revert: bool) : unit =
        let request = {
            Apps = [{ App = appId; Revision = revision }]
            Revert = revert
        }
        let _ = internalClient.Call<DeployAppRequest, DeployAppResponseBody>(HttpMethod.Post, KintoneApi.DeployApp, Some request)
        ()

    /// Deploy multiple apps
    member this.DeployApps(apps: DeployApp list, ?revert: bool) : unit =
        let request = {
            Apps = apps
            Revert = revert
        }
        let _ = internalClient.Call<DeployAppRequest, DeployAppResponseBody>(HttpMethod.Post, KintoneApi.DeployApp, Some request)
        ()

    /// Get deploy status
    member this.GetDeployStatus(appIds: int64 list) : AppDeployStatus list =
        let request = {
            Apps = appIds
        }
        let response = internalClient.Call<GetDeployStatusRequest, GetDeployStatusResponseBody>(HttpMethod.Get, KintoneApi.GetDeployStatus, Some request)
        response.Apps

    /// Get app ACL
    member this.GetAppAcl(app: int64, ?preview: bool) : AppRightEntity list =
        let request = {
            App = app
            Preview = preview
        }
        let response = internalClient.Call<GetAppAclRequest, GetAppAclResponseBody>(HttpMethod.Get, KintoneApi.GetAppAcl, Some request)
        response.Rights

    /// Update app ACL
    member this.UpdateAppAcl(app: int64, rights: AppRightEntity list, ?revision: int64) : int64 =
        let request = {
            App = app
            Rights = rights
            Revision = revision
        }
        let response = internalClient.Call<UpdateAppAclRequest, UpdateAppAclResponseBody>(HttpMethod.Put, KintoneApi.UpdateAppAcl, Some request)
        response.Revision

    /// Get record ACL
    member this.GetRecordAcl(app: int64, ?lang: string, ?preview: bool) : RecordRightEntity list =
        let request = {
            App = app
            Lang = lang
            Preview = preview
        }
        let response = internalClient.Call<GetRecordAclRequest, GetRecordAclResponseBody>(HttpMethod.Get, KintoneApi.GetRecordAcl, Some request)
        response.Rights

    /// Update record ACL
    member this.UpdateRecordAcl(app: int64, rights: RecordRightEntity list, ?revision: int64) : int64 =
        let request = {
            App = app
            Rights = rights
            Revision = revision
        }
        let response = internalClient.Call<UpdateRecordAclRequest, UpdateRecordAclResponseBody>(HttpMethod.Put, KintoneApi.UpdateRecordAcl, Some request)
        response.Revision

    /// Evaluate record ACL
    member this.EvaluateRecordAcl(app: int64, ids: int64 list) : Map<int64, EvaluatedRecordRight> =
        let request = {
            App = app
            Ids = ids
        }
        let response = internalClient.Call<EvaluateRecordAclRequest, EvaluateRecordAclResponseBody>(HttpMethod.Get, KintoneApi.EvaluateRecordAcl, Some request)
        response.Rights

    /// Get field ACL
    member this.GetFieldAcl(app: int64, ?preview: bool) : FieldRight list =
        let request = {
            App = app
            Preview = preview
        }
        let response = internalClient.Call<GetFieldAclRequest, GetFieldAclResponseBody>(HttpMethod.Get, KintoneApi.GetFieldAcl, Some request)
        response.Rights

    /// Update field ACL
    member this.UpdateFieldAcl(app: int64, rights: FieldRight list, ?revision: int64) : int64 =
        let request = {
            App = app
            Rights = rights
            Revision = revision
        }
        let response = internalClient.Call<UpdateFieldAclRequest, UpdateFieldAclResponseBody>(HttpMethod.Put, KintoneApi.UpdateFieldAcl, Some request)
        response.Revision

    /// Get app plugins
    member this.GetAppPlugins(app: int64) : AppPlugin list =
        let request = {
            App = app
        }
        let response = internalClient.Call<GetAppPluginsRequest, GetAppPluginsResponseBody>(HttpMethod.Get, KintoneApi.GetAppPlugins, Some request)
        response.Plugins

    /// Add app plugins
    member this.AddAppPlugins(app: int64, pluginIds: string list) : unit =
        let request = {
            App = app
            PluginIds = pluginIds
        }
        let _ = internalClient.Call<AddAppPluginsRequest, AddAppPluginsResponseBody>(HttpMethod.Post, KintoneApi.AddAppPlugins, Some request)
        ()
