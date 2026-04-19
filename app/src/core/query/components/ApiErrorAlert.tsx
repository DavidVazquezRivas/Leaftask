import type { ReactNode } from 'react'

import {
  Alert,
  AlertDescription,
  AlertTitle,
} from '@/shared/components/ui/alert'

interface ApiErrorAlertProps {
  title: string
  description: string
  action?: ReactNode
}

export function ApiErrorAlert({
  title,
  description,
  action,
}: ApiErrorAlertProps) {
  return (
    <Alert variant="destructive">
      <AlertTitle>{title}</AlertTitle>
      <AlertDescription className="space-y-3">
        <p>{description}</p>
        {action ? <div>{action}</div> : null}
      </AlertDescription>
    </Alert>
  )
}
