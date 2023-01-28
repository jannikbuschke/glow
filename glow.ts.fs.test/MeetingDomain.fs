namespace MeetingDomain

open System.Collections.Generic
open System.Text.Json.Serialization
open Glow.Glue.AspNetCore
open System
open MediatR

type ReportId = ReportId of Guid

module ReportId =
  let create (rawId: Guid) = ReportId rawId

  let value (id: ReportId) =
    let (ReportId rawId) = id
    rawId

type UserId = UserId of string

module UserId =
  let validate (rawId: string) : Result<UserId, string> =
    if System.String.IsNullOrEmpty rawId then
      match rawId with
      | null -> Result.Error "Value is null"
      | "" -> Result.Error "Value is empty string"
      | _ -> Result.Error "unknown"
    else
      Result.Ok(UserId rawId)

  let tryCreate (rawId: string) : UserId option =
    let result = validate rawId

    match result with
    | Result.Ok ok -> Some ok
    | _ -> None

  // can fail with exception
  let create (rawId: string) =
    let id = validate rawId

    match id with
    | Result.Ok id -> id
    | Result.Error errorValue -> failwith ("invalid userid: " + errorValue)

  let value (userId: UserId) =
    let (UserId rawId) = userId
    rawId


open System

type AssignmentId = AssignmentId of Guid

module AssignmentId =
  let create (rawId: Guid) = AssignmentId rawId

  let value (AssignmentId rawId) : Guid = rawId

open System

type CircularId = CircularId of System.Guid


module CircularId =
  let create (rawId: System.Guid) : CircularId = CircularId rawId
  let value (CircularId rawId) : Guid = rawId

type MeetingId = MeetingId of System.Guid

module MeetingId =
  let create (rawId: System.Guid) = MeetingId rawId

  let value (MeetingId rawId) = rawId

type MeetingItemId = MeetingItemId of System.Guid

module MeetingItemId =
  let create rawId = MeetingItemId rawId
  let value (MeetingItemId id) = id

type FileId = FileId of Guid

module FileId =
  let create (rawId: Guid) =
    if rawId = Guid.Empty then
      raise (BadRequestException("FileId is empty, sorry this is likely a bug. Please report it to the provider."))
    else
      FileId rawId

  let value (id: FileId) =
    let (FileId rawId) = id
    rawId

type ResolutionId = ResolutionId of Guid

module ResolutionId =
  let create (rawId: Guid) = ResolutionId rawId

  let value (ResolutionId rawId) : Guid = rawId

type ParentReference =
  | CircularId of CircularId
  | MeetingItemId of MeetingItemId


type ApiError =
    | BadRequest of string
    | NotFound of string
    | InternalServerError of string
    | ConfigurationError of string
    
type EmptyRecord =
  { Value: Skippable<unit> }


type ResolutionStatus =
  | Draft
  | ApprovedForVoting
  | NotApprovedForVoting
  | Postponed
  // to be removed
  | Aggreed
  | Agreed
  | Disagreed

type CreateResolution =
  { Text: string
    ValidFrom: NodaTime.Instant option
    OrganizationalUnitIds: Guid list
    OrganizationalUnitName: string option
    Status: ResolutionStatus option }

type ResolutionCreated =
  { ParentReference: ParentReference option
    Text: string
    ValidFrom: NodaTime.Instant option
    OrganizationalUnitIds: Guid list
    OrganizationalUnitName: string option
    Status: ResolutionStatus option }

type DeleteResolution = { Id: Guid }

type ResolutionDeleted = { Id: ResolutionId }

// type UpdateResolution =
//   { Text: string
//     ValidFrom: NodaTime.Instant option
//     OrganizationalUnitIds: Guid list
//     OrganizationalUnitName: string option
//     Status: ResolutionStatus }

type ResolutionUpdated =
  { Text: string
    ValidFrom: NodaTime.Instant option
    OrganizationalUnitIds: Guid list
    OrganizationalUnitName: string option
    Status: ResolutionStatus }


