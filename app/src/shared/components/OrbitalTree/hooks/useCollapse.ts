import { useCallback, useState } from 'react'

export function useCollapse() {
  const [collapsed, setCollapsed] = useState<Set<string>>(new Set())

  const toggle = useCallback((id: string) => {
    setCollapsed((prev) => {
      const next = new Set(prev)
      if (next.has(id)) {
        next.delete(id)
      } else {
        next.add(id)
      }
      return next
    })
  }, [])

  return { collapsed, toggle }
}
