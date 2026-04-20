const organizationAll = ['organization'] as const
const organizationManagementAll = [...organizationAll, 'management'] as const

export const QueryKeys = {
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
