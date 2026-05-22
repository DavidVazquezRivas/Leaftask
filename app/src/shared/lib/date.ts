export function formatRelative(isoString: string, locale: string): string {
  const diffMs = Date.now() - new Date(isoString).getTime()
  const minutes = Math.floor(diffMs / 60_000)
  const rtf = new Intl.RelativeTimeFormat(locale, { numeric: 'auto' })
  if (minutes < 1) return rtf.format(0, 'seconds')
  if (minutes < 60) return rtf.format(-minutes, 'minutes')
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return rtf.format(-hours, 'hours')
  const days = Math.floor(hours / 24)
  if (days < 7) return rtf.format(-days, 'days')
  return new Date(isoString).toLocaleDateString(locale)
}