type UserFriendlyResolutionId = UserFriendlyResolutionId of string

type UserFriendlyResolutionIdUpdated = { UserfriendlyId: UserFriendlyResolutionId }

type ResolutionEvent =
  | ResolutionCreated of ResolutionCreated
  | ResolutionUpdated of ResolutionUpdated
  | ResolutionDeleted of EmptyRecord
  | UserFriendlyResolutionIdUpdated of UserFriendlyResolutionIdUpdated

type O365EventIdAndMailbox =
  { EventId: string
    UserOrMailboxId: string }

type O365MeetingMainInviteCreatedOrUpdated =
  { EventIdAndMailbox: O365EventIdAndMailbox
     }

type O365MeetingItemInviteCreatedOrUpdated =
  { EventIdAndMailbox: O365EventIdAndMailbox }


type ParticipantType =
  | Required
  | Optional
  | Resource

type InviteParticipant =
  { UserId: UserId
    Name: string
    Email: string
    ParticipantType: ParticipantType }

type ExpectedParticipantChange =
  | IsUpToDate
  | NeedToBeAdded
  | NeedToBeRemoved

type ParticipantWithExpectedChange =
  { Participant: InviteParticipant
    ExpectedChange: ExpectedParticipantChange }

type InviteOptions =
  { InviteBoardMembers: bool
    EnableOnlineMeeting: bool
     }

type InviteStartAndEnd =
  { Start: NodaTime.Instant
    End: NodaTime.Instant
     }

type BclInviteStartAndEnd =
  { Start: DateTime
    End: DateTime
    BclTimezoneId: string
    BclTimezoneInfo: TimeZoneInfo }

type InviteData =
  { Subject: string
    Body: string
    BodyPreview: string
    StartAndEnd: InviteStartAndEnd
    Participants: InviteParticipant list
    Location: string option
    TeamsLink: string option
    LinkToO365Event: string option }

type DesiredInviteData =
  { Subject: string
    Body: string
    BodyPreview: string
    RequiredParticipants: ParticipantWithExpectedChange list
    RequiredParticipantsAreUpToDate: bool
    OptionalParticipants: ParticipantWithExpectedChange list
    OptionalParticipantsAreUpToDate: bool
    Devices: ParticipantWithExpectedChange list
    DevicesAreUpToDate: bool
    ExpectedInviteStartAndEnd: InviteStartAndEnd
    CurrentInviteStartAndEnd: InviteStartAndEnd option
    StartAndEndIsUpToDate: bool
    Location: string option }

type SendMainInviteRequest =
  { 
    MeetingId: MeetingId
    InviteCoreMeetingMembers: bool

    Location: string option
    Subject: string
    Body: string
    EnableOnlineMeeting: bool }
  interface IRequest<O365MeetingMainInviteCreatedOrUpdated>

type AdditionalParticipants = { UserId: string; AsOptional: bool }

type SendItemInvite =
  { MeetingItemId: MeetingItemId
    AdditionalParticipants: AdditionalParticipants list
    Location: string option
    Subject: string
    Body: string
    CopyMainEventOnlineInviteLink: bool }


open System
open System.Text.Json.Serialization

type UsersAddedToMeetingItemMinutesDistribution = { UserIds: UserId list }
type UserRemovedFromMeetingItemMinutesDistribution = { UserId: UserId }
type MeetingItemMinutesDistributionActivated = EmptyRecord

// type AttendanceeUpdated =
//   { TopicId: Guid
//     BoardMemberId: Guid
//     Duration: Gertrud.Meetings.AttendanceDuration }

// MeetingItemMinutes
type MinutesTextUpdated = { Value: string }

type MeetingItemMinutesFeedbackRequested =
  { AssigneesUserIds: UserId list
    Message: string
    Deadline: NodaTime.Instant option }


