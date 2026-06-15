export const AppPaths = {
  ROOT: '/',
  LOGIN: '/login',
  APP_HOME: '/app',
  APP_PROFILE: '/app/profile',
  APP_ORGANIZATION: '/app/organizations/:organizationId',
  APP_ORGANIZATION_SETTINGS: '/app/organizations/:organizationId/settings',
  APP_PROJECT: '/app/projects/:projectId',
  APP_PROJECT_SETTINGS: '/app/projects/:projectId/settings',
  APP_NOTIFICATIONS: '/app/notifications',
  APP_CHAT: '/app/chats',
  APP_CHAT_DETAIL: '/app/chats/:chatId',
  organization: (organizationId: string) =>
    `/app/organizations/${organizationId}`,
  organizationSettings: (organizationId: string) =>
    `/app/organizations/${organizationId}/settings`,
  project: (projectId: string) => `/app/projects/${projectId}`,
  projectSettings: (projectId: string) => `/app/projects/${projectId}/settings`,
  notifications: () => '/app/notifications',
  chat: () => '/app/chats',
  chatDetail: (chatId: string) => `/app/chats/${chatId}`,
} as const
