import { create } from 'zustand'
import { persist } from 'zustand/middleware'

interface OrganizationState {
  selectedOrganizationId: string | null
  setSelectedOrganizationId: (organizationId: string | null) => void
}

export const useOrganizationStore = create<OrganizationState>()(
  persist(
    (set) => ({
      selectedOrganizationId: null,
      setSelectedOrganizationId: (organizationId) =>
        set({ selectedOrganizationId: organizationId }),
    }),
    {
      name: 'leaftask-organization-store',
      partialize: (state) => ({
        selectedOrganizationId: state.selectedOrganizationId,
      }),
    }
  )
)