// When feedback assignee consents to the minutes and changes of other assignees
type MeetingItemMinutesFeedbackConsented = EmptyRecord
//   {
//   WorkaroundForEmptyRecord: Skippable<string>
// }

// when planner accepts feedback from assignees
type MeetingItemMinutesFeedbackAcceptedWithChanges =
  { WorkaroundForEmptyRecord: Skippable<string> }

type MeetingItemMinutesFeedbackAccepted =
  { WorkaroundForEmptyRecord: Skippable<string> }

type MeetingItemMinutesFeedbackRejected =
  { RolledBackMinutes: string
    WorkaroundForEmptyRecord: Skippable<string> }

type MeetingItemMinutesFeedbackStatus =
  | Active
  | Closed

type FeedbackAssignee = { UserId: UserId; Consented: bool }

type MeetingItemMinutesFeedbackRequest =
  { Status: MeetingItemMinutesFeedbackStatus
    AssigneesUserIds: UserId list
    Assignees: FeedbackAssignee list
    Message: string
    Deadline: NodaTime.Instant option
    CreatedBy: UserId option
    CreatedAtVersion: int64
    CreatedAt: NodaTime.Instant
    ClosedAt: NodaTime.Instant option
    ClosedAtVersion: int64 option }
  member this.IsActive() =
    let isDeadlineReached =
      match this.Deadline with
      | None -> false
      | Some deadline -> deadline < (NodaTime.Extensions.DateTimeExtensions.ToInstant DateTime.UtcNow)

    not isDeadlineReached
    && (this.Status = MeetingItemMinutesFeedbackStatus.Active)

  member this.IsAssigned(userId: UserId) =
    this.Assignees
    |> List.exists (fun a -> a.UserId = userId)

[<CLIMutable>]
type MinutesItem =
  { Minutes: string
    // if users should provide feedback, this will have a Some value
    FeedbackRequest: MeetingItemMinutesFeedbackRequest option
    DistributionList: UserId list
    Published: bool
    AssignmentIds: AssignmentId list
    ResolutionIds: ResolutionId list }

type MinuteItemChanged =
  | AssignmentAdded of AssignmentId
  | AssignmentRemoved of AssignmentId
  // typo cannot currently be fixed (requires data migration)
  | ASsignmentUpdated of AssignmentId
  | ResolutionAdded of ResolutionId
  | ResolutionRemoved of ResolutionId
  | ResolutionUpdated of ResolutionId

type MeetingItemMinutesUpdated = { Changes: MinuteItemChanged list }

// all the following events should be replaced by MeetingItemMinutesUpdated
type MeetingItemAssignmentUpdated = { AssignmentId: AssignmentId }

type MeetingItemResolutionUpdated = { ResoultionId: ResolutionId }

type AssignmentAddedToMeetingItem = { AssignmentId: AssignmentId }

type ResolutionAddedToMeetingItem = { ResolutionId: ResolutionId }

type AssignmentRemovedFromMeetingItem = { AssignmentId: AssignmentId }

type ResolutionRemovedFromMeetingItem = { ResolutionId: ResolutionId }

type Approver = { UserId: string }

type Approvable =
  { ByPlanners: bool
    ByApprovers: Approver list }

type MeetingItemStatus =
  | Pending = 1
  | TentativelyApproved = 2
  | FinallyApproved = 3
  | Rejected = 4

type MeetingItemReview =
  { Message: string option
    ReviewerId: string
    Result: MeetingItemStatus }

type File =
  { Id: Guid
    Name: string
    Path: string
    DisplayName: string
    FileClassificationId: Nullable<Guid> }

type AttendeeParticipationType =
  | Remote
  | Local
  | NoParticipation

type InviteType =
  | Required
  | Optional
  | NoInvite

