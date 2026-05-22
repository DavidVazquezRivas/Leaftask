/**
 * Dark mode: very dark tint of the color (~15% brightness).
 * Light mode: very light tint of the color (~85% white blend).
 */
export function nodeBackground(hex: string, isDark: boolean): string {
  if (hex.length < 7 || hex[0] !== '#') return isDark ? '#0d0d1a' : '#f4f4f8'
  const r = parseInt(hex.slice(1, 3), 16)
  const g = parseInt(hex.slice(3, 5), 16)
  const b = parseInt(hex.slice(5, 7), 16)

  if (isDark) {
    const max = Math.max(r, g, b)
    if (max === 0) return '#0d0d1a'
    const scale = 38 / max
    const fmt = (v: number) => Math.round(v * scale).toString(16).padStart(2, '0')
    return `#${fmt(r)}${fmt(g)}${fmt(b)}`
  } else {
    // Blend 18% color + 82% white
    const blend = 0.18
    const fmt = (v: number) =>
      Math.round(v * blend + 255 * (1 - blend)).toString(16).padStart(2, '0')
    return `#${fmt(r)}${fmt(g)}${fmt(b)}`
  }
}
