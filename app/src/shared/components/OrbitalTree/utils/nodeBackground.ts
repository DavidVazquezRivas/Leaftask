/**
 * Returns a very dark version of the given hex color that preserves its hue,
 * by scaling all channels so the dominant one lands at ~38 (≈15% brightness).
 */
export function nodeBackground(hex: string): string {
  if (hex.length < 7 || hex[0] !== '#') return '#0d0d1a'
  const r = parseInt(hex.slice(1, 3), 16)
  const g = parseInt(hex.slice(3, 5), 16)
  const b = parseInt(hex.slice(5, 7), 16)
  const max = Math.max(r, g, b)
  if (max === 0) return '#0d0d1a'
  const scale = 38 / max
  const fmt = (v: number) =>
    Math.round(v * scale)
      .toString(16)
      .padStart(2, '0')
  return `#${fmt(r)}${fmt(g)}${fmt(b)}`
}