type Attendee =
  { UserId: UserId
    ParticipationType: AttendeeParticipationType
    IsPresenter: bool
    InviteType: InviteType }

  member this.ResetPresenterAndParticipationType() =
    if this.InviteType = InviteType.NoInvite then
      { this with
          IsPresenter = false
          ParticipationType = AttendeeParticipationType.NoParticipation }
    else
      this

type MeetingMemberIdentifier =
  { MemberId: Guid option
    UserId: string }

type ChannelIdentity = { ChannelId: string; TeamId: string }

type ChatMessageReference =
  { Id: string
    Subject: string option
    Etag: string
    ChatId: string option
    ChannelIdentity: ChannelIdentity
    WebUrl: string }

module ChatMessageReference =
  let fromChatMessage (message: Microsoft.Graph.ChatMessage) =
    { Id = message.Id
      ChatId = message.ChatId |> Option.ofObj
      Subject = message.Subject |> Option.ofObj
      ChannelIdentity =
        { ChannelId = message.ChannelIdentity.ChannelId
          TeamId = message.ChannelIdentity.TeamId }
      Etag = message.Etag
      WebUrl = message.WebUrl }

type DriveItemReference =
  { DriveId: string
    DriveType: string
    Id: string
    Name: string option
    Path: string
    ShareId: string option
    SiteId: string option
    ODataType: string option }

// Sharepoint/DriveItem FileSyncResult
type FileSyncResult =
  { LastModified: DateTimeOffset option
    Size: int64 option
    ParentReference: DriveItemReference option
    Name: string
    Id: string
    Etag: string
    Ctag: string
    WebUrl: string }

module FileSyncResult =
  let fromDriveItem (driveItem: Microsoft.Graph.DriveItem) =
    { LastModified = Option.ofNullable driveItem.LastModifiedDateTime
      Size = Option.ofNullable driveItem.Size
      ParentReference =
        (match driveItem.ParentReference with
         | null -> None
         | _ ->
           let driveItemReference: DriveItemReference =
             { DriveId = driveItem.ParentReference.DriveId
               DriveType = driveItem.ParentReference.DriveType
               Id = driveItem.ParentReference.Id
               Path = driveItem.ParentReference.Path
               Name = Option.ofObj driveItem.ParentReference.Name
               ShareId = Option.ofObj driveItem.ParentReference.ShareId
               SiteId = Option.ofObj driveItem.ParentReference.SiteId
               ODataType = Option.ofObj driveItem.ParentReference.ODataType }

           Some(driveItemReference))
      Name = driveItem.Name
      Id = driveItem.Id
      Etag = driveItem.ETag
      Ctag = driveItem.CTag
      WebUrl = driveItem.WebUrl }

type SyncFileStatus =
  | NeverSynced
  | Synced
  | RemoteEdited
  | LocalEdited
  | RemoteRemoved
  | Syncing

type MeetingItemFile =
  { FileId: FileId
    AssignedToAll: bool
    AssignedTo: UserId list
    StandardFileSyncResult: FileSyncResult option
    MemberFileSyncResult: Dictionary<string, FileSyncResult> option }

type MeetingItemReassigned =
  { SourceMeetingId: MeetingId
    TargetMeetingId: MeetingId }

type MeetingItemAssigned = { MeetingItemId: MeetingItemId }

type SubmitterSet = { Attendee: Attendee }

type SubmitterRemoved = { UserId: UserId }

type AttendeeAdded = { Attendee: Attendee }

type SubmitterUpdated = { Attendee: Attendee }

type AttendeeUpdated = { Attendee: Attendee }

type AttendeeRemoved = { UserId: UserId }

type AttendeeRepositioned = { UserId: UserId; TargetPosition: int }

type MeetingItemApprovalStatus =
  | Pending
  | Approved
  | Rejected

