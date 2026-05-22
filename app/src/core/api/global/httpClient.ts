import axios from 'axios'

import { setupApiInterceptors } from '@/core/api/global/interceptors'
import { Environment } from '@/shared/constants/Environment'

export const apiClient = axios.create({
  baseURL: Environment.API_BASE_URL,
  withCredentials: true,
})

export const authClient = axios.create({
  baseURL: Environment.API_BASE_URL,
  withCredentials: true,
})

setupApiInterceptors(apiClient)
