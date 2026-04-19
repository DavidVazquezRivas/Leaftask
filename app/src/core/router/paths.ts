export const AppPaths = {
  ROOT: '/',
  LOGIN: '/login',
  APP_HOME: '/app',
  APP_PROFILE: '/app/profile',
  APP_ORGANIZATION: '/app/organizations/:organizationId',
  APP_ORGANIZATION_SETTINGS: '/app/organizations/:organizationId/settings',
  organization: (organizationId: string) =>
    `/app/organizations/${organizationId}`,
  organizationSettings: (organizationId: string) =>
    `/app/organizations/${organizationId}/settings`,
} as const
