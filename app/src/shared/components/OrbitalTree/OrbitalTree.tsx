import { useCallback, useMemo } from 'react'

import { useCollapse } from './hooks/useCollapse'
import { useOrbitalBounds } from './hooks/useOrbitalBounds'
import { useOrbitalLayout } from './hooks/useOrbitalLayout'
import { usePan } from './hooks/usePan'
import { OrbitalEdge } from './components/OrbitalEdge'
import { OrbitalNode, computeBaseRadius, computeTotalRadius } from './components/OrbitalNode'
import { ORBITAL_BASE_RADIUS } from './constants'
import type { OrbitalTreeProps, RawNode } from './types'

function getDescendantIds(data: RawNode[], parentId: string): string[] {
  const children = data.filter((n) => n.parentId === parentId)
  return children.flatMap((c) => [c.id, ...getDescendantIds(data, c.id)])
}

export function OrbitalTree<T extends RawNode>({
  data,
  nodeAdapter,
  onClickNode,
  onAddChild,
}: OrbitalTreeProps<T>) {
  const { collapsed, toggle } = useCollapse()
  const { transform, isDragging, isMoving, handlers } = usePan()

  // Pre-compute visuals from raw data so the layout can use node sizes
  const visualsMap = useMemo(
    () => new Map(data.map((d) => [d.id, nodeAdapter(d)])),
    [data, nodeAdapter]
  )

  const getNodeRadius = useCallback(
    (id: string) => {
      const v = visualsMap.get(id)
      return v ? computeTotalRadius(v.orbits, v.size) : ORBITAL_BASE_RADIUS
    },
    [visualsMap]
  )

  const { nodes, edges } = useOrbitalLayout(data, getNodeRadius)
  const { width, height } = useOrbitalBounds(nodes, visualsMap)

  const hiddenIds = useMemo(() => {
    const hidden = new Set<string>()
    collapsed.forEach((id) => {
      getDescendantIds(data, id).forEach((d) => hidden.add(d))
    })
    return hidden
  }, [collapsed, data])

  const visibleNodes = useMemo(
    () => nodes.filter((n) => !hiddenIds.has(n.id)),
    [nodes, hiddenIds]
  )

  const visibleEdges = useMemo(
    () => edges.filter((e) => !hiddenIds.has(e.targetId)),
    [edges, hiddenIds]
  )

  const adjustedEdges = useMemo(
    () =>
      visibleEdges.map((edge) => {
        const srcSize = visualsMap.get(edge.sourceId)?.size ?? 0
        const tgtSize = visualsMap.get(edge.targetId)?.size ?? 0
        const dx1 = computeBaseRadius(srcSize) - ORBITAL_BASE_RADIUS
        const dx2 = computeBaseRadius(tgtSize) - ORBITAL_BASE_RADIUS
        return { ...edge, x1: edge.x1 + dx1, x2: edge.x2 - dx2 }
      }),
    [visibleEdges, visualsMap]
  )

  return (
    <div
      style={{
        width: '100%',
        height: '100%',
        overflow: 'hidden',
        position: 'relative',
        cursor: isDragging.current ? 'grabbing' : 'grab',
      }}
      {...handlers}
    >
      <div
        style={{
          position: 'relative',
          width,
          height,
          minWidth: width,
          transform: `translate(${transform.x}px, ${transform.y}px) scale(${transform.scale})`,
          transformOrigin: '0 0',
          willChange: 'transform',
        }}
      >
        <svg
          style={{
            position: 'absolute',
            inset: 0,
            width,
            height,
            pointerEvents: 'none',
            overflow: 'visible',
          }}
        >
          {adjustedEdges.map((edge) => (
            <OrbitalEdge
              key={edge.id}
              {...edge}
              color={visualsMap.get(edge.sourceId)?.color ?? '#4b5563'}
            />
          ))}
        </svg>

        {visibleNodes.map((node) => {
          const visual = visualsMap.get(node.id)
          if (!visual) return null
          return (
            <OrbitalNode
              key={node.id}
              x={node.screenX}
              y={node.screenY}
              visual={visual}
              hasChildren={node.hasChildren}
              isCollapsed={collapsed.has(node.id)}
              onToggle={() => {
                if (!isMoving()) toggle(node.id)
              }}
              onClick={
                onClickNode
                  ? () => {
                      if (!isMoving()) onClickNode(node.raw)
                    }
                  : undefined
              }
              onAddChild={
                onAddChild
                  ? () => {
                      if (!isMoving()) onAddChild(node.raw)
                    }
                  : undefined
              }
            />
          )
        })}
      </div>
    </div>
  )
}
