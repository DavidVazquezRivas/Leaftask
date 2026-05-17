export type WorkItemStatus = 'backlog' | 'in-progress' | 'done' | 'blocked'
export type WorkItemType = 'task' | 'bug'
export type WorkItemPriority = 'low' | 'medium' | 'high' | 'critical'

export interface WorkItemMock {
  id: string
  code: string
  title: string
  estimation: number
  progress: number
  assignee: { id: string; fullName: string } | null
  parentId: string | null
  statusId: WorkItemStatus
  type: WorkItemType
  priority: WorkItemPriority
}

export const workItemsMock: WorkItemMock[] = [
  {
    id: 'AUR-100',
    code: 'AUR-100',
    title: 'Authentication System',
    estimation: 44,
    progress: 0.75,
    assignee: { id: '1', fullName: 'Marco García' },
    parentId: null,
    statusId: 'in-progress',
    type: 'task',
    priority: 'critical',
  },
  {
    id: 'AUR-110',
    code: 'AUR-110',
    title: 'OAuth Integration',
    estimation: 16,
    progress: 0.8,
    assignee: { id: '2', fullName: 'Carlos Ruiz' },
    parentId: 'AUR-100',
    statusId: 'in-progress',
    type: 'task',
    priority: 'high',
  },
  {
    id: 'AUR-111',
    code: 'AUR-111',
    title: 'Session Management',
    estimation: 28,
    progress: 1.0,
    assignee: { id: '3', fullName: 'Ana López' },
    parentId: 'AUR-100',
    statusId: 'done',
    type: 'task',
    priority: 'medium',
  },
  {
    id: 'AUR-120',
    code: 'AUR-120',
    title: 'Google OAuth',
    estimation: 8,
    progress: 0.3,
    assignee: { id: '4', fullName: 'Pedro Martín' },
    parentId: 'AUR-110',
    statusId: 'in-progress',
    type: 'task',
    priority: 'high',
  },
  {
    id: 'AUR-121',
    code: 'AUR-121',
    title: 'Token Refresh',
    estimation: 8,
    progress: 0.6,
    assignee: null,
    parentId: 'AUR-110',
    statusId: 'in-progress',
    type: 'task',
    priority: 'medium',
  },
  {
    id: 'AUR-122',
    code: 'AUR-122',
    title: 'Fix Login Bug',
    estimation: 4,
    progress: 0.2,
    assignee: { id: '5', fullName: 'Jorge Díaz' },
    parentId: 'AUR-110',
    statusId: 'blocked',
    type: 'bug',
    priority: 'critical',
  },
  {
    id: 'AUR-200',
    code: 'AUR-200',
    title: 'Dashboard UI',
    estimation: 36,
    progress: 0.1,
    assignee: null,
    parentId: null,
    statusId: 'backlog',
    type: 'task',
    priority: 'low',
  },
  {
    id: 'AUR-210',
    code: 'AUR-210',
    title: 'Analytics Cards',
    estimation: 12,
    progress: 0.0,
    assignee: null,
    parentId: 'AUR-200',
    statusId: 'backlog',
    type: 'task',
    priority: 'low',
  },
  {
    id: 'AUR-211',
    code: 'AUR-211',
    title: 'Real-time Updates',
    estimation: 12,
    progress: 0.0,
    assignee: null,
    parentId: 'AUR-200',
    statusId: 'backlog',
    type: 'task',
    priority: 'low',
  },
]
