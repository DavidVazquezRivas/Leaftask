import { stratify, tree } from 'd3-hierarchy'
import { useMemo } from 'react'

import {
  ORBITAL_BASE_RADIUS,
  ORBITAL_NODE_SIZE_H,
  ORBITAL_NODE_SIZE_V,
  ORBITAL_PADDING,
  ORBITAL_VIRTUAL_ROOT_ID,
} from '../constants'
import type { EdgeData, PositionedNode, RawNode } from '../types'

export function useOrbitalLayout<T extends RawNode>(
  data: T[]
): { nodes: PositionedNode<T>[]; edges: EdgeData[] } {
  return useMemo(() => {
    if (data.length === 0) return { nodes: [], edges: [] }

    const parentIds = new Set(
      data.filter((n) => n.parentId !== null).map((n) => n.parentId as string)
    )

    const roots = data.filter((n) => n.parentId === null)
    const needsVirtualRoot = roots.length > 1

    const prepared = needsVirtualRoot
      ? ([
          { id: ORBITAL_VIRTUAL_ROOT_ID, parentId: null } as unknown as T,
          ...data.map((n) =>
            n.parentId === null
              ? ({ ...n, parentId: ORBITAL_VIRTUAL_ROOT_ID } as T)
              : n
          ),
        ] as T[])
      : data

    let pointRoot
    try {
      const s = stratify<T>()
        .id((d) => d.id)
        .parentId((d) => d.parentId as string | null | undefined)
      const root = s(prepared)
      const treeLayout = tree<T>().nodeSize([
        ORBITAL_NODE_SIZE_V,
        ORBITAL_NODE_SIZE_H,
      ])
      pointRoot = treeLayout(root)
    } catch {
      return { nodes: [], edges: [] }
    }

    const allNodes = pointRoot.descendants()
    const renderNodes = allNodes.filter(
      (n) => n.data.id !== ORBITAL_VIRTUAL_ROOT_ID
    )

    const minY =
      renderNodes.length > 0 ? Math.min(...renderNodes.map((n) => n.y)) : 0
    const minX = Math.min(...allNodes.map((n) => n.x))

    const toScreenX = (y: number) => y - minY + ORBITAL_PADDING
    const toScreenY = (x: number) => x - minX + ORBITAL_PADDING

    const nodes: PositionedNode<T>[] = renderNodes.map((n) => ({
      id: n.data.id,
      screenX: toScreenX(n.y),
      screenY: toScreenY(n.x),
      raw: n.data,
      hasChildren: parentIds.has(n.data.id),
    }))

    const edges: EdgeData[] = pointRoot
      .links()
      .filter((l) => l.source.data.id !== ORBITAL_VIRTUAL_ROOT_ID)
      .map((l) => ({
        id: `${l.source.data.id}--${l.target.data.id}`,
        sourceId: l.source.data.id,
        targetId: l.target.data.id,
        x1: toScreenX(l.source.y) + ORBITAL_BASE_RADIUS,
        y1: toScreenY(l.source.x),
        x2: toScreenX(l.target.y) - ORBITAL_BASE_RADIUS,
        y2: toScreenY(l.target.x),
      }))

    return { nodes, edges }
  }, [data])
}
