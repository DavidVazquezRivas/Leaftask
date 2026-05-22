import { GoogleLogin } from '@react-oauth/google'
import { useNavigate } from 'react-router-dom'

import { setAccessToken } from '@/core/auth/sessionSelectors'
import { useAppTranslation } from '@/core/i18n'
import { useGoogleOAuthLoginMutation } from '@/core/query/user/session/mutations'
import { AppPaths } from '@/core/router/paths'
import { Environment } from '@/shared/constants/Environment'

interface GoogleOAuthLoginButtonProps {
  disabled?: boolean
}

export function GoogleOAuthLoginButton({
  disabled = false,
}: GoogleOAuthLoginButtonProps) {
  const navigate = useNavigate()
  const { t } = useAppTranslation('user')
  const googleOAuthLogin = useGoogleOAuthLoginMutation()

  const isDisabled = disabled || googleOAuthLogin.isPending

  if (!Environment.GOOGLE_CLIENT_ID) {
    return (
      <p className="text-sm text-destructive">
        {t('login.googleClientIdMissing')}
      </p>
    )
  }

  return (
    <div className="flex w-full justify-center">
      <GoogleLogin
        onSuccess={async (response) => {
          if (!response.credential || isDisabled) {
            return
          }

          const accessToken = await googleOAuthLogin.mutateAsync(
            response.credential
          )
          setAccessToken(accessToken)
          navigate(AppPaths.APP_HOME, { replace: true })
        }}
        onError={() => {
          googleOAuthLogin.reset()
        }}
        text="signin_with"
        shape="pill"
        theme="outline"
        size="large"
        useOneTap={false}
      />
    </div>
  )
}
