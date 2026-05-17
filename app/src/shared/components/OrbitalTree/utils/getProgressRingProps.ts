export interface ProgressRingProps {
  strokeDasharray: string
  strokeDashoffset: number
}

export const getProgressRingProps = (
  filled: number,
  radius: number
): ProgressRingProps => {
  const circumference = 2 * Math.PI * radius
  const clamped = Math.min(100, Math.max(0, filled))
  const dash = (clamped / 100) * circumference
  return {
    strokeDasharray: `${dash} ${circumference}`,
    strokeDashoffset: circumference * 0.25,
  }
}
