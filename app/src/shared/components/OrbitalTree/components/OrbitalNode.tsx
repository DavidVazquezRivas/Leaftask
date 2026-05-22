import { useEffect, useState, type CSSProperties } from 'react'

function useIsDark() {
  const [isDark, setIsDark] = useState(() =>
    document.documentElement.classList.contains('dark')
  )
  useEffect(() => {
    const el = document.documentElement
    const obs = new MutationObserver(() =>
      setIsDark(el.classList.contains('dark'))
    )
    obs.observe(el, { attributes: true, attributeFilter: ['class'] })
    return () => obs.disconnect()
  }, [])
  return isDark
}

import {
  ORBITAL_BASE_RADIUS,
  ORBITAL_ORBIT_GAP,
  ORBITAL_RING_PADDING,
} from '../constants'
import type { NodeVisual } from '../types'
import { createStarPath } from '../utils/createStarPath'
import { nodeBackground } from '../utils/nodeBackground'

interface OrbitalNodeProps {
  x: number
  y: number
  visual: NodeVisual
  hasChildren: boolean
  isCollapsed: boolean
  onToggle: () => void
  onClick?: () => void
  onAddChild?: () => void
}

interface RingConfig {
  key: number
  r: number
  dasharray: string | undefined
  dashoffset: number | undefined
  opacity: number
}

const BASE_RADIUS_MIN = 31

// size ∈ [0,1]: linear, 31px (0h) → 150px (80h+)
export function computeBaseRadius(size: number): number {
  return Math.round(
    BASE_RADIUS_MIN +
      Math.min(size, 1) * (ORBITAL_BASE_RADIUS - BASE_RADIUS_MIN)
  )
}

// orbits = ring count (work logged); size = circle scale (estimation)
export function computeTotalRadius(orbits: number, size: number): number {
  return (
    computeBaseRadius(size) +
    Math.ceil(orbits) * ORBITAL_ORBIT_GAP +
    ORBITAL_RING_PADDING
  )
}

function buildRings(orbits: number, baseRadius: number): RingConfig[] {
  const rings: RingConfig[] = []
  const fullOrbits = Math.floor(orbits)
  const fractional = orbits - fullOrbits

  for (let i = 0; i < fullOrbits; i++) {
    rings.push({
      key: i,
      r: baseRadius + (i + 1) * ORBITAL_ORBIT_GAP,
      dasharray: undefined,
      dashoffset: undefined,
      opacity: Math.max(0.2, 0.85 - i * 0.12),
    })
  }

  if (fractional > 0.001) {
    const r = baseRadius + (fullOrbits + 1) * ORBITAL_ORBIT_GAP
    const circ = 2 * Math.PI * r
    rings.push({
      key: fullOrbits,
      r,
      dasharray: `${fractional * circ} ${circ - fractional * circ}`,
      dashoffset: circ * 0.25,
      opacity: Math.max(0.2, 0.85 - fullOrbits * 0.12),
    })
  }

  return rings
}

