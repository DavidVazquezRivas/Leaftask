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
    CustomFields: {
      FieldTypes: 'api/v1/projects/field-types',
      List: (projectId: string) => `api/v1/projects/${projectId}/fields`,
      Create: (projectId: string) => `api/v1/projects/${projectId}/fields`,
      Update: (projectId: string, fieldId: string) =>
        `api/v1/projects/${projectId}/fields/${fieldId}`,
      Delete: (projectId: string, fieldId: string) =>
        `api/v1/projects/${projectId}/fields/${fieldId}`,
    },
  },
  WorkItem: {
    Management: {
      List: (projectId: string) => `api/v1/workitems/${projectId}`,
      Create: (projectId: string) => `api/v1/workitems/${projectId}`,
      Detail: (projectId: string, itemId: string) =>
        `api/v1/workitems/${projectId}/${itemId}`,
      Update: (projectId: string, itemId: string) =>
        `api/v1/workitems/${projectId}/${itemId}`,
      Delete: (projectId: string, itemId: string) =>
        `api/v1/workitems/${projectId}/${itemId}`,
    },
    WorkLogs: {
      List: (projectId: string, itemId: string) =>
        `api/v1/workitems/${projectId}/${itemId}/work-logs`,
      Create: (projectId: string, itemId: string) =>
        `api/v1/workitems/${projectId}/${itemId}/work-logs`,
      Update: (projectId: string, itemId: string, logId: string) =>
        `api/v1/workitems/${projectId}/${itemId}/work-logs/${logId}`,
      Delete: (projectId: string, itemId: string, logId: string) =>
        `api/v1/workitems/${projectId}/${itemId}/work-logs/${logId}`,
    },
    Attachments: {
      Upload: (projectId: string, itemId: string) =>
        `api/v1/projects/${projectId}/work-items/${itemId}/attachments`,
      Delete: (projectId: string, itemId: string, attachmentId: string) =>
        `api/v1/projects/${projectId}/work-items/${itemId}/attachments/${attachmentId}`,
      PresignedUpload: (projectId: string, itemId: string, fileName: string) =>
        `api/v1/projects/${projectId}/work-items/${itemId}/attachments/presigned-upload?fileName=${encodeURIComponent(fileName)}`,
    },
    Comments: {
      List: (projectId: string, itemId: string) =>
        `api/v1/projects/${projectId}/work-items/${itemId}/comments`,
      Create: (projectId: string, itemId: string) =>
        `api/v1/projects/${projectId}/work-items/${itemId}/comments`,
      Update: (projectId: string, itemId: string, commentId: string) =>
        `api/v1/projects/${projectId}/work-items/${itemId}/comments/${commentId}`,
      Delete: (projectId: string, itemId: string, commentId: string) =>
        `api/v1/projects/${projectId}/work-items/${itemId}/comments/${commentId}`,
    },
    Configuration: {
      Types: 'api/v1/workitems/types',
      Statuses: 'api/v1/workitems/statuses',
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
