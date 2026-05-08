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
  Project: {
    Management: {
      Me: 'api/v1/projects/me',
      Organization: (organizationId: string) =>
        `api/v1/projects/organization/${organizationId}`,
      Create: 'api/v1/projects',
      Detail: (projectId: string) => `api/v1/projects/${projectId}`,
      Update: (projectId: string) => `api/v1/projects/${projectId}`,
      Delete: (projectId: string) => `api/v1/projects/${projectId}`,
    },
    Roles: {
      PermissionsList: (projectId: string) =>
        `api/v1/projects/${projectId}/permissions`,
      List: (projectId: string) => `api/v1/projects/${projectId}/roles`,
      Create: (projectId: string) => `api/v1/projects/${projectId}/roles`,
      Update: (projectId: string, roleId: string) =>
        `api/v1/projects/${projectId}/roles/${roleId}`,
      Delete: (projectId: string, roleId: string) =>
        `api/v1/projects/${projectId}/roles/${roleId}`,
    },
    Members: {
      List: (projectId: string) =>
        `api/v1/projects/${projectId}/members`,
      Update: (projectId: string, memberId: string) =>
        `api/v1/projects/${projectId}/members/${memberId}`,
      Delete: (projectId: string, memberId: string) =>
        `api/v1/projects/${projectId}/members/${memberId}`,
    },
    Invitations: {
      Pending: (projectId: string) =>
        `api/v1/projects/${projectId}/invitations`,
      Create: (projectId: string) =>
        `api/v1/projects/${projectId}/invitations`,
      Update: (projectId: string, invitationId: string) =>
        `api/v1/projects/${projectId}/invitations/${invitationId}`,
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