type CreateMeetingItem =
  { Id: Guid option
    DisplayName: string

    Goal: string option
    Description: string option

    MeetingItemTypeId: Guid option
    ApprovalStatus: Skippable<MeetingItemApprovalStatus>
    ConfidentialityLevelId: Guid option

    DecisionTemplate: string option
    ActionsTemplate: string option
    MinutesTemplate: string option

    MeetingId: MeetingId option

    OrgainzationalUnitIds: Guid list

    Submitter: Attendee option
    Attendees: Attendee list
    SubstitudeIds: string list

    TagIds: Guid list
    Approvable: Approvable
    // This is the 'desired' duration as registered by the applicant
    // the actual duration will be set on the agenda
    DurationInMinutes: int

    GroupId: Guid option

    Comments: string option

    MeetingItemFiles: MeetingItemFile list

    ResponsibleMemberUserIds: string list
}

type MeetingItemCreated =
  { DisplayName: string

    Goal: string option
    Description: string option
    ApprovalStatus: Skippable<MeetingItemApprovalStatus>
    MeetingItemTypeId: Guid option

    ConfidentialityLevelId: Guid option

    DecisionTemplate: string option
    ActionsTemplate: string option
    MinutesTemplate: string option

    MeetingId: MeetingId option

    OrganizationalUnitIds: Guid list

    Submitter: Attendee option
    Attendees: Attendee list
    SubstitudeIds: string list

    TagIds: Guid list
    Approvable: Approvable

    DurationInMinutes: int
    GroupId: Guid option

    Comments: string option

    MeetingItemFiles: MeetingItemFile list

    ResponsibleMemberUserIds: UserId list
}


type MeetingItemUpdated =
  { DisplayName: string
    Goal: string option
    Description: string option
    Color: string option
    ResponsibleMemberUserIds: UserId list

    MeetingItemTypeId: Guid option

    DecisionTemplate: string option
    ActionsTemplate: string option
    MinutesTemplate: string option

    OrgainzationalUnitIds: Guid list

    Submitter: Attendee option
    Attendees: Attendee list
    SubstitudeIds: string list

    ConfidentialityLevelId: Guid option

    TagIds: Guid list
    DurationInMinutes: int
    Comments: string option }

type MeetingItemApproved = { Message: string option }
type MeetingItemRejected = { Message: string option }
type MeetingItemResubmitted = { Message: string }
type MeetingItemDeleted = { Id: MeetingItemId }
type MeetingItemFilesUpdated = { Files: FileId list }

type RegistrationFilesUpdated = { Files: FileId list }

type FilesUpdated =
  { MeetingItemFiles: MeetingItemFile list }

type MeetingItemFileAdded = { MeetingItemFile: MeetingItemFile }

type MeetingItemFileRemoved = { FileId: FileId }

type MeetingItemFileSynced =
  { FileId: FileId
    FileSyncResult: FileSyncResult
    MeetingMemberUserId: string option }

type InviteOptionsUpdated = { InviteEnabled: bool }

// maybe add status flag: Accepted | WithChanges | Rejected
type FeedbackClosed = { MeetingItemId: MeetingItemId }

type FeedbackAccepted =
  { WorkaroundForEmptyRecord: Skippable<string> }
// type FeedbackConsentReset = {
//   WorkaroundForEmptyRecord: Skippable<string>
// }

type FeedbackConsentReset =
  { WorkaroundForEmptyRecord: Skippable<string> }

type MinutesTextRolledBack = { Value: string }

