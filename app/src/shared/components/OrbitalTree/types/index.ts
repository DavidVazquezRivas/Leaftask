import type { ReactNode } from 'react'

export type NodeShape = 'circle' | 'square' | 'triangle' | 'spike'

export interface NodeVisual {
  title: string
  subtitle: string
  color: string
  shape: NodeShape
  filled: number    // 0–100 progress %
  size: number      // 0–1 circle scale (driven by estimation)
  orbits: number    // ring count (driven by registered work)
  avatar: string
  over: ReactNode | null
}

export interface RawNode {
  id: string
  parentId: string | null
}

export type NodeAdapter<T extends RawNode> = (raw: T) => NodeVisual

export interface OrbitalTreeProps<T extends RawNode> {
  data: T[]
  nodeAdapter: NodeAdapter<T>
  onClickNode?: (raw: T) => void
  onAddChild?: (raw: T) => void
}

export interface PositionedNode<T extends RawNode> {
  id: string
  screenX: number
  screenY: number
  raw: T
  hasChildren: boolean
}

export interface EdgeData {
  id: string
  sourceId: string
  targetId: string
  x1: number
  y1: number
  x2: number
  y2: number
}
