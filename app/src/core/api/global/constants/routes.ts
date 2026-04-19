export const ApiRoutes = {
  Organization: {
    Management: {
      List: 'api/v1/organizations',
      Create: 'api/v1/organizations',
      Detail: (organizationId: string) =>
        `api/v1/organizations/${organizationId}`,
      Permissions: (organizationId: string) =>
        `api/v1/organizations/${organizationId}/permissions/me`,
      Update: (organizationId: string) =>
        `api/v1/organizations/${organizationId}`,
      Delete: (organizationId: string) =>
        `api/v1/organizations/${organizationId}`,
    },
  },
  User: {
    Session: {
      Refresh: 'api/v1/session/refresh',
      Logout: 'api/v1/session/logout',
      OAuthGoogle: 'api/v1/session/oauth/google',
      Me: 'api/v1/session/me',
    },
  },
} as const
