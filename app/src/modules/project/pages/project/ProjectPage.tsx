import { useParams } from 'react-router-dom'

export function ProjectPage() {
  const { projectId } = useParams<{ projectId: string }>()

  return (
    <div className="flex flex-col gap-2">
      <h1 className="text-2xl font-bold">Project</h1>
      <p className="text-sm text-muted-foreground">{projectId}</p>
    </div>
  )
}
