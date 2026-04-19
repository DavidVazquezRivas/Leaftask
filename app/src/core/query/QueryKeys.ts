export const QueryKeys = {
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
