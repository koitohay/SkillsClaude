<template>
  <div class="max-w-2xl mx-auto">
    <!-- Progress indicator -->
    <div class="flex items-center gap-2 mb-8">
      <div class="flex items-center gap-2">
        <div class="w-7 h-7 rounded-full bg-violet-200 flex items-center justify-center">
          <svg class="w-3.5 h-3.5 text-violet-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7"/></svg>
        </div>
        <span class="text-sm text-violet-500">Service</span>
      </div>
      <div class="flex-1 h-px bg-violet-200"></div>
      <div class="flex items-center gap-2">
        <div class="w-7 h-7 rounded-full bg-violet-200 flex items-center justify-center">
          <svg class="w-3.5 h-3.5 text-violet-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7"/></svg>
        </div>
        <span class="text-sm text-violet-500">Tidspunkt</span>
      </div>
      <div class="flex-1 h-px bg-violet-200"></div>
      <div class="flex items-center gap-2">
        <div class="w-7 h-7 rounded-full bg-violet-600 flex items-center justify-center">
          <span class="text-white text-xs font-bold">3</span>
        </div>
        <span class="text-sm font-semibold text-violet-700">Bekræft</span>
      </div>
    </div>

    <h2 class="page-header mb-1">Gennemse forespørgsel</h2>
    <p class="page-sub mb-6">Kontroller oplysningerne før du sender</p>

    <div class="card mb-6">
      <p class="section-label">Detaljer</p>
      <div class="space-y-3">
        <div class="flex justify-between items-center">
          <span class="text-sm text-gray-500">Kategori</span>
          <span class="font-medium text-gray-900 text-sm">{{ categoryName }}</span>
        </div>
        <div v-if="w.freeText" class="flex justify-between items-start gap-4">
          <span class="text-sm text-gray-500 flex-shrink-0">Beskrivelse</span>
          <span class="font-medium text-gray-900 text-sm text-right leading-snug max-w-xs">{{ w.freeText }}</span>
        </div>
        <div class="divider"></div>
        <div class="grid grid-cols-3 gap-3 text-center">
          <div class="bg-gray-50 rounded-xl p-3">
            <p class="text-xs text-gray-400 mb-1">Dato</p>
            <p class="font-semibold text-sm text-gray-900">{{ w.requestedDate }}</p>
          </div>
          <div class="bg-gray-50 rounded-xl p-3">
            <p class="text-xs text-gray-400 mb-1">Tidspunkt</p>
            <p class="font-semibold text-sm text-gray-900">{{ w.requestedTime }}</p>
          </div>
          <div class="bg-gray-50 rounded-xl p-3">
            <p class="text-xs text-gray-400 mb-1">By</p>
            <p class="font-semibold text-sm text-gray-900">{{ w.city }}</p>
          </div>
        </div>
      </div>
    </div>

    <div v-if="error" class="alert-error mb-4">
      <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
      {{ error }}
    </div>

    <div class="flex justify-between">
      <button @click="$router.back()" class="btn-secondary">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"/></svg>
        Tilbage
      </button>
      <button @click="submit" :disabled="loading" class="btn-primary px-8 py-2.5">
        <svg v-if="loading" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>
        <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"/></svg>
        {{ loading ? 'Sender…' : 'Send forespørgsel' }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'client-only' })

import type { CategoryDto, ServiceRequestDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const bookingStore = useBookingStore()
const w = bookingStore.wizard

onMounted(() => {
  if (!bookingStore.wizard.categoryId && !bookingStore.wizard.freeText.trim()) {
    navigateTo('/booking/new')
  }
})

const { data: categories } = await useAsyncData('categories', () => $api<CategoryDto[]>('/categories'))

const categoryName = computed(() =>
  categories.value?.find(c => c.id === w.categoryId)?.name ?? 'Fri tekst')

const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true
  try {
    const result = await $api<ServiceRequestDto>('/requests', {
      method: 'POST',
      body: {
        categoryId: w.categoryId,
        freeTextDescription: w.freeText || null,
        requestedDate: w.requestedDate,
        requestedTime: w.requestedTime + ':00',
        city: w.city,
      },
    })
    bookingStore.resetWizard()
    await navigateTo(`/booking/${result.id}/offers`)
  } catch (e: unknown) {
    const errData = (e as { data?: { error?: string; errors?: { message: string }[] } })?.data
    error.value = errData?.error ?? errData?.errors?.[0]?.message ?? 'Noget gik galt. Prøv igen.'
  } finally {
    loading.value = false
  }
}
</script>
