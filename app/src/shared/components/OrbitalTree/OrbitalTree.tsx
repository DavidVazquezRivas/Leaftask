import { useMemo } from 'react'

import { useCollapse } from './hooks/useCollapse'
import { useOrbitalBounds } from './hooks/useOrbitalBounds'
import { useOrbitalLayout } from './hooks/useOrbitalLayout'
import { usePan } from './hooks/usePan'
import { OrbitalEdge } from './components/OrbitalEdge'
import { OrbitalNode } from './components/OrbitalNode'
import type { OrbitalTreeProps, RawNode } from './types'

function getDescendantIds(data: RawNode[], parentId: string): string[] {
  const children = data.filter((n) => n.parentId === parentId)
  return children.flatMap((c) => [c.id, ...getDescendantIds(data, c.id)])
}

export function OrbitalTree<T extends RawNode>({
  data,
  nodeAdapter,
  onClickNode,
}: OrbitalTreeProps<T>) {
  const { collapsed, toggle } = useCollapse()
  const { nodes, edges } = useOrbitalLayout(data)
  const { transform, isDragging, isMoving, handlers } = usePan()

  const visualsMap = useMemo(
    () => new Map(nodes.map((n) => [n.id, nodeAdapter(n.raw)])),
    [nodes, nodeAdapter]
  )

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
          {visibleEdges.map((edge) => (
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
            />
          )
        })}
      </div>
    </div>
  )
}
