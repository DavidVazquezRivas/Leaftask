export interface AgentData {
  id: string
  projectId: string
  name: string
  instructions: string
  templateId: string | null
  createdAt: string
}

export interface CreateAgentRequest {
  projectId: string
  name: string
  instructions: string
  roleId: string
}
