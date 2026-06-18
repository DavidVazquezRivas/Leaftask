import type { AxiosError, AxiosInstance } from 'axios'

import { ApiError } from '@/core/api/global/errors'
import { isApiErrorResponse } from '@/core/api/global/types/response'

export const setupApiErrorInterceptor = (apiClient: AxiosInstance): void => {
  apiClient.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
      const data = error.response?.data
      if (isApiErrorResponse(data)) {
        throw new ApiError(data.error.code, {
          message: data.error.message,
          status: error.response?.status,
          meta: data.meta,
        })
      }
      return Promise.reject(error)
    }
  )
}
