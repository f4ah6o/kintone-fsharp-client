namespace Kintone.Client.Model

open System

/// App representation
type App = {
    AppId: int64
    Code: string option
    Name: string
    Description: string option
    SpaceId: int64 option
    ThreadId: int64 option
    CreatedAt: DateTimeOffset option
    Creator: User option
    ModifiedAt: DateTimeOffset option
    Modifier: User option
}

/// View type
type ViewType =
    | List
    | Calendar
    | Custom

/// Built-in view type
type BuiltinType =
    | Assignee

/// Device scope
type Device =
    | Desktop
    | Mobile
    | Any

/// View representation
type View = {
    Id: int64 option
    Type: ViewType
    Name: string
    Index: int option
    BuiltinType: BuiltinType option
    Fields: string list option
    FilterCond: string option
    Sort: string option
    Pager: bool option
    Device: Device option
    // Calendar specific
    Title: string option
    DateField: string option
    // Custom view specific
    Html: string option
}

/// Layout types
type LayoutType =
    | Row
    | Subtable
    | Group

/// Field layout
type FieldLayout = {
    Type: FieldType
    Code: string
    Label: string option
    Size: FieldSize option
}

/// Row layout
type RowLayout = {
    Type: LayoutType
    Fields: FieldLayout list
}

/// Group layout
type GroupLayout = {
    Type: LayoutType
    Code: string
    Label: string option
    Open: bool option
    Layout: RowLayout list
}

/// Subtable layout
type SubtableLayout = {
    Type: LayoutType
    Code: string
    Fields: FieldLayout list
}

/// Layout element
type Layout =
    | Row of RowLayout
    | Group of GroupLayout
    | Subtable of SubtableLayout

/// App icon types
type AppIconType =
    | Preset
    | File

/// App icon
type AppIcon =
    | Preset of key: string
    | File of key: string * url: string option

/// App settings
type AppSettings = {
    Name: string
    Description: string option
    Icon: AppIcon option
    Theme: string option
}

/// Chart type
type ChartType =
    | Bar
    | Line
    | Pie
    | Area
    | Table

/// Aggregation function
type AggregationFunction =
    | Count
    | Sum
    | Average
    | Max
    | Min

/// Aggregation
type Aggregation = {
    Type: AggregationFunction
    Field: string option
}

/// Chart
type Chart = {
    Type: ChartType
    Name: string
    Index: int
    Size: string
    AxisX: string option
    AxisY: string option
    Legend: bool option
    GroupBy: string list option
    Aggregations: Aggregation list option
    FilterCond: string option
}

/// Report
type Report = {
    Id: int64 option
    Name: string
    Index: int option
    Charts: Chart list
}

/// Periodic report period types
type PeriodUnit =
    | Hour
    | Day
    | Week
    | Month
    | Quarter
    | Year

/// Periodic report period
type PeriodicReportPeriod = {
    Unit: PeriodUnit
    Every: int option
    DayOfWeek: string option
    DayOfMonth: int option
    Time: string option
}

/// Periodic report
type PeriodicReport = {
    FilterCond: string option
    Reports: int64 list
    Period: PeriodicReportPeriod
}

/// General notification
type GeneralNotification = {
    Id: int64 option
    Entity: Entity
    IncludeSubs: bool option
    RecordAdded: bool option
    RecordEdited: bool option
    CommentAdded: bool option
    StatusChanged: bool option
    FileImported: bool option
}

/// Per-record notification
type PerRecordNotification = {
    Id: int64 option
    FilterCond: string option
    Title: string option
    Targets: NotificationTarget list option
}

/// Notification target
and NotificationTarget = {
    Entity: Entity
    IncludeSubs: bool option
}

/// Reminder notification
type ReminderNotification = {
    Timing: ReminderTiming
    FilterCond: string option
    Title: string option
    Targets: NotificationTarget list option
}

/// Reminder timing
and ReminderTiming = {
    Code: string
    Value: string
    TimeZone: string option
}

/// Process state
type ProcessState = {
    Name: string
    Index: int option
    Assignee: ProcessAssignee option
}

/// Process action
and ProcessAction = {
    Name: string
    From: string
    To: string
    FilterCond: string option
}

/// Process assignee
and ProcessAssignee = {
    Type: string
    Entities: Entity list
}

/// Process management
type ProcessManagement = {
    Enable: bool
    States: Map<string, ProcessState>
    Actions: ProcessAction list
}

/// Field accessibility
type FieldAccessibility =
    | Readable
    | Writable
    | NoAccess

/// Field right
type FieldRight = {
    Code: string
    Entities: Entity list
    Accessibility: FieldAccessibility
}

/// Record right entity
type RecordRightEntity = {
    Entity: Entity
    ViewRecords: bool option
    AddRecords: bool option
    EditRecords: bool option
    DeleteRecords: bool option
    ImportRecords: bool option
    ExportRecords: bool option
}

/// Evaluated record right
type EvaluatedRecordRight = {
    View: bool
    Add: bool
    Edit: bool
    Delete: bool
}

/// App right entity
type AppRightEntity = {
    Entity: Entity
    AppEditable: bool option
    RecordViewable: bool option
    RecordAddable: bool option
    RecordEditable: bool option
    RecordDeletable: bool option
    RecordImportable: bool option
    RecordExportable: bool option
}

/// Admin notes
type AdminNotes = {
    Notes: string option
}

/// Customize scope
type CustomizeScope =
    | All
    | Admin
    | NoCustomization

/// Customize type
type CustomizeType =
    | JavaScript
    | Css

/// Customize resource
type CustomizeResource =
    | Url of url: string
    | File of key: string option * url: string option

/// Customize settings
type CustomizeSettings = {
    Desktop: CustomizeBody option
    Mobile: CustomizeBody option
}

/// Customize body
and CustomizeBody = {
    Js: CustomizeResource list
    Css: CustomizeResource list
}

/// App action
type AppAction = {
    Name: string
    Index: int option
    DestApp: DestApp
    Mappings: FieldMapping list
    Entities: Entity list option
}

/// Destination app
and DestApp = {
    App: int64
    Code: string option
}

/// Field mapping
and FieldMapping = {
    SrcField: string
    SrcType: FieldType
    DestField: string
    DestType: FieldType
}

/// Deploy status
type DeployStatus =
    | Processing
    | Success
    | Failed
    | Canceled

/// App deploy status
type AppDeployStatus = {
    App: int64
    Status: DeployStatus
}
