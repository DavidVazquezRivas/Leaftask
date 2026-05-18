import { useMemo } from 'react'

import { ORBITAL_PADDING } from '../constants'
import { computeTotalRadius } from '../components/OrbitalNode'
import type { NodeVisual, PositionedNode, RawNode } from '../types'

export function useOrbitalBounds<T extends RawNode>(
  nodes: PositionedNode<T>[],
  visualsMap: Map<string, NodeVisual>
): { width: number; height: number } {
  return useMemo(() => {
    if (nodes.length === 0) {
      return { width: ORBITAL_PADDING * 2, height: ORBITAL_PADDING * 2 }
    }

    let maxX = 0
    let maxY = 0

    for (const n of nodes) {
      const visual = visualsMap.get(n.id)
      const orbits = visual?.orbits ?? 0
      const size = visual?.size ?? 0
      const totalRadius = computeTotalRadius(orbits, size)
      maxX = Math.max(maxX, n.screenX + totalRadius)
      maxY = Math.max(maxY, n.screenY + totalRadius)
    }

    return {
      width: maxX + ORBITAL_PADDING,
      height: maxY + ORBITAL_PADDING,
    }
  }, [nodes, visualsMap])
}
