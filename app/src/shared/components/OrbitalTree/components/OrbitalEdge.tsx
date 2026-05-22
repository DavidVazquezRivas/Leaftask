interface OrbitalEdgeProps {
  id: string
  x1: number
  y1: number
  x2: number
  y2: number
  color: string
}

export function OrbitalEdge({ x1, y1, x2, y2, color }: OrbitalEdgeProps) {
  const dx = Math.abs(x2 - x1) * 0.45
  const d = `M ${x1},${y1} C ${x1 + dx},${y1} ${x2 - dx},${y2} ${x2},${y2}`

  return (
    <g>
      {/* Wide atmospheric glow */}
      <path
        d={d}
        fill="none"
        stroke={color}
        strokeWidth={8}
        strokeOpacity={0.07}
      />
      {/* Soft mid-glow */}
      <path
        d={d}
        fill="none"
        stroke={color}
        strokeWidth={3}
        strokeOpacity={0.18}
      />
      {/* Crisp signal line */}
      <path
        d={d}
        fill="none"
        stroke={color}
        strokeWidth={1.2}
        strokeOpacity={0.6}
      />
    </g>
  )
}
