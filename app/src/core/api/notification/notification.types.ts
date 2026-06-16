export interface SimpleReferenceData {
  id: string
  name: string
}

export type NotificationType = 'Assignment' | 'Mention' | 'Invitation' | 'ProjectInvitation' | 'Request'

export interface NotificationData {
  id: string
  type: NotificationType
  context: SimpleReferenceData
  target: SimpleReferenceData
  timestamp: string
  actor: SimpleReferenceData | null
  read: boolean
}

export type ApprovalStatus = 'pending' | 'approved' | 'rejected'

export interface ApprovalCommentData {
  id: string
  content: string
  timestamp: string
  author: SimpleReferenceData
}

export interface ApprovalData {
  id: string
  status: ApprovalStatus
  contextType: 'organization' | 'project'
  permissionName: string
  actionType: string | null
  actionPayload: string | null
  context: SimpleReferenceData
  target: SimpleReferenceData
  requester: SimpleReferenceData
  createdAt: string
  comments: ApprovalCommentData[]
}
