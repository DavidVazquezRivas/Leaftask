import { useAuthStore } from '@/core/zustand/auth/authStore'

export const getAccessToken = (): string | null => {
  return useAuthStore.getState().accessToken
}

export const isAuthenticated = (): boolean => {
  return Boolean(getAccessToken())
}

export const setAccessToken = (token: string): void => {
  useAuthStore.getState().setAccessToken(token)
}

export const clearSession = (): void => {
  useAuthStore.getState().clearSession()
}
