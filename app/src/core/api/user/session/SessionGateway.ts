import axios from 'axios'

import { getAccessToken } from '@/core/auth/sessionSelectors'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import {
  isApiErrorResponse,
  isApiSuccessResponse,
} from '@/core/api/global/types/response'
import type {
  GoogleOAuthRequest,
  GoogleOAuthSessionApiResponse,
  RefreshSessionApiResponse,
  SessionLogoutApiResponse,
  SessionMeApiResponse,
  SessionMeData,
} from '@/core/api/user/session/session.types'
import { Environment } from '@/shared/constants/Environment'

const sessionClient = axios.create({
  baseURL: Environment.API_BASE_URL,
  withCredentials: true,
})

export class SessionGateway {
  static async logoutSession(): Promise<void> {
    const accessToken = getAccessToken()

    try {
      const response = await sessionClient.delete<SessionLogoutApiResponse>(
        ApiRoutes.SESSION_LOGOUT,
        {
          headers: accessToken
            ? {
                Authorization: `Bearer ${accessToken}`,
              }
            : undefined,
        }
      )

      const payload = response.data

      if (isApiErrorResponse(payload)) {
        throw new ApiError(payload.error.code, {
          message: payload.error.message,
          meta: payload.meta,
        })
      }
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        return
      }

      throw error
    }
  }

  static async getCurrentUser(): Promise<SessionMeData> {
    const accessToken = getAccessToken()

    const response = await sessionClient.get<SessionMeApiResponse>(
      ApiRoutes.SESSION_ME,
      {
        headers: accessToken
          ? {
              Authorization: `Bearer ${accessToken}`,
            }
          : undefined,
      }
    )

    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data) {
      throw new ApiError('Session.Me.MissingUserData', {
        message: 'Session me response does not contain user data',
      })
    }

    return payload.data
  }

  static async loginWithGoogle(token: string): Promise<string> {
    const requestPayload: GoogleOAuthRequest = { token }

    const response = await sessionClient.post<GoogleOAuthSessionApiResponse>(
      ApiRoutes.SESSION_OAUTH_GOOGLE,
      requestPayload
    )

    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data.accessToken) {
      throw new ApiError('Session.OAuth.Google.MissingAccessToken', {
        message: 'Google OAuth response does not contain an accessToken',
      })
    }

    return payload.data.accessToken
  }

  static async refreshSession(): Promise<string> {
    const response = await sessionClient.post<RefreshSessionApiResponse>(
      ApiRoutes.SESSION_REFRESH
    )

    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data.accessToken) {
      throw new ApiError('Session.Refresh.MissingAccessToken', {
        message: 'Refresh response does not contain an accessToken',
      })
    }

    return payload.data.accessToken
  }
}
