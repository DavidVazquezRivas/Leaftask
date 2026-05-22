import type { ReactNode } from 'react'

interface SecondaryPanelProps {
  title: string
  subtitle?: ReactNode
  titleIcon: ReactNode
  content: ReactNode
  primaryAction?: ReactNode
  footer?: ReactNode
}

export function SecondaryPanel({
  title,
  subtitle,
  titleIcon,
  content,
  primaryAction,
  footer,
}: SecondaryPanelProps) {
  return (
    <aside className="h-full w-76 shrink-0 border-r bg-card/95">
      <div className="flex h-full flex-col overflow-hidden">
        <header className="flex min-h-16 items-center border-b px-5 py-3">
          <div className="flex min-w-0 items-center gap-3">
            <div className="grid size-8 place-items-center rounded-md bg-muted text-muted-foreground">
              {titleIcon}
            </div>
            <div className="min-w-0">
              <p className="truncate text-sm font-semibold">{title}</p>
              {subtitle ? (
                <p className="truncate text-xs text-muted-foreground">
                  {subtitle}
                </p>
              ) : null}
            </div>
          </div>
        </header>

        <section className="min-h-0 flex-1 overflow-y-auto px-5 py-5">
          {content}
        </section>

        {primaryAction ? (
          <div className="border-t px-4 py-4">{primaryAction}</div>
        ) : null}

        {footer ? (
          <footer className="border-t px-5 py-4">{footer}</footer>
        ) : null}
      </div>
    </aside>
  )
}