type MeetingItemEvent =
  | FilesUpdated of FilesUpdated
  | MeetingItemUpdated of MeetingItemUpdated
  | O365MeetingItemInviteCreatedOrUpdated of O365MeetingItemInviteCreatedOrUpdated

  | ResolutionAddedToMeetingItem of ResolutionAddedToMeetingItem
  | ResolutionRemovedFromMeetingItem of ResolutionRemovedFromMeetingItem
  | AssignmentRemovedFromMeetingItem of AssignmentRemovedFromMeetingItem
  | AssignmentAddedToMeetingItem of AssignmentAddedToMeetingItem

  | UsersAddedToMeetingItemMinutesDistribution of UsersAddedToMeetingItemMinutesDistribution
  | UserRemovedFromMeetingItemMinutesDistribution of UserRemovedFromMeetingItemMinutesDistribution
  | MeetingItemMinutesDistributionActivated of MeetingItemMinutesDistributionActivated

  | MeetingItemRejected of MeetingItemRejected
  | MeetingItemFileSynced of MeetingItemFileSynced
  | MeetingItemResubmitted of MeetingItemResubmitted
  | MeetingItemApproved of MeetingItemApproved
  | MeetingItemCreated of MeetingItemCreated
  | MeetingItemFileAdded of MeetingItemFileAdded
  | MeetingItemFileRemoved of MeetingItemFileRemoved

  | MeetingItemDeleted of EmptyRecord

  | InviteOptionsUpdated of InviteOptionsUpdated

  | SubmitterSet of SubmitterSet
  | SubmitterRemoved of SubmitterRemoved
  | SubmitterUpdated of SubmitterUpdated
  | AttendeeAdded of AttendeeAdded
  | AttendeeRemoved of AttendeeRemoved
  | AttendeeUpdated of AttendeeUpdated
  | AttendeeRepositioned of AttendeeRepositioned
  | MeetingItemReassigned of MeetingItemReassigned

  | MeetingItemMinutesUpdated of MeetingItemMinutesUpdated
  // minutes events, are projected to MeetingItemMinutes projection
  | FeedbackRequested of MeetingItemMinutesFeedbackRequested
  | MinutesTextUpdated of MinutesTextUpdated
  | MinutesTextRolledBack of MinutesTextRolledBack
  // not needed anymore
  | FeedbackClosed of FeedbackClosed
  | FeedbackAccepted of MeetingItemMinutesFeedbackAccepted
  | FeedbackRejected of MeetingItemMinutesFeedbackRejected
  | FeedbackAcceptedWithChanges of MeetingItemMinutesFeedbackAcceptedWithChanges
  | FeedbackConsented of MeetingItemMinutesFeedbackConsented
  | FeedbackConsentReset of FeedbackConsentReset

[<CLIMutable>]
type MeetingItem =
  { Id: Guid
    MeetingId: MeetingId option
    DisplayName: string
    Goal: string option
    Description: string option
    Color: string option
    ConfidentialityLevelId: Guid option
    ResponsibleMemberUserIds: UserId list
    CreatedBy: string option
    LastO365InviteUpdate: O365MeetingItemInviteCreatedOrUpdated option
    ApplicantId: UserId option
    Submitter: Attendee option
    SubstituteIds: string list
    AlignedWithMember: bool option
    Attendees: Attendee list
    GroupId: Guid option
    OrganizationalUnitIds: Guid list
    DecisionTemplate: string option
    MinutesTemplate: string option
    ActionsTemplate: string option
    ApprovalStatus: MeetingItemApprovalStatus
    TagIds: Guid list
    Approvable: Approvable
    CreatedAt: NodaTime.Instant
    MinutesItem: MinutesItem
    DurationInMinutes: int
    Comments: string option
    MeetingItemTypeId: Guid option
    MeetingItemFiles: MeetingItemFile list
    FileSyncResult: FileSyncResult option
    Reviews: MeetingItemReview list
    InviteEnabled: bool }



type AssignmentViewmodel =
  { AssignmentId: AssignmentId
    ParentReference: ParentReference option
    DisplayName: string
    AssigneeUserIds: UserId list
    DueDate: NodaTime.Instant option }

type GetAssignment =
  { AssignmentId: AssignmentId }

  interface IRequest<AssignmentViewmodel>



type AssignmentListViewmodel =
  { AssignmentId: AssignmentId
    DisplayName: string
    AssigneeUserIds: UserId list
    DueDate: NodaTime.Instant option }

