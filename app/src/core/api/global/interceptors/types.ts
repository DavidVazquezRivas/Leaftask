import type { InternalAxiosRequestConfig } from 'axios'

export type RetriableRequestConfig = InternalAxiosRequestConfig & {
  _retry?: boolean
  skipAuthRefresh?: boolean
  skipAuthorization?: boolean
}
