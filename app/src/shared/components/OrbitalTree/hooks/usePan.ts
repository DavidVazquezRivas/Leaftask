import { useCallback, useRef, useState } from 'react'

interface Transform {
  x: number
  y: number
  scale: number
}

const INITIAL: Transform = { x: 0, y: 0, scale: 1 }

export function usePan() {
  const [transform, setTransform] = useState<Transform>(INITIAL)
  const stateRef = useRef<Transform>(INITIAL)
  const dragging = useRef(false)
  const lastPos = useRef({ x: 0, y: 0 })
  const moved = useRef(false)

  const apply = useCallback((next: Transform) => {
    stateRef.current = next
    setTransform(next)
  }, [])

  const onMouseDown = useCallback((e: React.MouseEvent) => {
    if (e.button !== 0) return
    dragging.current = true
    moved.current = false
    lastPos.current = { x: e.clientX, y: e.clientY }
  }, [])

  const onMouseMove = useCallback(
    (e: React.MouseEvent) => {
      if (!dragging.current) return
      const dx = e.clientX - lastPos.current.x
      const dy = e.clientY - lastPos.current.y
      if (Math.abs(dx) > 3 || Math.abs(dy) > 3) moved.current = true
      lastPos.current = { x: e.clientX, y: e.clientY }
      const t = stateRef.current
      apply({ ...t, x: t.x + dx, y: t.y + dy })
    },
    [apply]
  )

  const onMouseUp = useCallback(() => {
    dragging.current = false
  }, [])

  const onMouseLeave = useCallback(() => {
    dragging.current = false
  }, [])

  const onWheel = useCallback(
    (e: React.WheelEvent) => {
      e.stopPropagation()
      const { x, y, scale } = stateRef.current
      const factor = e.deltaY < 0 ? 1.1 : 0.9
      const newScale = Math.min(4, Math.max(0.15, scale * factor))
      const rect = (e.currentTarget as HTMLElement).getBoundingClientRect()
      const mouseX = e.clientX - rect.left
      const mouseY = e.clientY - rect.top
      const contentX = (mouseX - x) / scale
      const contentY = (mouseY - y) / scale
      apply({
        x: mouseX - contentX * newScale,
        y: mouseY - contentY * newScale,
        scale: newScale,
      })
    },
    [apply]
  )

  const isMoving = useCallback(() => moved.current, [])

  return {
    transform,
    isDragging: dragging,
    isMoving,
    handlers: { onMouseDown, onMouseMove, onMouseUp, onMouseLeave, onWheel },
  }
}
