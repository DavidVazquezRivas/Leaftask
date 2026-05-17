export const darkenColor = (color: string, factor: number): string => {
  const hex = color.replace('#', '')
  let r: number, g: number, b: number

  if (hex.length === 3) {
    r = parseInt(hex[0] + hex[0], 16)
    g = parseInt(hex[1] + hex[1], 16)
    b = parseInt(hex[2] + hex[2], 16)
  } else if (hex.length === 6) {
    r = parseInt(hex.slice(0, 2), 16)
    g = parseInt(hex.slice(2, 4), 16)
    b = parseInt(hex.slice(4, 6), 16)
  } else {
    return color
  }

  const d = (v: number) => Math.round(v * (1 - factor))
  return `#${d(r).toString(16).padStart(2, '0')}${d(g).toString(16).padStart(2, '0')}${d(b).toString(16).padStart(2, '0')}`
}
