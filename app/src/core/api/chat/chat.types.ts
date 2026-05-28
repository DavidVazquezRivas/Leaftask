export type ChatType = 'person' | 'agent'
export type MessageStatus = 'pending' | 'sent' | 'delivered' | 'read'

export interface ChatLastMessageData {
  content: string
  timestamp: string
  status: MessageStatus
}

export interface ChatData {
  id: string
  name: string
  type: ChatType
  lastMessage: ChatLastMessageData | null
  otherParticipantId: string | null
  unreadCount: number
}

export interface MessageSenderData {
  id: string
  name: string
  type: ChatType
}

export interface ChatMessageData {
  id: string
  chatId: string
  content: string
  timestamp: string
  status: MessageStatus
  sender: MessageSenderData | null
}

export interface CreateChatRequest {
  otherParticipantId: string
  otherParticipantType: ChatType
}

export interface SendMessageRequest {
  content: string
}
