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
  },
  user: {
    session: {
      all: ['user', 'session'] as const,
      refresh: () => [...QueryKeys.user.session.all, 'refresh'] as const,
      me: () => [...QueryKeys.user.session.all, 'me'] as const,
      loginWithGoogle: () =>
        [...QueryKeys.user.session.all, 'login-with-google'] as const,
    },
  },
} as const
