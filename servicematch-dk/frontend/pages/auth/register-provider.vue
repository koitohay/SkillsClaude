<template>
  <div class="min-h-[70vh] flex items-center justify-center py-12">
    <div class="w-full max-w-lg">
      <div class="text-center mb-8">
        <div class="w-12 h-12 rounded-2xl bg-gradient-to-br from-violet-600 to-purple-500 flex items-center justify-center mx-auto mb-4 shadow-lg">
          <span class="text-white text-xl font-bold">S</span>
        </div>
        <h1 class="text-2xl font-bold text-gray-900">Bliv serviceudbyder</h1>
        <p class="text-sm text-gray-500 mt-1">Opret din virksomhedsprofil og modtag bookinger</p>
      </div>

      <div class="card">
        <form @submit.prevent="submit" class="space-y-4">
          <!-- Company info -->
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Firmanavn</label>
              <input v-model="form.companyName" required class="input-field" placeholder="Hansen Services" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Kontaktperson</label>
              <input v-model="form.contactName" required class="input-field" placeholder="Lars Hansen" />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">E-mail</label>
            <input v-model="form.email" type="email" required autocomplete="email" class="input-field" placeholder="firma@example.dk" />
          </div>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Telefon</label>
              <input v-model="form.phoneNumber" required class="input-field" placeholder="+45 20 12 34 56" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">CVR-nummer</label>
              <input v-model="form.cvrNumber" required maxlength="8" pattern="\d{8}" class="input-field" placeholder="12345678" />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Adresse</label>
            <input v-model="form.address" required class="input-field" placeholder="Strøget 1" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">By</label>
            <select v-model="form.city" required class="input-field">
              <option value="">Vælg by</option>
              <option v-for="city in cities" :key="city" :value="city">{{ city }}</option>
            </select>
          </div>

          <div class="divider"></div>

          <!-- Categories -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">Servicekategorier</label>
            <div class="flex flex-wrap gap-2">
              <label v-for="cat in categories" :key="cat.id"
                     :class="['flex items-center gap-1.5 px-3 py-1.5 rounded-xl border text-sm cursor-pointer transition-all',
                              form.categoryIds.includes(cat.id)
                                ? 'bg-violet-50 border-violet-300 text-violet-700 font-medium'
                                : 'bg-gray-50 border-gray-200 text-gray-600 hover:border-violet-200']">
                <input type="checkbox" :value="cat.id" v-model="form.categoryIds" class="sr-only" />
                {{ cat.name }}
              </label>
            </div>
          </div>

          <!-- Services -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-2">
              Dine ydelser
              <span class="text-gray-400 font-normal">(valgfri)</span>
            </label>
            <div v-if="form.services.length" class="space-y-2 mb-3">
              <div v-for="(svc, idx) in form.services" :key="idx"
                   class="flex items-center gap-3 bg-gray-50 rounded-xl px-4 py-2.5 border border-gray-100">
                <div class="flex-1 min-w-0">
                  <p class="font-semibold text-sm text-gray-900">{{ svc.name }}</p>
                  <p class="text-xs text-gray-500">{{ svc.description }}</p>
                </div>
                <p class="text-sm font-bold text-gray-700 flex-shrink-0">{{ svc.basePrice }} DKK</p>
                <button type="button" @click="form.services.splice(idx, 1)"
                        class="text-xs px-2 py-1 rounded-lg text-red-400 hover:text-red-600 hover:bg-red-50 transition-colors">
                  Slet
                </button>
              </div>
            </div>

            <div v-if="showServiceForm" class="border border-violet-200 bg-violet-50/60 rounded-xl p-4 space-y-3 mb-3">
              <input v-model="newSvc.name" class="input-field text-sm" placeholder="Ydelsens navn (fx Tandrensning)" />
              <textarea v-model="newSvc.description" rows="2" class="input-field text-sm" placeholder="Kort beskrivelse" />
              <div class="grid grid-cols-2 gap-3">
                <div class="relative">
                  <input v-model.number="newSvc.basePrice" type="number" min="1" class="input-field text-sm pr-14" placeholder="Pris" />
                  <span class="absolute right-4 top-1/2 -translate-y-1/2 text-xs text-gray-400">DKK</span>
                </div>
                <select v-model="newSvc.categoryId" class="input-field text-sm">
                  <option :value="null">Ingen kategori</option>
                  <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                </select>
              </div>
              <div class="flex gap-2">
                <button type="button" @click="addService" class="btn-primary text-sm px-4 py-2">Tilføj</button>
                <button type="button" @click="showServiceForm = false" class="btn-secondary text-sm px-4 py-2">Annuller</button>
              </div>
            </div>
            <button v-else type="button" @click="showServiceForm = true" class="btn-ghost w-full justify-center text-sm">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
              Tilføj ydelse
            </button>
          </div>

          <div class="divider"></div>

          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Adgangskode</label>
            <input v-model="form.password" type="password" required minlength="8" autocomplete="new-password" class="input-field" placeholder="Min. 8 tegn" />
          </div>

          <div v-if="error" class="alert-error">
            <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
            {{ error }}
          </div>

          <button type="submit" :disabled="loading" class="btn-primary w-full py-3 mt-2">
            <svg v-if="loading" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>
            {{ loading ? 'Opretter…' : 'Opret udbyderkonto' }}
          </button>
        </form>

        <div class="divider"></div>

        <p class="text-center text-sm text-gray-500">
          Har du allerede en konto?
          <NuxtLink to="/auth/login" class="text-violet-600 font-medium hover:text-violet-700">Log ind</NuxtLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  middleware: [() => {
    const auth = useAuthStore()
    if (auth.isProvider) return navigateTo('/provider/dashboard')
    if (auth.isClient) return navigateTo('/dashboard')
  }],
})

import type { CategoryDto, NewServiceDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const authStore = useAuthStore()

const cities = [
  'København', 'Aarhus', 'Odense', 'Aalborg', 'Esbjerg',
  'Randers', 'Kolding', 'Vejle', 'Horsens', 'Helsingør',
]

const { data: categories } = await useAsyncData('categories', () => $api<CategoryDto[]>('/categories'))

const form = reactive({
  companyName: '', contactName: '', email: '', phoneNumber: '',
  address: '', city: '', cvrNumber: '', password: '',
  categoryIds: [] as number[],
  services: [] as NewServiceDto[],
})

const showServiceForm = ref(false)
const newSvc = reactive({ name: '', description: '', basePrice: 0, categoryId: null as number | null })

function addService() {
  if (!newSvc.name || !newSvc.description || newSvc.basePrice <= 0) return
  form.services.push({ name: newSvc.name, description: newSvc.description, basePrice: newSvc.basePrice, categoryId: newSvc.categoryId })
  newSvc.name = ''; newSvc.description = ''; newSvc.basePrice = 0; newSvc.categoryId = null
  showServiceForm.value = false
}

const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  if (form.categoryIds.length === 0) { error.value = 'Vælg mindst én kategori.'; return }
  loading.value = true
  try {
    await $api('/auth/register/provider', { method: 'POST', body: form })
    const auth = await $api<{ token: string; role: string; userId: string }>('/auth/login', {
      method: 'POST', body: { email: form.email, password: form.password },
    })
    authStore.setAuth({ token: auth.token, role: 'Provider', userId: auth.userId })
    await navigateTo('/provider/dashboard')
  } catch (e: unknown) {
    error.value = (e as { data?: { error?: string } })?.data?.error ?? 'Registrering mislykkedes.'
  } finally {
    loading.value = false
  }
}
</script>
