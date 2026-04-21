<template>
  <div class="max-w-2xl mx-auto">
    <!-- Progress indicator -->
    <div class="flex items-center gap-2 mb-8">
      <div class="flex items-center gap-2">
        <div class="w-7 h-7 rounded-full bg-violet-600 flex items-center justify-center">
          <span class="text-white text-xs font-bold">1</span>
        </div>
        <span class="text-sm font-semibold text-violet-700">Service</span>
      </div>
      <div class="flex-1 h-px bg-gray-200"></div>
      <div class="flex items-center gap-2">
        <div class="w-7 h-7 rounded-full bg-gray-200 flex items-center justify-center">
          <span class="text-gray-500 text-xs font-bold">2</span>
        </div>
        <span class="text-sm text-gray-400">Tidspunkt</span>
      </div>
      <div class="flex-1 h-px bg-gray-200"></div>
      <div class="flex items-center gap-2">
        <div class="w-7 h-7 rounded-full bg-gray-200 flex items-center justify-center">
          <span class="text-gray-500 text-xs font-bold">3</span>
        </div>
        <span class="text-sm text-gray-400">Bekræft</span>
      </div>
    </div>

    <h2 class="page-header mb-1">Hvad søger du?</h2>
    <p class="page-sub mb-6">Vælg en kategori eller beskriv hvad du har brug for</p>

    <!-- Category grid -->
    <div class="grid grid-cols-2 sm:grid-cols-3 gap-3 mb-6">
      <div v-for="cat in categories" :key="cat.id"
           :class="['card cursor-pointer border-2 transition-all text-center py-4 px-3',
                    bookingStore.wizard.categoryId === cat.id
                      ? 'border-violet-500 bg-violet-50 shadow-sm'
                      : 'border-transparent hover:border-violet-200 hover:bg-gray-50']"
           @click="selectCategory(cat.id)">
        <div class="text-2xl mb-2">{{ catEmoji(cat.slug) }}</div>
        <div class="font-semibold text-sm text-gray-800">{{ cat.name }}</div>
        <div v-if="bookingStore.wizard.categoryId === cat.id"
             class="mt-1.5 text-xs text-violet-600 font-medium">Valgt ✓</div>
      </div>
    </div>

    <!-- Free text -->
    <div class="card">
      <label class="block text-sm font-medium text-gray-700 mb-1.5">
        Eller beskriv hvad du søger
        <span class="text-gray-400 font-normal">(valgfri hvis kategori er valgt)</span>
      </label>
      <textarea v-model="bookingStore.wizard.freeText" rows="3" class="input-field"
                placeholder="F.eks. 'Har brug for en frisør til langt hår, inkl. farve'" />
    </div>

    <div class="mt-6 flex justify-end">
      <button @click="next" :disabled="!canProceed" class="btn-primary px-8 py-2.5">
        Næste
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/></svg>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'client-only' })

import type { CategoryDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const bookingStore = useBookingStore()
const router = useRouter()

const { data: categories } = await useAsyncData('categories', () => $api<CategoryDto[]>('/categories'))

const canProceed = computed(() =>
  bookingStore.wizard.categoryId !== null || bookingStore.wizard.freeText.trim().length > 0)

function selectCategory(id: number) {
  bookingStore.wizard.categoryId = bookingStore.wizard.categoryId === id ? null : id
}

function catEmoji(slug: string): string {
  const map: Record<string, string> = {
    salon: '✂️', nails: '💅', massage: '💆', dentist: '🦷', kiropraktor: '🦴'
  }
  return map[slug] ?? '🔧'
}

function next() {
  if (canProceed.value) router.push('/booking/schedule')
}
</script>
