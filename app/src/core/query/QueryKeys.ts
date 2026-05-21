const workItemAll = ['workitem'] as const
const workItemManagementAll = [...workItemAll, 'management'] as const
const workItemConfigAll = [...workItemAll, 'config'] as const
const workItemWorkLogsAll = [...workItemAll, 'work-logs'] as const
const workItemCommentsAll = [...workItemAll, 'comments'] as const

const projectAll = ['project'] as const
const projectManagementAll = [...projectAll, 'management'] as const
const projectRolesAll = [...projectAll, 'roles'] as const
const projectMembersAll = [...projectAll, 'members'] as const
const projectInvitationsAll = [...projectAll, 'invitations'] as const
const projectCustomFieldsAll = [...projectAll, 'custom-fields'] as const

const organizationAll = ['organization'] as const
const organizationManagementAll = [...organizationAll, 'management'] as const

export const QueryKeys = {
  workItem: {
    all: workItemAll,
    management: {
      all: workItemManagementAll,
      list: (projectId: string) =>
        [...workItemManagementAll, 'list', projectId] as const,
      detail: (projectId: string, itemId: string) =>
        [...workItemManagementAll, 'detail', projectId, itemId] as const,
    },
    config: {
      all: workItemConfigAll,
      types: [...workItemConfigAll, 'types'] as const,
      statuses: [...workItemConfigAll, 'statuses'] as const,
    },
    workLogs: {
      all: workItemWorkLogsAll,
      list: (projectId: string, itemId: string) =>
        [...workItemWorkLogsAll, 'list', projectId, itemId] as const,
    },
    comments: {
      all: workItemCommentsAll,
      list: (projectId: string, itemId: string) =>
        [...workItemCommentsAll, 'list', projectId, itemId] as const,
    },
  },
  project: {
    all: projectAll,
    management: {
      all: projectManagementAll,
      myProjects: (params: { limit?: number; sort?: string[] } = {}) =>
        [...projectManagementAll, 'my-projects', params] as const,
      orgProjects: (
        organizationId: string,
        params: { limit?: number; sort?: string[] } = {}
      ) =>
        [
          ...projectManagementAll,
          'org-projects',
          organizationId,
          params,
        ] as const,
      detail: (projectId: string) =>
        [...projectManagementAll, 'detail', projectId] as const,
    },
    roles: {
      all: projectRolesAll,
      list: (projectId: string) =>
        [...projectRolesAll, 'list', projectId] as const,
      permissions: {
        all: [...projectRolesAll, 'permissions'] as const,
        list: (projectId: string) =>
          [...projectRolesAll, 'permissions', 'list', projectId] as const,
      },
    },
    members: {
      all: projectMembersAll,
      list: (projectId: string, params: { limit?: number } = {}) =>
        [...projectMembersAll, 'list', projectId, params] as const,
    },
    invitations: {
      all: projectInvitationsAll,
      pending: (projectId: string) =>
        [...projectInvitationsAll, 'pending', projectId] as const,
    },
    customFields: {
      all: projectCustomFieldsAll,
      fieldTypes: [...projectCustomFieldsAll, 'field-types'] as const,
      list: (projectId: string) =>
        [...projectCustomFieldsAll, 'list', projectId] as const,
    },
  },
  organization: {
    all: organizationAll,
    management: {
      all: organizationManagementAll,
      detail: (organizationId: string) =>
        [...organizationManagementAll, 'detail', organizationId] as const,
      list: (params: { limit?: number; sort?: string[] } = {}) =>
        [...organizationManagementAll, 'list', params] as const,
      permissions: {
        all: [...organizationManagementAll, 'permissions'] as const,
        me: (organizationId: string) =>
          [
            ...organizationManagementAll,
            'permissions',
            'me',
            organizationId,
          ] as const,
      },
    },
    roles: {
      all: [...organizationAll, 'roles'] as const,
      list: (organizationId: string) =>
        [...organizationAll, 'roles', 'list', organizationId] as const,
      permissions: {
        all: [...organizationAll, 'roles', 'permissions'] as const,
        list: () =>
          [...organizationAll, 'roles', 'permissions', 'list'] as const,
      },
    },
    members: {
      all: [...organizationAll, 'members'] as const,
      list: (organizationId: string, params: { limit?: number } = {}) =>
        [
          ...organizationAll,
          'members',
          'list',
          organizationId,
          params,
        ] as const,
      distribution: {
        all: [...organizationAll, 'members', 'distribution'] as const,
        byOrganization: (organizationId: string) =>
          [
            ...organizationAll,
            'members',
            'distribution',
            organizationId,
          ] as const,
      },
    },
    invitations: {
      all: [...organizationAll, 'invitations'] as const,
      create: (organizationId: string) =>
        [...organizationAll, 'invitations', 'create', organizationId] as const,
      pending: (organizationId: string) =>
        [...organizationAll, 'invitations', 'pending', organizationId] as const,
    },
  },
  user: {
    users: {
      all: ['user', 'users'] as const,
      list: (params: { limit?: number; search?: string } = {}) =>
        [...QueryKeys.user.users.all, 'list', params] as const,
    },
    session: {
      all: ['user', 'session'] as const,
      refresh: () => [...QueryKeys.user.session.all, 'refresh'] as const,
      me: () => [...QueryKeys.user.session.all, 'me'] as const,
      loginWithGoogle: () =>
        [...QueryKeys.user.session.all, 'login-with-google'] as const,
    },
  },
} as const
