import type { AxiosError, AxiosInstance } from 'axios'

import { ApiRoutes } from '@/core/api/global/constants/routes'
import { ApiGateway } from '@/core/api/ApiGateway'
import { clearSession, setAccessToken } from '@/core/auth/sessionSelectors'
import type { RetriableRequestConfig } from '@/core/api/global/interceptors/types'

let refreshPromise: Promise<string> | null = null

const getRefreshTokenOnce = async (): Promise<string> => {
  if (refreshPromise) {
    return refreshPromise
  }

  refreshPromise = (async () => {
    try {
      const token = await ApiGateway.user.session.refreshSession()
      setAccessToken(token)
      return token
    } catch (error) {
      clearSession()
      throw error
    } finally {
      refreshPromise = null
    }
  })()

  return refreshPromise
}

export const setupRefreshResponseInterceptor = (
  apiClient: AxiosInstance
): void => {
  apiClient.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
      const requestConfig = error.config as RetriableRequestConfig | undefined

      if (!requestConfig) {
        return Promise.reject(error)
      }

      const isUnauthorized = error.response?.status === 401
      const isRefreshRequest = requestConfig.url?.includes(
        ApiRoutes.User.Session.Refresh
      )

      if (
        !isUnauthorized ||
        requestConfig._retry ||
        requestConfig.skipAuthRefresh ||
        isRefreshRequest
      ) {
        return Promise.reject(error)
      }

      requestConfig._retry = true

      try {
        const refreshedToken = await getRefreshTokenOnce()
        requestConfig.headers.set('Authorization', `Bearer ${refreshedToken}`)
        return apiClient(requestConfig)
      } catch (refreshError) {
        clearSession()
        return Promise.reject(refreshError)
      }
    }
  )
}
