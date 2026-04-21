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
        <div class="w-7 h-7 rounded-full bg-violet-600 flex items-center justify-center">
          <span class="text-white text-xs font-bold">2</span>
        </div>
        <span class="text-sm font-semibold text-violet-700">Tidspunkt</span>
      </div>
      <div class="flex-1 h-px bg-gray-200"></div>
      <div class="flex items-center gap-2">
        <div class="w-7 h-7 rounded-full bg-gray-200 flex items-center justify-center">
          <span class="text-gray-500 text-xs font-bold">3</span>
        </div>
        <span class="text-sm text-gray-400">Bekræft</span>
      </div>
    </div>

    <!-- Selected service chip -->
    <div v-if="bookingStore.wizard.freeText"
         class="flex items-center gap-3 bg-violet-50 border border-violet-200 rounded-xl px-4 py-3 mb-6">
      <svg class="w-4 h-4 text-violet-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
      <p class="text-sm text-gray-700 flex-1 leading-snug">{{ bookingStore.wizard.freeText }}</p>
      <button @click="bookingStore.wizard.freeText = ''; bookingStore.wizard.selectedServiceName = ''"
              class="text-violet-400 hover:text-violet-600 flex-shrink-0">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/></svg>
      </button>
    </div>

    <h2 class="page-header mb-1">Hvornår og hvor?</h2>
    <p class="page-sub mb-6">Vælg dato, tidspunkt og by for din service</p>

    <div class="card space-y-5">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1.5">Dato</label>
        <input v-model="bookingStore.wizard.requestedDate" type="date" required :min="today" class="input-field" />
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1.5">Tidspunkt</label>
        <input v-model="bookingStore.wizard.requestedTime" type="time" required class="input-field" />
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1.5">By</label>
        <select v-model="bookingStore.wizard.city" required class="input-field">
          <option value="">Vælg by</option>
          <option v-for="city in cities" :key="city" :value="city">{{ city }}</option>
        </select>
      </div>
    </div>

    <div class="mt-6 flex justify-between">
      <button @click="$router.back()" class="btn-secondary">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"/></svg>
        Tilbage
      </button>
      <button @click="next" :disabled="!canProceed" class="btn-primary px-8 py-2.5">
        Gennemse
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/></svg>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'client-only' })

const bookingStore = useBookingStore()
const router = useRouter()

onMounted(() => {
  if (!bookingStore.wizard.categoryId && !bookingStore.wizard.freeText.trim()) {
    navigateTo('/booking/new')
  }
})

const today = computed(() => new Date().toISOString().split('T')[0])
const cities = [
  'København', 'Aarhus', 'Odense', 'Aalborg', 'Esbjerg',
  'Randers', 'Kolding', 'Vejle', 'Horsens', 'Helsingør',
]

const canProceed = computed(() =>
  bookingStore.wizard.requestedDate &&
  bookingStore.wizard.requestedTime &&
  bookingStore.wizard.city)

function next() {
  if (canProceed.value) router.push('/booking/review')
}
</script>
