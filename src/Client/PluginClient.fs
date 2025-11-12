namespace Kintone.Client

open System
open Kintone.Client.Model
open Kintone.Client.Api

/// Plugin client for plugin operations
type PluginClient(internalClient: InternalClient) =

    /// Get available plugins
    member this.GetPlugins(?limit: int, ?offset: int) : Plugin list =
        let request = {
            Limit = limit
            Offset = offset
        }
        let response = internalClient.Call<GetPluginsRequest, GetPluginsResponseBody>(HttpMethod.Get, KintoneApi.GetPlugins, Some request)
        response.Plugins

    /// Install a plugin
    member this.InstallPlugin(pluginZip: byte array) : string =
        let request = {
            PluginZip = pluginZip
        }
        let response = internalClient.Call<InstallPluginRequest, InstallPluginResponseBody>(HttpMethod.Post, KintoneApi.InstallPlugin, Some request)
        response.PluginId

    /// Uninstall a plugin
    member this.UninstallPlugin(pluginId: string) : unit =
        let request: UninstallPluginRequest = {
            PluginId = pluginId
        }
        let _ = internalClient.Call<UninstallPluginRequest, UninstallPluginResponseBody>(HttpMethod.Delete, KintoneApi.UninstallPlugin, Some request)
        ()

    /// Update a plugin
    member this.UpdatePlugin(pluginId: string, pluginZip: byte array) : string =
        let request = {
            PluginId = pluginId
            PluginZip = pluginZip
        }
        let response = internalClient.Call<UpdatePluginRequest, UpdatePluginResponseBody>(HttpMethod.Put, KintoneApi.UpdatePlugin, Some request)
        response.PluginId
