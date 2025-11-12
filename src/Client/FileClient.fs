namespace Kintone.Client

open System
open Kintone.Client.Model
open Kintone.Client.Api

/// File client for file operations
type FileClient(internalClient: InternalClient) =

    /// Upload a file
    member this.UploadFile(fileName: string, contentType: string, data: byte array) : string =
        let request = {
            FileName = fileName
            ContentType = contentType
            Data = data
        }
        let response = internalClient.Call<UploadFileRequest, UploadFileResponseBody>(HttpMethod.Post, KintoneApi.UploadFile, Some request)
        response.FileKey

    /// Download a file
    member this.DownloadFile(fileKey: string) : byte array * string =
        let request: DownloadFileRequest = {
            FileKey = fileKey
        }
        let response = internalClient.Call<DownloadFileRequest, DownloadFileResponseBody>(HttpMethod.Get, KintoneApi.DownloadFile, Some request)
        (response.Data, response.ContentType)
