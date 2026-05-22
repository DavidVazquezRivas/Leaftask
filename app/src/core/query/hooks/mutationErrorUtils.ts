import { isAxiosError } from 'axios'

import { ApiError } from '@/core/api/global/errors/ApiError'

export const isForbiddenError = (error: unknown): boolean => {
  if (error instanceof ApiError) return error.status === 403
  return isAxiosError(error) && error.response?.status === 403
}

export const isOwnerProtectionError = (error: unknown): boolean => {
  if (!(error instanceof ApiError)) return false
  return error.code.toLowerCase().includes('owner')
}
