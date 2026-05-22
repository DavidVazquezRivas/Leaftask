import { ApiRoutes } from '@/core/api/global/constants/routes'
import { ApiError } from '@/core/api/global/errors'
import { apiClient } from '@/core/api/global/httpClient'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  GetUsersApiResponse,
  GetUsersParams,
  GetUsersSuccessResponse,
} from '@/core/api/user/users/users.types'

const serializeUsersParams = (params: GetUsersParams): string => {
  const searchParams = new URLSearchParams()

  if (typeof params.limit === 'number') {
    searchParams.set('limit', String(params.limit))
  }

  if (params.cursor) {
    searchParams.set('cursor', params.cursor)
  }

  if (params.search && params.search.trim().length > 0) {
    searchParams.set('search', params.search.trim())
  }

  return searchParams.toString()
}

export class UsersGateway {
  static async getUsers(
    params: GetUsersParams = {}
  ): Promise<GetUsersSuccessResponse> {
    const response = await apiClient.get<GetUsersApiResponse>(
      ApiRoutes.User.Users.List,
      {
        params,
        paramsSerializer: () => serializeUsersParams(params),
      }
    )

    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload)) {
      throw new ApiError('Users.List.InvalidResponse', {
        message: 'Users list response is invalid',
      })
    }

    return payload
  }
}
