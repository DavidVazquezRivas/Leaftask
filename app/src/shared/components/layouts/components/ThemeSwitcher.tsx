import { Moon, Sun } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'

import { useAppTranslation } from '@/core/i18n'
import { Button } from '@/shared/components/ui/button'

type ThemeMode = 'light' | 'dark'

const THEME_STORAGE_KEY = 'leaftask-theme'

function applyTheme(theme: ThemeMode) {
  document.documentElement.classList.toggle('dark', theme === 'dark')
}

function useThemeMode() {
  const [theme, setTheme] = useState<ThemeMode>(() => {
    const storedTheme = localStorage.getItem(THEME_STORAGE_KEY)
    return storedTheme === 'dark' ? 'dark' : 'light'
  })

  useEffect(() => {
    applyTheme(theme)
    localStorage.setItem(THEME_STORAGE_KEY, theme)
  }, [theme])

  const toggleTheme = () => {
    setTheme((previousTheme) => (previousTheme === 'dark' ? 'light' : 'dark'))
  }

  return { theme, toggleTheme }
}

export function ThemeSwitcher() {
  const { t } = useAppTranslation('global')
  const { theme, toggleTheme } = useThemeMode()

  const label = useMemo(() => {
    return theme === 'dark'
      ? t('publicLayout.themeDark')
      : t('publicLayout.themeLight')
  }, [theme, t])

  return (
    <Button
      size="sm"
      variant="outline"
      data-icon="inline-start"
      onClick={toggleTheme}
    >
      {theme === 'dark' ? <Moon /> : <Sun />}
      {label}
    </Button>
  )
}

export function PrivateThemeToggle() {
  const { t } = useAppTranslation('global')
  const { theme, toggleTheme } = useThemeMode()

  return (
    <Button
      size="icon-sm"
      variant="ghost"
      onClick={toggleTheme}
      aria-label={t('privateLayout.theme')}
      title={t('privateLayout.theme')}
    >
      {theme === 'dark' ? <Moon /> : <Sun />}
    </Button>
  )
}
