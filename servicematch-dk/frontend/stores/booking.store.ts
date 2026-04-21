import { defineStore } from 'pinia'
import type { CategoryDto, ServiceRequestDto } from '~/types/api.types'

export const useBookingStore = defineStore('booking', {
  state: () => ({
    categories: [] as CategoryDto[],
    myRequests: [] as ServiceRequestDto[],
    wizard: {
      categoryId: null as number | null,
      freeText: '',
      selectedServiceName: '',
      requestedDate: '',
      requestedTime: '',
      city: '',
    },
  }),

  actions: {
    resetWizard() {
      this.wizard = { categoryId: null, freeText: '', selectedServiceName: '', requestedDate: '', requestedTime: '', city: '' }
    },
  },
})
