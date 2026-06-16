import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import { isApiErrorResponse, isApiSuccessResponse, type ApiMeta } from '@/core/api/global/types/response'
import type { AgentData, CreateAgentRequest } from './agent.types'

interface AgentApiResponse {
  data?: AgentData
  error?: { code: string; message: string }
  meta?: ApiMeta
}

export class AgentGateway {
  static async create(body: CreateAgentRequest): Promise<AgentData> {
    const response = await apiClient.post<AgentApiResponse>(ApiRoutes.Agent.Create, body)
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data) {
      throw new ApiError('Agent.Create.InvalidResponse', { message: 'Create agent response is invalid' })
    }

    return payload.data
  }

  static async delete(agentId: string, projectId: string): Promise<void> {
    const response = await apiClient.delete<{ error?: { code: string; message: string } }>(
      `${ApiRoutes.Agent.Delete(agentId)}?projectId=${projectId}`
    )
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
      })
    }
  }
}
