export const createStarPath = (
  cx: number,
  cy: number,
  outerR: number,
  innerR: number,
  numPoints: number
): string => {
  const step = Math.PI / numPoints
  const points: string[] = []
  for (let i = 0; i < numPoints * 2; i++) {
    const r = i % 2 === 0 ? outerR : innerR
    const angle = i * step - Math.PI / 2
    points.push(`${cx + r * Math.cos(angle)},${cy + r * Math.sin(angle)}`)
  }
  return points.join(' ')
}