type GetAssignmentsRequest =
  { Page: int }

  interface IRequest<AssignmentListViewmodel list>

type TeamAndChannelId =
  { TeamDisplayName: string
    ChannelDisplayName: string option }

type GetTeamAndChannelId =
  { TeamId: string
    ChannelId: string option }

  interface IRequest<TeamAndChannelId>

type CurrentMeetingMainEvent =
  { O365Event: Microsoft.Graph.Event
    LastSync: O365MeetingMainInviteCreatedOrUpdated }

type CurrentMeetingItemEvent =
  { O365Event: Microsoft.Graph.Event
    Id: O365EventIdAndMailbox }

type InviteState =
  | NoInviteNeeded
  | InviteDoesNotYetExist
  | InviteIsUpToDate
  | InviteIsNotUpToDate

type MeetingEventDurationOptionViewmodel =
  { 
    Start: NodaTime.Instant option
    End: NodaTime.Instant option }

type MeetingMainInviteViewmodel =
  { MeetingDisplayName: string
    CurrentO365Event: Microsoft.Graph.Event option
    InviteHasBeenSent: NodaTime.Instant option
    LastSyncEvent: O365MeetingMainInviteCreatedOrUpdated option
    InviteData: DesiredInviteData
    DurationOptions: MeetingEventDurationOptionViewmodel list
    InviteState: InviteState }

type MeetingItemInvitesViewmodel =
  { MeetingItemDisplayName: string
    MeetingItemId: MeetingItemId
    CurrentO365Event: Microsoft.Graph.Event option
    InviteHasBeenSent: NodaTime.Instant option
    LastSyncEvent: O365MeetingItemInviteCreatedOrUpdated option
    InviteData: DesiredInviteData
    InviteState: InviteState }


type MeetingCreatedResult = { MeetingId: MeetingId }

type Member ={
  Id: Guid
  UserId:UserId
}

type CreateMeeting =
  { Id: Guid option
    BoardId: Guid option
    Title: string
    Subtitle: string
    InviteText: string
    PlannedDurationInMinutes: int option
    Date: NodaTime.Instant
    RegistrationDeadlineInHours: int
    DocumentDeadlineInHours: int
    Location: string option
    Recorder: UserId option }
  interface IRequest<MeetingCreatedResult>

type CreateMeetingViewmodel =
  { InitialRequest: CreateMeeting
    Boardmembers: Member list
    Start: NodaTime.LocalTime }

type GetCreateMeetingViewmodel =
  { BoardId: Guid option }
  interface IRequest<CreateMeetingViewmodel>


  type ItemFileViewModel =
    { FileId: FileId
      AssignedToAll: bool
      AssignedTo: UserId list
      SensitivityId: Guid option
      ContainsPersonalData: bool option
      TypeIds: Guid list
      SyncStatus: SyncFileStatus
      StandardFileSyncResult: FileSyncResult option
      MemberFileSyncResult: Dictionary<string, FileSyncResult> option
      Name: string
      DisplayName: string option
      IsPlaceholder: bool }

  type MeetingItemFilesViewModel =
    { Files: ItemFileViewModel list
      MeetingItemId: MeetingItemId
      DisplayIndex: string
      DisplayName: string }

  type MeetingFilesViewModel =
    { MeetingId: MeetingId
      Items: MeetingItemFilesViewModel list
      MeetingFinalFolder: FileSyncResult option
      MeetingDraftFolder: FileSyncResult option
       }

  type GetAllFiles =
    { MeetingId: MeetingId }
    interface IRequest<MeetingFilesViewModel>

  type UpdateMeetingItemFile =
    { MeetingItemId: MeetingItemId
      FileId: FileId
      Name: string
      DisplayName: string option
      ConfidentialityLevelId: Guid option
      ContainsPersonalData: bool option
      TypeIds: Guid list
      IsPlaceholder: bool
      AssignedTo: UserId list
      AssignedToAll: bool }
    interface IRequest<Unit>

  type UpdateMyTopicFile =
    { MeetingItemId: MeetingItemId
      FileId: FileId
      Name: string
      DisplayName: string option }
    interface IRequest<Unit>

  type DeleteMeetingItemFile =
    { MeetingItemId: MeetingItemId
      FileId: FileId }
    interface IRequest<Unit>

  type StartMeetingFilesSyncInBackground =
    { MeetingId: MeetingId }
    interface IRequest<Unit>


