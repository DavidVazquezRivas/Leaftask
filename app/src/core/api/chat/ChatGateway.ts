import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import { isApiErrorResponse, isApiSuccessResponse, type ApiMeta } from '@/core/api/global/types/response'
import type { ChatData, ChatMessageData, CreateChatRequest, SendMessageRequest } from './chat.types'

interface ChatApiResponse {
  data?: ChatData
  error?: { code: string; message: string }
  meta?: ApiMeta
}

interface ChatsListApiResponse {
  data?: ChatData[]
  error?: { code: string; message: string }
  meta?: ApiMeta
}

interface ChatMessageApiResponse {
  data?: ChatMessageData
  error?: { code: string; message: string }
  meta?: ApiMeta
}

interface MessagesResponseData {
  items: ChatMessageData[]
  nextCursor: string | null
  hasMore: boolean
}

interface ChatMessagesListApiResponse {
  data?: MessagesResponseData
  error?: { code: string; message: string }
  meta?: ApiMeta
}

export class ChatGateway {
  static async list(): Promise<ChatData[]> {
    const response = await apiClient.get<ChatsListApiResponse>(ApiRoutes.Chat.List)
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload)) {
      throw new ApiError('Chat.List.InvalidResponse', { message: 'Chats list response is invalid' })
    }

    return payload.data ?? []
  }

  static async create(body: CreateChatRequest): Promise<ChatData> {
    const response = await apiClient.post<ChatApiResponse>(ApiRoutes.Chat.Create, body)
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data) {
      throw new ApiError('Chat.Create.InvalidResponse', { message: 'Create chat response is invalid' })
    }

    return payload.data
  }

  static async delete(chatId: string): Promise<void> {
    const response = await apiClient.delete(ApiRoutes.Chat.Delete(chatId))
    const payload = response.data

    if (payload && isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }
  }

  static async pollMessages(): Promise<ChatMessageData[]> {
    const response = await apiClient.get<ChatsListApiResponse & { data?: ChatMessageData[] }>(
      ApiRoutes.Chat.Polling
    )
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload)) {
      throw new ApiError('Chat.Polling.InvalidResponse', { message: 'Polling response is invalid' })
    }

    return (payload.data as ChatMessageData[]) ?? []
  }

  static async listMessages(
    chatId: string,
    params: { limit?: number; cursor?: string | null } = {}
  ): Promise<{ data: ChatMessageData[]; nextCursor: string | null }> {
    const searchParams = new URLSearchParams()
    if (params.limit) searchParams.set('limit', String(params.limit))
    if (params.cursor) searchParams.set('cursor', params.cursor)
    searchParams.append('sort', 'createdAt:asc')

    const response = await apiClient.get<ChatMessagesListApiResponse>(
      ApiRoutes.Chat.Messages.List(chatId),
      { params: {}, paramsSerializer: () => searchParams.toString() }
    )
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload)) {
      throw new ApiError('Chat.Messages.List.InvalidResponse', { message: 'Messages list response is invalid' })
    }

    return {
      data: payload.data?.items ?? [],
      nextCursor: payload.data?.nextCursor ?? payload.meta?.pagination?.nextCursor ?? null,
    }
  }

  static async markAsRead(chatId: string): Promise<void> {
    const response = await apiClient.post(ApiRoutes.Chat.MarkAsRead(chatId))
    const payload = response.data

    if (payload && isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }
  }

  static async sendMessage(chatId: string, body: SendMessageRequest): Promise<ChatMessageData> {
    const response = await apiClient.post<ChatMessageApiResponse>(
      ApiRoutes.Chat.Messages.Create(chatId),
      body
    )
    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data) {
      throw new ApiError('Chat.Messages.Create.InvalidResponse', { message: 'Send message response is invalid' })
    }

    return payload.data
  }
}
