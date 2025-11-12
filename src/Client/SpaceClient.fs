namespace Kintone.Client

open System
open Kintone.Client.Model
open Kintone.Client.Api

/// Space client for space operations
type SpaceClient(internalClient: InternalClient) =

    /// Get a space by ID
    member this.GetSpace(spaceId: int64) : SpaceBody =
        let request: GetSpaceRequest = { Id = spaceId }
        let response = internalClient.Call<GetSpaceRequest, GetSpaceResponseBody>(HttpMethod.Get, KintoneApi.GetSpace, Some request)
        {
            Id = response.Id
            Name = response.Name
            DefaultThread = response.DefaultThread
            IsPrivate = response.IsPrivate
            Creator = response.Creator
            Modifier = response.Modifier
            MemberCount = response.MemberCount
            CoverType = response.CoverType
            CoverKey = response.CoverKey
            CoverUrl = response.CoverUrl
            Body = response.Body
            UseMultiThread = response.UseMultiThread
            IsGuest = response.IsGuest
            Attachments = response.Attachments
        }

    /// Add a space from a template
    member this.AddSpaceFromTemplate(templateId: int64, name: string, ?members: SpaceMember list, ?isPrivate: bool, ?isGuest: bool) : int64 =
        let request = {
            Id = templateId
            Name = name
            Members = members
            IsPrivate = isPrivate
            IsGuest = isGuest
        }
        let response = internalClient.Call<AddSpaceFromTemplateRequest, AddSpaceFromTemplateResponseBody>(HttpMethod.Post, KintoneApi.AddSpaceFromTemplate, Some request)
        response.Id

    /// Update a space
    member this.UpdateSpace(spaceId: int64, ?body: string, ?name: string, ?attachments: SpaceAttachment list) : unit =
        let request = {
            Id = spaceId
            Body = body
            Name = name
            Attachments = attachments
        }
        let _ = internalClient.Call<UpdateSpaceRequest, UpdateSpaceResponseBody>(HttpMethod.Put, KintoneApi.UpdateSpace, Some request)
        ()

    /// Get space members
    member this.GetSpaceMembers(spaceId: int64) : SpaceMember list =
        let request: GetSpaceMembersRequest = { Id = spaceId }
        let response = internalClient.Call<GetSpaceMembersRequest, GetSpaceMembersResponseBody>(HttpMethod.Get, KintoneApi.GetSpaceMembers, Some request)
        response.Members

    /// Update space members
    member this.UpdateSpaceMembers(spaceId: int64, members: SpaceMember list) : unit =
        let request = {
            Id = spaceId
            Members = members
        }
        let _ = internalClient.Call<UpdateSpaceMembersRequest, UpdateSpaceMembersResponseBody>(HttpMethod.Put, KintoneApi.UpdateSpaceMembers, Some request)
        ()

    /// Add a thread to a space
    member this.AddThread(spaceId: int64, name: string, ?body: string) : int64 =
        let request = {
            Space = spaceId
            Name = name
            Body = body
        }
        let response = internalClient.Call<AddThreadRequest, AddThreadResponseBody>(HttpMethod.Post, KintoneApi.AddThread, Some request)
        response.Id

    /// Update a thread
    member this.UpdateThread(threadId: int64, ?name: string, ?body: string) : unit =
        let request = {
            Id = threadId
            Name = name
            Body = body
        }
        let _ = internalClient.Call<UpdateThreadRequest, UpdateThreadResponseBody>(HttpMethod.Put, KintoneApi.UpdateThread, Some request)
        ()

    /// Add guests to the organization
    member this.AddGuests(guests: Guest list) : unit =
        let request = {
            Guests = guests
        }
        let _ = internalClient.Call<AddGuestsRequest, AddGuestsResponseBody>(HttpMethod.Post, KintoneApi.AddGuests, Some request)
        ()
