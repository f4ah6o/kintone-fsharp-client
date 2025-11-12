namespace Kintone.Client.Model

open System
open System.Numerics

/// Field type enumeration
type FieldType =
    | SingleLineText
    | MultiLineText
    | RichText
    | Number
    | Calc
    | Date
    | DateTime
    | Time
    | CheckBox
    | RadioButton
    | DropDown
    | MultiSelect
    | File
    | Link
    | UserSelect
    | GroupSelect
    | OrganizationSelect
    | Subtable
    | Category
    | RecordNumber
    | CreatedTime
    | Creator
    | UpdatedTime
    | Modifier
    | Status
    | StatusAssignee
    | Group
    | ReferenceTable
    | Label
    | Spacer
    | Hr
    | Id
    | Revision

/// Field value types
type FieldValue =
    | SingleLineText of string option
    | MultiLineText of string option
    | RichText of string option
    | Number of decimal option
    | Calc of string option
    | Date of DateTime option
    | DateTime of DateTimeOffset option
    | Time of TimeSpan option
    | CheckBox of string list
    | RadioButton of string option
    | DropDown of string option
    | MultiSelect of string list
    | File of FileBody list
    | Link of string option
    | UserSelect of User list
    | GroupSelect of Group list
    | OrganizationSelect of Organization list
    | Subtable of TableRow list
    | Category of string list
    | RecordNumber of string option
    | CreatedTime of DateTimeOffset option
    | Creator of User option
    | UpdatedTime of DateTimeOffset option
    | Modifier of User option
    | Status of string option
    | StatusAssignee of User list
    | Id of int64 option
    | Revision of int64 option

/// Table row for subtable fields
and TableRow = {
    Id: string option
    Value: Map<string, FieldValue>
}

/// Field size
type FieldSize = {
    Width: string option
    Height: string option
    InnerHeight: string option
}

/// Option for selection fields
type FieldOption = {
    Label: string
    Index: int option
}

/// Unit position for number fields
type UnitPosition =
    | Before
    | After

/// Field property types
type FieldProperty =
    | SingleLineTextProperty of SingleLineTextFieldProperty
    | MultiLineTextProperty of MultiLineTextFieldProperty
    | RichTextProperty of RichTextFieldProperty
    | NumberProperty of NumberFieldProperty
    | CalcProperty of CalcFieldProperty
    | DateProperty of DateFieldProperty
    | DateTimeProperty of DateTimeFieldProperty
    | TimeProperty of TimeFieldProperty
    | CheckBoxProperty of CheckBoxFieldProperty
    | RadioButtonProperty of RadioButtonFieldProperty
    | DropDownProperty of DropDownFieldProperty
    | MultiSelectProperty of MultiSelectFieldProperty
    | FileProperty of FileFieldProperty
    | LinkProperty of LinkFieldProperty
    | UserSelectProperty of UserSelectFieldProperty
    | GroupSelectProperty of GroupSelectFieldProperty
    | OrganizationSelectProperty of OrganizationSelectFieldProperty
    | SubtableProperty of SubtableFieldProperty
    | CategoryProperty of CategoryFieldProperty
    | RecordNumberProperty of RecordNumberFieldProperty
    | CreatedTimeProperty of CreatedTimeFieldProperty
    | CreatorProperty of CreatorFieldProperty
    | UpdatedTimeProperty of UpdatedTimeFieldProperty
    | ModifierProperty of ModifierFieldProperty
    | StatusProperty of StatusFieldProperty
    | StatusAssigneeProperty of StatusAssigneeFieldProperty
    | ReferenceTableProperty of ReferenceTableFieldProperty
    | LabelProperty of LabelFieldProperty
    | GroupProperty of GroupFieldProperty

/// Base field property attributes
and BaseFieldProperty = {
    Code: string
    Label: string
    NoLabel: bool option
}

/// Single line text field property
and SingleLineTextFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Unique: bool option
    MinLength: int option
    MaxLength: int option
    DefaultValue: string option
    Expression: string option
    HideExpression: bool option
}

/// Multi line text field property
and MultiLineTextFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    DefaultValue: string option
}

/// Rich text field property
and RichTextFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    DefaultValue: string option
}

/// Number field property
and NumberFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Unique: bool option
    MinValue: decimal option
    MaxValue: decimal option
    DefaultValue: decimal option
    Digit: bool option
    DisplayScale: int option
    Unit: string option
    UnitPosition: UnitPosition option
}

/// Calc field property
and CalcFieldProperty = {
    Base: BaseFieldProperty
    Expression: string option
    Digit: bool option
    DisplayScale: int option
    Unit: string option
    UnitPosition: UnitPosition option
    HideExpression: bool option
}

/// Date field property
and DateFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Unique: bool option
    DefaultValue: DateTime option
    DefaultExpression: string option
}

/// DateTime field property
and DateTimeFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Unique: bool option
    DefaultValue: DateTimeOffset option
    DefaultExpression: string option
}

/// Time field property
and TimeFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    DefaultValue: TimeSpan option
    DefaultExpression: string option
}

/// CheckBox field property
and CheckBoxFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Options: FieldOption list
    DefaultValue: string list option
    Align: string option
}

/// RadioButton field property
and RadioButtonFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Options: FieldOption list
    DefaultValue: string option
    Align: string option
}

/// DropDown field property
and DropDownFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Options: FieldOption list
    DefaultValue: string option
}

/// MultiSelect field property
and MultiSelectFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Options: FieldOption list
    DefaultValue: string list option
}

/// File field property
and FileFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    ThumbnailSize: int option
}

/// Link field property
and LinkFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    Unique: bool option
    MaxLength: int option
    MinLength: int option
    DefaultValue: string option
    Protocol: string option
}

/// UserSelect field property
and UserSelectFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    DefaultValue: User list option
    Entities: Entity list option
}

/// GroupSelect field property
and GroupSelectFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    DefaultValue: Group list option
    Entities: Entity list option
}

/// OrganizationSelect field property
and OrganizationSelectFieldProperty = {
    Base: BaseFieldProperty
    Required: bool option
    DefaultValue: Organization list option
    Entities: Entity list option
}

/// Subtable field property
and SubtableFieldProperty = {
    Base: BaseFieldProperty
    Fields: Map<string, FieldProperty>
}

/// Category field property
and CategoryFieldProperty = {
    Base: BaseFieldProperty
    Enabled: bool option
}

/// RecordNumber field property
and RecordNumberFieldProperty = {
    Base: BaseFieldProperty
}

/// CreatedTime field property
and CreatedTimeFieldProperty = {
    Base: BaseFieldProperty
}

/// Creator field property
and CreatorFieldProperty = {
    Base: BaseFieldProperty
}

/// UpdatedTime field property
and UpdatedTimeFieldProperty = {
    Base: BaseFieldProperty
}

/// Modifier field property
and ModifierFieldProperty = {
    Base: BaseFieldProperty
}

/// Status field property
and StatusFieldProperty = {
    Base: BaseFieldProperty
}

/// StatusAssignee field property
and StatusAssigneeFieldProperty = {
    Base: BaseFieldProperty
}

/// ReferenceTable field property
and ReferenceTableFieldProperty = {
    Base: BaseFieldProperty
    ReferenceTable: string option
}

/// Label field property
and LabelFieldProperty = {
    Base: BaseFieldProperty
}

/// Group field property
and GroupFieldProperty = {
    Base: BaseFieldProperty
    OpenGroup: bool option
}
