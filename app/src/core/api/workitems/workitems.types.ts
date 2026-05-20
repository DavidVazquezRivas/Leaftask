import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export interface WorkItemAssigneeData {
  id: string
  firstName: string
  lastName: string
}

export interface WorkItemData {
  id: string
  code: string
  title: string
  estimation: number | null
  progress: number
  assignee: WorkItemAssigneeData | null
  dedication: number | null
  typeId: string
  statusId: string
  parentId: string | null
}

export interface WorkItemTypeData {
  id: string
  name: string
}

export interface WorkItemStatusData {
  id: string
  name: string
}

export interface WorkItemDedicationDetailData {
  total: number
  registers: number
}

export interface WorkItemAttachmentData {
  id: string
  fileName: string
  url: string
}

export interface WorkItemCommentData {
  id: string
  author: { id: string; fullName: string }
  content: string
  createdAt: string
  attachments: WorkItemAttachmentData[]
}

export interface WorkItemLogValueData {
  value: string
}

export interface WorkItemLogEntryData {
  timestamp: string
  user: { id: string; fullName: string }
  fieldName: string
  oldValue: WorkItemLogValueData
  newValue: WorkItemLogValueData
}

export interface WorkItemCustomFieldValueData {
  fieldId: string
  name: string
  value: string
  typeId: string
}

export interface WorkItemDetailData {
  id: string
  code: string
  title: string
  description: string | null
  limitDate: string | null
  assignee: { id: string; fullName: string } | null
  estimation: number | null
  dedication: WorkItemDedicationDetailData
  progress: number
  typeId: string
  statusId: string
  parentId: string | null
  attachments: WorkItemAttachmentData[]
  comments: WorkItemCommentData[]
  log: WorkItemLogEntryData[]
  customFields: WorkItemCustomFieldValueData[]
}

export interface GetWorkItemsParams {
  limit?: number
  cursor?: string | null
  sort?: string[]
}

export interface CreateWorkItemRequest {
  title: string
  parentId: string
  typeId: string
  statusId: string
  estimation: number
  description?: string
  assigneeId?: string | null
  customFields?: Record<string, string>
}

export interface UpdateWorkItemRequest {
  title?: string
  description?: string
  statusId?: string
  typeId?: string
  assigneeId?: string | null
  updateAssignee?: boolean
  progress?: number
  estimation?: number
  limitDate?: string | null
  parentId?: string | null
  updateParent?: boolean
  customFields?: Record<string, string>
}

export interface WorkLogUserData {
  id: string
  firstName: string
  lastName: string
}

export interface WorkLogData {
  id: string
  dedication: number
  date: string
  user: WorkLogUserData
  description: string
}

export interface LogWorkRequest {
  dedication: number
  date: string
  description: string
}

export interface UpdateWorkLogRequest {
  dedication?: number
  date?: string
  description?: string
}

export type GetWorkLogsSuccessResponse = ApiSuccessResponse<WorkLogData[]>
export type GetWorkLogsApiResponse = ApiResponse<WorkLogData[]>
export type LogWorkSuccessResponse = ApiSuccessResponse<WorkLogData>
export type LogWorkApiResponse = ApiResponse<WorkLogData>
export type UpdateWorkLogSuccessResponse = ApiSuccessResponse<WorkLogData>
export type UpdateWorkLogApiResponse = ApiResponse<WorkLogData>

export type GetWorkItemsSuccessResponse = ApiSuccessResponse<WorkItemData[]>
export type GetWorkItemsApiResponse = ApiResponse<WorkItemData[]>

export type GetWorkItemTypesSuccessResponse = ApiSuccessResponse<WorkItemTypeData[]>
export type GetWorkItemTypesApiResponse = ApiResponse<WorkItemTypeData[]>

export type GetWorkItemStatusesSuccessResponse = ApiSuccessResponse<WorkItemStatusData[]>
export type GetWorkItemStatusesApiResponse = ApiResponse<WorkItemStatusData[]>

export type CreateWorkItemSuccessResponse = ApiSuccessResponse<WorkItemDetailData>
export type CreateWorkItemApiResponse = ApiResponse<WorkItemDetailData>

export type GetWorkItemDetailSuccessResponse = ApiSuccessResponse<WorkItemDetailData>
export type GetWorkItemDetailApiResponse = ApiResponse<WorkItemDetailData>

export type UpdateWorkItemSuccessResponse = ApiSuccessResponse<WorkItemDetailData>
export type UpdateWorkItemApiResponse = ApiResponse<WorkItemDetailData>
