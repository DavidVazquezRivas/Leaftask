export type SortDirection = 'asc' | 'desc'

export interface ApiSortItem {
  field: string
  direction: SortDirection
}

export interface ApiPagination {
  limit: number
  nextCursor: string | null
  hasMore: boolean
}

export interface ApiMeta {
  requestId?: string
  timestamp?: string
  sort?: ApiSortItem[]
  pagination?: ApiPagination
  [key: string]: unknown
}

export interface ApiSuccessResponse<TData> {
  data: TData
  meta?: ApiMeta
}

export interface ApiErrorBody {
  code: string
  message: string
}

export interface ApiErrorResponse {
  error: ApiErrorBody
  meta?: ApiMeta
}

export type ApiResponse<TData> = ApiSuccessResponse<TData> | ApiErrorResponse

export const isApiSuccessResponse = <TData>(
  payload: ApiResponse<TData>
): payload is ApiSuccessResponse<TData> => {
  return 'data' in payload
}

export const isApiErrorResponse = <TData>(
  payload: ApiResponse<TData>
): payload is ApiErrorResponse => {
  return 'error' in payload
}
