import type { AxiosInstance } from 'axios'

import { setupAuthorizationRequestInterceptor } from '@/core/api/global/interceptors/authorizationRequestInterceptor'
import { setupRefreshResponseInterceptor } from '@/core/api/global/interceptors/refreshResponseInterceptor'
import { setupApiErrorInterceptor } from '@/core/api/global/interceptors/apiErrorInterceptor'

export const setupApiInterceptors = (apiClient: AxiosInstance): void => {
  setupAuthorizationRequestInterceptor(apiClient)
  setupRefreshResponseInterceptor(apiClient)
  setupApiErrorInterceptor(apiClient)
}
