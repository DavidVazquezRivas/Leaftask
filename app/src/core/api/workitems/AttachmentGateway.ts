import { apiClient } from '@/core/api/global/httpClient'
import { ApiError } from '@/core/api/global/errors'
import { ApiRoutes } from '@/core/api/global/constants/routes'
import { isApiErrorResponse, isApiSuccessResponse } from '@/core/api/global/types/response'
import type { WorkItemAttachmentData } from './workitems.types'

interface UploadAttachmentApiResponse {
  data?: WorkItemAttachmentData
  error?: { code: string; message: string }
  meta?: unknown
}

interface PresignedUrlData {
  presignedUrl: string
  publicUrl: string
}

interface PresignedUrlApiResponse {
  data?: PresignedUrlData
  error?: { code: string; message: string }
  meta?: unknown
}

export class AttachmentGateway {
  /**
   * Uploads a file directly to MinIO using a presigned URL.
   * The backend only generates the URL — the file never passes through it.
   * Use this for inline images in rich text descriptions.
   */
  static async presignAndUpload(
    projectId: string,
    itemId: string,
    file: File
  ): Promise<string> {
    const presign = await AttachmentGateway.getPresignedUrl(projectId, itemId, file.name)

    const uploadResponse = await fetch(presign.presignedUrl, {
      method: 'PUT',
      body: file,
      headers: { 'Content-Type': file.type },
    })

    if (!uploadResponse.ok) {
      throw new ApiError('Attachment.Presign.UploadFailed', {
        message: `Direct upload to storage failed: ${uploadResponse.status}`,
      })
    }

    return presign.publicUrl
  }

  private static async getPresignedUrl(
    projectId: string,
    itemId: string,
    fileName: string
  ): Promise<PresignedUrlData> {
    const response = await apiClient.get<PresignedUrlApiResponse>(
      ApiRoutes.WorkItem.Attachments.PresignedUpload(projectId, itemId, fileName)
    )

    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data) {
      throw new ApiError('Attachment.Presign.InvalidResponse', {
        message: 'Presigned URL response is invalid',
      })
    }

    return payload.data
  }

  /** Traditional upload through backend (for explicit file attachments). */
  static async upload(
    projectId: string,
    itemId: string,
    file: File
  ): Promise<WorkItemAttachmentData> {
    const form = new FormData()
    form.append('file', file)

    const response = await apiClient.post<UploadAttachmentApiResponse>(
      ApiRoutes.WorkItem.Attachments.Upload(projectId, itemId),
      form,
      { headers: { 'Content-Type': 'multipart/form-data' } }
    )

    const payload = response.data

    if (isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }

    if (!isApiSuccessResponse(payload) || !payload.data) {
      throw new ApiError('Attachment.Upload.InvalidResponse', {
        message: 'Upload response is invalid',
      })
    }

    return payload.data
  }

  static async delete(
    projectId: string,
    itemId: string,
    attachmentId: string
  ): Promise<void> {
    const response = await apiClient.delete(
      ApiRoutes.WorkItem.Attachments.Delete(projectId, itemId, attachmentId)
    )

    const payload = response.data

    if (payload && isApiErrorResponse(payload)) {
      throw new ApiError(payload.error.code, {
        message: payload.error.message,
        status: response.status,
        meta: payload.meta,
      })
    }
  }
}
