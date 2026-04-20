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
    Roles: {
      PermissionsList: 'api/v1/organizations/permissions',
      List: (organizationId: string) =>
        `api/v1/organizations/${organizationId}/roles`,
      Create: (organizationId: string) =>
        `api/v1/organizations/${organizationId}/roles`,
      Update: (organizationId: string, roleId: string) =>
        `api/v1/organizations/${organizationId}/roles/${roleId}`,
      Delete: (organizationId: string, roleId: string) =>
        `api/v1/organizations/${organizationId}/roles/${roleId}`,
    },
    Members: {
      List: (organizationId: string) =>
        `api/v1/organizations/${organizationId}/members`,
      Distribution: (organizationId: string) =>
        `api/v1/organizations/${organizationId}/members/distribution`,
      Update: (organizationId: string, memberId: string) =>
        `api/v1/organizations/${organizationId}/members/${memberId}`,
      Delete: (organizationId: string, memberId: string) =>
        `api/v1/organizations/${organizationId}/members/${memberId}`,
    },
    Invitations: {
      Create: (organizationId: string) =>
        `api/v1/organizations/${organizationId}/invitations`,
      Pending: (organizationId: string) =>
        `api/v1/organizations/${organizationId}/invitations/pending`,
      Update: (organizationId: string, invitationId: string) =>
        `api/v1/organizations/${organizationId}/invitations/${invitationId}`,
    },
  },
  User: {
    Users: {
      List: 'api/v1/users',
    },
    Session: {
      Refresh: 'api/v1/session/refresh',
      Logout: 'api/v1/session/logout',
      OAuthGoogle: 'api/v1/session/oauth/google',
      Me: 'api/v1/session/me',
    },
  },
} as const
