import { Languages } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'

import { resources, useAppTranslation } from '@/core/i18n'
import { Button } from '@/shared/components/ui/button'

const supportedLocales = Object.keys(resources)

const localeFlags: Record<string, string> = {
  en: '🇬🇧',
  es: '🇪🇸',
}

export function PrivateLanguageSwitcher() {
  const { i18n, t } = useAppTranslation('global')
  const [isOpen, setIsOpen] = useState(false)
  const containerRef = useRef<HTMLDivElement | null>(null)
  const activeLanguage = (i18n.resolvedLanguage ?? i18n.language).split('-')[0]

  useEffect(() => {
    const onDocumentPointerDown = (event: MouseEvent) => {
      const target = event.target

      if (!(target instanceof Node)) {
        return
      }

      if (!containerRef.current?.contains(target)) {
        setIsOpen(false)
      }
    }

    document.addEventListener('mousedown', onDocumentPointerDown)

    return () => {
      document.removeEventListener('mousedown', onDocumentPointerDown)
    }
  }, [])

  return (
    <div ref={containerRef} className="relative">
      <Button
        size="icon-sm"
        variant="ghost"
        aria-label={t('privateLayout.language')}
        title={t('privateLayout.language')}
        onClick={() => {
          setIsOpen((previousValue) => !previousValue)
        }}
      >
        <Languages />
      </Button>

      {isOpen ? (
        <div className="absolute bottom-0 left-full z-20 ml-2 min-w-24 rounded-md border bg-popover p-1 shadow-md">
          {supportedLocales.map((locale) => {
            const isActive = locale === activeLanguage

            return (
              <button
                key={locale}
                type="button"
                className="flex w-full items-center gap-2 rounded-sm px-2 py-1.5 text-sm hover:bg-accent hover:text-accent-foreground"
                aria-label={`${t('privateLayout.language')}: ${locale.toUpperCase()}`}
                onClick={() => {
                  void i18n.changeLanguage(locale)
                  setIsOpen(false)
                }}
              >
                <span>{localeFlags[locale] ?? '🌐'}</span>
                <span>{locale.toUpperCase()}</span>
                {isActive ? (
                  <span className="ml-auto text-xs text-muted-foreground">
                    •
                  </span>
                ) : null}
              </button>
            )
          })}
        </div>
      ) : null}
    </div>
  )
}