export function OrbitalNode({
  x,
  y,
  visual,
  hasChildren,
  isCollapsed,
  onToggle,
  onClick,
  onAddChild,
}: OrbitalNodeProps) {
  const isDark = useIsDark()
  const { title, subtitle, color, shape, filled, size, orbits, avatar, over } =
    visual

  const baseRadius = computeBaseRadius(size)
  const diameter = baseRadius * 2
  const rings = buildRings(orbits, baseRadius)
  const totalRadius = computeTotalRadius(orbits, size)
  const totalSize = totalRadius * 2
  const bgColor = nodeBackground(color, isDark)
  const buttonBg = isDark ? '#0a0f1e' : '#ffffff'
  const isComplete = filled >= 100

  const shapeOffset = totalRadius - baseRadius

  const shapeRadius =
    shape === 'circle' ? '50%' : shape === 'square' ? '8px' : '0'
  const shapeClip =
    shape === 'triangle' ? 'polygon(50% 0%, 0% 100%, 100% 100%)' : undefined

  const fillGradient =
    filled <= 0
      ? bgColor
      : filled >= 100
        ? color
        : `linear-gradient(to top, ${color} ${filled}%, ${bgColor} ${filled}%)`

  const outerGlow = isComplete
    ? `0 0 18px ${color}cc, 0 0 36px ${color}66, 0 0 60px ${color}22`
    : `0 0 ${8 + Math.round(filled / 8)}px ${color}66, 0 0 ${20 + Math.round(filled / 4)}px ${color}22`

  const boxShadow = [
    outerGlow,
    isDark ? 'inset 0 0 0 1px rgba(255,255,255,0.10)' : 'inset 0 0 0 1px rgba(0,0,0,0.06)',
    isDark ? 'inset 0 3px 8px rgba(255,255,255,0.07)' : 'inset 0 3px 8px rgba(255,255,255,0.60)',
    isDark ? 'inset 0 -6px 14px rgba(0,0,0,0.45)' : 'inset 0 -6px 14px rgba(0,0,0,0.08)',
  ].join(', ')

  const isImageAvatar =
    avatar.startsWith('http') ||
    avatar.startsWith('/') ||
    avatar.startsWith('data:')

  const starPath = createStarPath(
    baseRadius,
    baseRadius,
    baseRadius - 2,
    (baseRadius - 2) * 0.65,
    8
  )

  return (
    <div
      style={
        {
          position: 'absolute',
          left: x - totalRadius,
          top: y - totalRadius,
          width: totalSize,
          height: totalSize,
          overflow: 'visible',
          transition: 'left 0.3s ease, top 0.3s ease',
          cursor: onClick ? 'pointer' : 'default',
        } as CSSProperties
      }
      onClick={onClick}
    >
      {/* Orbit rings with atmospheric glow */}
      {rings.length > 0 && (
        <svg
          width={totalSize}
          height={totalSize}
          style={{
            position: 'absolute',
            left: 0,
            top: 0,
            pointerEvents: 'none',
            overflow: 'visible',
          }}
        >
          {rings.map((ring) => (
            <g key={ring.key}>
              <circle
                cx={totalRadius}
                cy={totalRadius}
                r={ring.r}
                fill="none"
                stroke={color}
                strokeWidth={11}
                opacity={ring.opacity * 0.11}
                strokeDasharray={ring.dasharray}
                strokeDashoffset={ring.dashoffset}
              />
              <circle
                cx={totalRadius}
                cy={totalRadius}
                r={ring.r}
                fill="none"
                stroke={color}
                strokeWidth={4}
                opacity={ring.opacity * 0.28}
                strokeLinecap={ring.dasharray ? 'round' : undefined}
                strokeDasharray={ring.dasharray}
                strokeDashoffset={ring.dashoffset}
              />
              <circle
                cx={totalRadius}
                cy={totalRadius}
                r={ring.r}
                fill="none"
                stroke={color}
                strokeWidth={1.5}
                opacity={ring.opacity}
                strokeLinecap={ring.dasharray ? 'round' : undefined}
                strokeDasharray={ring.dasharray}
                strokeDashoffset={ring.dashoffset}
              />
            </g>
          ))}
        </svg>
      )}

      {/* Shape + content */}
      <div
        style={{
          position: 'absolute',
          left: shapeOffset,
          top: shapeOffset,
          width: diameter,
          height: diameter,
        }}
      >
        {shape !== 'spike' ? (
          <>
            <div
              style={{
                position: 'absolute',
                inset: 0,
                borderRadius: shapeRadius,
                clipPath: shapeClip,
                background: fillGradient,
                border: `2px solid ${color}`,
                boxShadow,
              }}
            />
            {filled > 4 && filled < 96 && (
              <div
                style={{
                  position: 'absolute',
                  inset: 0,
                  borderRadius: shapeRadius,
                  clipPath: shapeClip,
                  background: `linear-gradient(to top,
                    transparent ${filled - 4}%,
                    rgba(255,255,255,0.22) ${filled - 0.5}%,
                    rgba(255,255,255,0.07) ${filled + 1}%,
                    transparent ${filled + 6}%
                  )`,
                  pointerEvents: 'none',
                }}
              />
            )}
            <div
              style={{
                position: 'absolute',
                inset: 0,
                borderRadius: shapeRadius,
                clipPath: shapeClip,
                background:
                  'radial-gradient(ellipse 65% 38% at 50% 10%, rgba(255,255,255,0.14) 0%, transparent 100%)',
                pointerEvents: 'none',
              }}
            />
          </>
        ) : (
          <div
            style={{
              position: 'absolute',
              inset: 0,
              filter: isComplete
                ? `drop-shadow(0 0 8px ${color}) drop-shadow(0 0 18px ${color}aa)`
                : `drop-shadow(0 0 5px ${color}88)`,
            }}
          >
            <svg
              width={diameter}
              height={diameter}
              style={{ display: 'block' }}
            >
              <defs>
                <radialGradient id="spike-shine" cx="50%" cy="30%" r="50%">
                  <stop offset="0%" stopColor="white" stopOpacity={0.6} />
                  <stop offset="100%" stopColor="white" stopOpacity={0} />
                </radialGradient>
              </defs>
              <polygon
                points={starPath}
                fill={bgColor}
                stroke={color}
                strokeWidth={2}
              />
              {filled > 0 && (
                <polygon
                  points={starPath}
                  fill={color}
                  fillOpacity={Math.min(0.92, filled / 100)}
                  strokeWidth={0}
                />
              )}
              <polygon
                points={starPath}
                fill="url(#spike-shine)"
                strokeWidth={0}
                opacity={0.25}
              />
            </svg>
          </div>
        )}

        {/* Text */}
        <div
          style={{
            position: 'absolute',
            inset: 0,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
            padding: '8px',
            pointerEvents: 'none',
            userSelect: 'none',
          }}
        >
          <span
            style={{
              fontSize: 13,
              fontWeight: 700,
              color: 'white',
              textAlign: 'center',
              lineHeight: 1.2,
              maxWidth: diameter - 14,
              overflow: 'hidden',
              whiteSpace: 'nowrap',
              textOverflow: 'ellipsis',
              textShadow: '0 1px 4px rgba(0,0,0,0.95)',
              letterSpacing: '0.02em',
            }}
          >
            {title}
          </span>
          <span
            style={{
              fontSize: 10,
              color: 'rgba(255,255,255,0.62)',
              textAlign: 'center',
              lineHeight: 1.2,
              maxWidth: diameter - 14,
              overflow: 'hidden',
              whiteSpace: 'nowrap',
              textOverflow: 'ellipsis',
              marginTop: 3,
              textShadow: '0 1px 3px rgba(0,0,0,0.95)',
            }}
          >
            {subtitle}
          </span>
        </div>

        {/* Avatar badge — scales with node size */}
        <div
          style={{
            position: 'absolute',
            bottom: -6,
            right: -6,
            width: Math.round(Math.max(18, Math.min(44, baseRadius * 0.5))),
            height: Math.round(Math.max(18, Math.min(44, baseRadius * 0.5))),
            borderRadius: '50%',
            background: `radial-gradient(circle at 40% 35%, ${color}dd, ${color})`,
            border: '1.5px solid rgba(255,255,255,0.2)',
            boxShadow: `0 0 6px ${color}88, inset 0 1px 2px rgba(255,255,255,0.25)`,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            fontSize: Math.round(Math.max(7, Math.min(16, baseRadius * 0.18))),
            fontWeight: 700,
            color: 'white',
            zIndex: 10,
            overflow: 'hidden',
            pointerEvents: 'none',
            textShadow: '0 1px 2px rgba(0,0,0,0.6)',
          }}
        >
          {isImageAvatar ? (
            <img
              src={avatar}
              alt=""
              style={{ width: '100%', height: '100%', objectFit: 'cover' }}
            />
          ) : (
            avatar
          )}
        </div>

        {/* Over overlay */}
        {over !== null ? (
          <div
            style={{
              position: 'absolute',
              inset: 0,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              zIndex: 20,
              pointerEvents: 'none',
            }}
          >
            {over}
          </div>
        ) : null}
      </div>

      {/* Right-side action buttons (collapse + add child) */}
      {hasChildren || onAddChild ? (
        <div
          style={{
            position: 'absolute',
            left: totalRadius + baseRadius - 9,
            top: totalRadius - (hasChildren && onAddChild ? 20 : 9),
            display: 'flex',
            flexDirection: 'column',
            gap: 4,
            zIndex: 30,
          }}
        >
          {hasChildren && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation()
                onToggle()
              }}
              style={{
                width: 18,
                height: 18,
                borderRadius: '50%',
                background: buttonBg,
                border: `1.5px solid ${color}`,
                color: color,
                boxShadow: `0 0 6px ${color}66`,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                fontSize: 12,
                fontWeight: 700,
                cursor: 'pointer',
                padding: 0,
                lineHeight: 1,
              }}
              aria-label={isCollapsed ? 'Expand' : 'Collapse'}
            >
              {isCollapsed ? '+' : '−'}
            </button>
          )}
          {onAddChild && (
            <button
              type="button"
              onClick={(e) => {
                e.stopPropagation()
                onAddChild()
              }}
              style={{
                width: 18,
                height: 18,
                borderRadius: '50%',
                background: buttonBg,
                border: `1.5px solid ${color}88`,
                color: `${color}cc`,
                boxShadow: `0 0 4px ${color}44`,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                fontSize: 14,
                fontWeight: 400,
                cursor: 'pointer',
                padding: 0,
                lineHeight: 1,
              }}
              aria-label="Add child"
            >
              +
            </button>
          )}
        </div>
      ) : null}
    </div>
  )
}
