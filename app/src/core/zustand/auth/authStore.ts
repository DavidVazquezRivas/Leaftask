import { create } from 'zustand'

interface AuthState {
  accessToken: string | null
  isBootstrapped: boolean
  setAccessToken: (token: string) => void
  clearSession: () => void
  setBootstrapped: (value: boolean) => void
}

export const useAuthStore = create<AuthState>((set) => ({
  accessToken: null,
  isBootstrapped: false,
  setAccessToken: (token) => set({ accessToken: token }),
  clearSession: () => set({ accessToken: null }),
  setBootstrapped: (value) => set({ isBootstrapped: value }),
}))
