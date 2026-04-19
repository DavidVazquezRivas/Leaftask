import type { ApiMeta } from '@/core/api/global/types/response'

export interface ApiErrorOptions {
  message?: string
  status?: number
  meta?: ApiMeta
  cause?: unknown
}

export class ApiError extends Error {
  readonly code: string
  readonly status?: number
  readonly meta?: ApiMeta

  constructor(code: string, options: ApiErrorOptions = {}) {
    super(options.message ?? code)
    this.name = 'ApiError'
    this.code = code
    this.status = options.status
    this.meta = options.meta
    this.cause = options.cause
  }
}
