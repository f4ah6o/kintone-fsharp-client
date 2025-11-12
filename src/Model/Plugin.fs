namespace Kintone.Client.Model

open System

/// Plugin
type Plugin = {
    Id: string
    Name: string
    Description: string option
    Version: int option
    RequireAuth: bool option
    Enabled: bool option
}

/// App plugin
type AppPlugin = {
    Id: string
    Name: string option
    Version: string option
}

/// Installed plugin
type InstalledPlugin = {
    Id: string
    Name: string
    Description: string option
    Version: int
    RequireAuth: bool
}
