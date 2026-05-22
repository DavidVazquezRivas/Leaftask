import type { AxiosInstance } from 'axios'

import { getAccessToken } from '@/core/auth/sessionSelectors'
import type { RetriableRequestConfig } from '@/core/api/global/interceptors/types'

export const setupAuthorizationRequestInterceptor = (
  apiClient: AxiosInstance
): void => {
  apiClient.interceptors.request.use((config) => {
    const requestConfig = config as RetriableRequestConfig

    if (requestConfig.skipAuthorization) {
      return requestConfig
    }

    const accessToken = getAccessToken()

    if (accessToken) {
      requestConfig.headers.set('Authorization', `Bearer ${accessToken}`)
    }

    return requestConfig
  })
}
