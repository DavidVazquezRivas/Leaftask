import { useMemo, useState } from 'react'
import { useParams } from 'react-router'
import { Plus, Loader2 } from 'lucide-react'
import { useTranslation } from 'react-i18next'

import { OrbitalTree } from '@/shared/components/OrbitalTree'
import type { WorkItemData } from '@/core/api/workitems'
import {
  useProjectWorkItemsQuery,
  useWorkItemTypesQuery,
  useWorkItemStatusesQuery,
  useCreateWorkItemMutation,
} from '@/core/query/workitems'
import {
  useProjectMembersQuery,
  useProjectCustomFieldsQuery,
  useProjectFieldTypesQuery,
} from '@/core/query/project'

import { makeProjectNodeAdapter } from './components/projectNodeAdapter'
import { CreateWorkItemDialog } from './components/CreateWorkItemDialog'
import { WorkItemDetailPanel } from './components/WorkItemDetailPanel'

interface CreateState {
  open: boolean
  parentId: string | null
}

export function ProjectTreePage() {
  const { projectId } = useParams<{ projectId: string }>()
  const { t } = useTranslation('workitems')

  const workItemsQuery = useProjectWorkItemsQuery(projectId ?? null)
  const typesQuery = useWorkItemTypesQuery()
  const statusesQuery = useWorkItemStatusesQuery()
  const membersQuery = useProjectMembersQuery(projectId ?? null, { limit: 100 })
  const customFieldsQuery = useProjectCustomFieldsQuery(projectId ?? '')
  const fieldTypesQuery = useProjectFieldTypesQuery()
  const createMutation = useCreateWorkItemMutation(projectId ?? '')

  const [createState, setCreateState] = useState<CreateState>({
    open: false,
    parentId: null,
  })
  const [selectedItemId, setSelectedItemId] = useState<string | null>(null)

  const types = useMemo(() => typesQuery.data?.data ?? [], [typesQuery.data])
  const statuses = useMemo(
    () => statusesQuery.data?.data ?? [],
    [statusesQuery.data]
  )
  const members = useMemo(
    () => membersQuery.data?.data ?? [],
    [membersQuery.data]
  )
  const workItems = useMemo(
    () => workItemsQuery.data?.data ?? [],
    [workItemsQuery.data]
  )
  const customFields = useMemo(
    () => customFieldsQuery.data?.data ?? [],
    [customFieldsQuery.data]
  )
  const fieldTypeNameById = useMemo(
    () =>
      new Map((fieldTypesQuery.data?.data ?? []).map((ft) => [ft.id, ft.name])),
    [fieldTypesQuery.data]
  )

  const nodeAdapter = useMemo(
    () => makeProjectNodeAdapter(types, statuses),
    [types, statuses]
  )

  const openCreate = (parentId: string | null) =>
    setCreateState({ open: true, parentId })

  const closeCreate = () => setCreateState({ open: false, parentId: null })

  const handleCreate = (payload: {
    title: string
    description: string
    typeId: string
    statusId: string
    assigneeId: string | null
    estimation: number
    customFields: Record<string, string>
  }) => {
    createMutation.mutate(
      {
        title: payload.title,
        description: payload.description,
        typeId: payload.typeId,
        statusId: payload.statusId,
        assigneeId: payload.assigneeId,
        estimation: payload.estimation,
        customFields: payload.customFields,
        parentId: createState.parentId ?? (projectId as string),
      },
      { onSuccess: closeCreate }
    )
  }

  const isLoading =
    workItemsQuery.isLoading || typesQuery.isLoading || statusesQuery.isLoading

  if (isLoading) {
    return (
      <div className="flex h-full w-full items-center justify-center">
        <Loader2 className="size-6 animate-spin text-muted-foreground" />
      </div>
    )
  }

  return (
    <div className="relative h-full w-full overflow-hidden">
      {workItems.length === 0 ? (
        <div className="flex h-full w-full flex-col items-center justify-center gap-3">
          <p className="text-sm text-muted-foreground">
            {t('empty.description')}
          </p>
          <button
            type="button"
            onClick={() => openCreate(null)}
            className="flex items-center gap-2 rounded-lg border border-border bg-card px-4 py-2 text-sm font-medium text-foreground transition-colors hover:bg-accent"
          >
            <Plus size={14} />
            {t('empty.action')}
          </button>
        </div>
      ) : (
        <OrbitalTree
          data={workItems as WorkItemData[]}
          nodeAdapter={nodeAdapter}
          onClickNode={(raw) => setSelectedItemId(raw.id)}
          onAddChild={(raw) => openCreate(raw.id)}
        />
      )}

      {workItems.length > 0 && (
        <button
          type="button"
          onClick={() => openCreate(null)}
          className="absolute bottom-6 right-6 z-10 flex size-10 items-center justify-center rounded-full border border-border bg-card text-foreground shadow-sm transition-colors hover:bg-accent"
          aria-label={t('addRoot')}
        >
          <Plus size={18} />
        </button>
      )}

      <CreateWorkItemDialog
        open={createState.open}
        isSubmitting={createMutation.isPending}
        types={types}
        statuses={statuses}
        members={members}
        customFields={customFields}
        fieldTypeNameById={fieldTypeNameById}
        onClose={closeCreate}
        onSubmit={handleCreate}
      />

      <WorkItemDetailPanel
        open={selectedItemId !== null}
        projectId={projectId ?? ''}
        itemId={selectedItemId}
        onClose={() => setSelectedItemId(null)}
        onDelete={() => {
          setSelectedItemId(null)
        }}
        types={types}
        statuses={statuses}
        workItems={workItems}
        members={members}
        customFields={customFields}
        fieldTypeNameById={fieldTypeNameById}
      />
    </div>
  )
}
