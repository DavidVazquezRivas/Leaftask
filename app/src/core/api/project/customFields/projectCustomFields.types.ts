import type {
  ApiResponse,
  ApiSuccessResponse,
} from '@/core/api/global/types/response'

export interface FieldTypeData {
  id: string
  name: string
  description: string
}

export interface CustomFieldOptionData {
  id: string
  name: string
}

export interface CustomFieldData {
  id: string
  name: string
  type: string
  options: CustomFieldOptionData[]
  required: boolean
  appliesTo: string[]
}

export interface CreateCustomFieldRequest {
  name: string
  type: string
  options: string[]
  required: boolean
  appliesTo: string[]
}

export interface PatchCustomFieldRequest {
  name?: string
  type?: string
  options?: string[]
  required?: boolean
  appliesTo?: string[]
}

export type GetFieldTypesSuccessResponse = ApiSuccessResponse<FieldTypeData[]>
export type GetFieldTypesApiResponse = ApiResponse<FieldTypeData[]>

export type GetCustomFieldsSuccessResponse = ApiSuccessResponse<CustomFieldData[]>
export type GetCustomFieldsApiResponse = ApiResponse<CustomFieldData[]>

export type CreateCustomFieldSuccessResponse = ApiSuccessResponse<CustomFieldData>
export type CreateCustomFieldApiResponse = ApiResponse<CustomFieldData>

export type PatchCustomFieldSuccessResponse = ApiSuccessResponse<CustomFieldData>
export type PatchCustomFieldApiResponse = ApiResponse<CustomFieldData>

export type DeleteCustomFieldSuccessResponse = ApiSuccessResponse<null>
export type DeleteCustomFieldApiResponse = ApiResponse<null>