type HeaderViewmodel =
  { MeetingId: MeetingId
    Title: string
    Subtitle: string option

    Start: NodaTime.Instant
    End: NodaTime.Instant
    PlannendEnd: NodaTime.Instant }


type GetHeaderViewmodel =
  { 
    Id: MeetingId }
  interface IRequest<HeaderViewmodel>


  type MeetingHeader =
    { Id: MeetingId
      Title: string
      Subtitle: string option
      Date: NodaTime.Instant
      Location: string option
      PlannedEnd: NodaTime.Instant option
      ActualEnd: NodaTime.Instant
      ActualDurationInMinutes: int32
      PlannedDurationInMinutes: int32 option }

  type GetMeetingHeader =
    { Id: MeetingId }
    interface IRequest<MeetingHeader>
type MeetingMember={
  Id: Guid
  UserId: UserId
}

type GetMeetingMembersRequest =
  { 
    MeetingId: MeetingId }
  interface IRequest<MeetingMember list>

type MeetingMemberAsUser={Name:string}
type GetMeetingMembersAsUsersRequest =
  { 
    MeetingId: MeetingId }
  interface IRequest<MeetingMemberAsUser list>

type ArchiveMeeting =
  { MeetingId: MeetingId }

  interface IRequest<Result<unit, ApiError>>

type ReactivateArchivedMeeting =
  { MeetingId: MeetingId }
  interface IRequest<Result<unit, ApiError>>
  
type ParticipationSummary =
  { AcceptedOnline: int
    AcceptedOffline: int
    Rejected: int
    Pending: int }

type MeetingLocationType ={Name:string}
type MeetingWithDetails =
  { Id: MeetingId
    Title: string
    Start: NodaTime.Instant
    End: NodaTime.Instant
    Location: string option
    LocationType: MeetingLocationType
    ParticipationSummary: ParticipationSummary }


type OtherEvent =
  { Title: string
    Start: NodaTime.Instant
    End: NodaTime.Instant }

type MeetingOrOtherEvent =
   | Meeting of MeetingWithDetails
   | OtherEvent of OtherEvent

type Day =
  { Day: NodaTime.Instant
    Items: MeetingOrOtherEvent list }

type GetUpcomingmeetingsWithDetails =
  { Count: int }
  interface IRequest<Day list>


type ShareDocumentToTeamsViewmodel =
  { 
    TeamId: string
    FinalDocumentsChannelId: string
    DraftDocumentsChannelId: string
    Filename: string option
     }

type DocumentType =
  | Agenda
  | Minutes

type DocumentState =
  | Draft
  | Final

type GetShareDocumentToTeamsViewmodel =
  { DocumentType: DocumentType
    MeetingId: MeetingId
    DocumentState: DocumentState option
   }

  interface IRequest<Result<ShareDocumentToTeamsViewmodel, ApiError>>


type Role =
  | Planner
  | Boardmember
  | DefaultUser
  | Admin


type Dashboard =
  | DefaultUser of string
  | Planner of string
  | Boardmember of string

type SidebarViewmodel = {
  ShowCirculars: bool
}

type DashboardAndNavigationViewmodel =
  { Dashboard: Dashboard
    Sidebar: SidebarViewmodel
    MainRole: Role
    IsAdmin: bool
    IsPlanner: bool }

type GetDashboard =
  { Data: MediatR.Unit }
  interface IRequest<DashboardAndNavigationViewmodel>
