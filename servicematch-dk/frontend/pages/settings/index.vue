<template>
  <div class="max-w-2xl mx-auto">
    <div class="mb-8">
      <h2 class="page-header">Indstillinger</h2>
      <p class="page-sub">Opdater dine profiloplysninger og ydelser</p>
    </div>

    <!-- Skeleton -->
    <div v-if="pending" class="card space-y-4" style="padding: 1.5rem;">
      <div class="h-4 rounded skeleton w-1/4 mb-2"></div>
      <div class="h-10 rounded-xl skeleton"></div>
      <div class="h-10 rounded-xl skeleton"></div>
      <div class="h-10 rounded-xl skeleton"></div>
    </div>

    <template v-else>
      <!-- ── Client profile ── -->
      <div v-if="auth.isClient" class="card mb-5" style="padding: 1.75rem;">
        <p class="section-label">Profiloplysninger</p>

        <div v-if="profileSaved" class="alert-success mb-4">
          <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          Ændringer gemt
        </div>
        <div v-if="profileError" class="alert-error mb-4">
          <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          {{ profileError }}
        </div>

        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">Fuldt navn</label>
            <input v-model="clientForm.fullName" class="input-field" placeholder="Jane Doe" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">E-mail</label>
            <input v-model="clientForm.email" type="email" class="input-field" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">Telefonnummer</label>
            <input v-model="clientForm.phoneNumber" class="input-field" placeholder="+45 20 12 34 56" />
          </div>
        </div>
        <button @click="saveClientProfile" :disabled="savingProfile" class="btn-primary mt-5">
          <svg v-if="savingProfile" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>
          {{ savingProfile ? 'Gemmer…' : 'Gem ændringer' }}
        </button>
      </div>

      <!-- ── Provider profile ── -->
      <div v-else-if="auth.isProvider" class="card mb-5" style="padding: 1.75rem;">
        <p class="section-label">Virksomhedsoplysninger</p>

        <div v-if="profileSaved" class="alert-success mb-4">
          <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          Ændringer gemt
        </div>
        <div v-if="profileError" class="alert-error mb-4">
          <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          {{ profileError }}
        </div>

        <div class="space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">Firmanavn</label>
              <input v-model="providerForm.companyName" class="input-field" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">Kontaktperson</label>
              <input v-model="providerForm.contactName" class="input-field" />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">E-mail</label>
            <input v-model="providerForm.email" type="email" class="input-field" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">Telefonnummer</label>
            <input v-model="providerForm.phoneNumber" class="input-field" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">Adresse</label>
            <input v-model="providerForm.address" class="input-field" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">By</label>
            <select v-model="providerForm.city" class="input-field">
              <option v-for="city in cities" :key="city" :value="city">{{ city }}</option>
            </select>
          </div>
        </div>
        <button @click="saveProviderProfile" :disabled="savingProfile" class="btn-primary mt-5">
          <svg v-if="savingProfile" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>
          {{ savingProfile ? 'Gemmer…' : 'Gem ændringer' }}
        </button>
      </div>

      <!-- ── Provider services ── -->
      <div v-if="auth.isProvider" class="card" style="padding: 1.75rem;">
        <p class="section-label">Mine ydelser</p>

        <div v-if="serviceError" class="alert-error mb-4">
          <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          {{ serviceError }}
        </div>

        <!-- Service list -->
        <div v-if="services.length" class="space-y-2 mb-5">
          <div v-for="svc in services" :key="svc.id"
               class="rounded-xl overflow-hidden transition-all duration-150"
               :style="editingService?.id === svc.id
                 ? 'border: 1.5px solid #c4b5fd; background: rgb(237 233 254 / 0.25);'
                 : 'border: 1.5px solid var(--border); background: var(--bg);'">
            <!-- Edit mode -->
            <template v-if="editingService?.id === svc.id">
              <div class="p-4 space-y-3">
                <input v-model="editingService.name" class="input-field text-sm" placeholder="Navn" />
                <textarea v-model="editingService.description" rows="2" class="input-field text-sm" placeholder="Beskrivelse" />
                <div class="relative">
                  <input v-model.number="editingService.basePrice" type="number" min="1" class="input-field text-sm pr-14" placeholder="Pris" />
                  <span class="absolute right-4 top-1/2 -translate-y-1/2 text-sm" style="color: var(--text-3);">DKK</span>
                </div>
                <div class="flex gap-2">
                  <button @click="saveEditService" class="btn-primary text-xs px-4 py-2">Gem ændringer</button>
                  <button @click="editingService = null" class="btn-secondary text-xs px-4 py-2">Annuller</button>
                </div>
              </div>
            </template>
            <!-- View mode -->
            <template v-else>
              <div class="flex items-center gap-3 px-4 py-3">
                <div class="flex-1 min-w-0">
                  <p class="font-semibold text-sm" style="color: var(--text-1);">{{ svc.name }}</p>
                  <p class="text-xs mt-0.5 line-clamp-1" style="color: var(--text-2);">{{ svc.description }}</p>
                </div>
                <p class="font-bold text-sm flex-shrink-0 mr-1" style="color: var(--violet-lt);">
                  {{ svc.basePrice.toFixed(0) }} <span class="font-normal text-xs" style="color: var(--text-3);">DKK</span>
                </p>
                <button @click="startEdit(svc)" class="btn-ghost text-xs px-2 py-1.5">Rediger</button>
                <button @click="deleteService(svc.id)"
                        class="text-xs px-2 py-1.5 rounded-lg font-medium transition-all duration-150"
                        style="color: #dc2626;"
                        onmouseenter="this.style.background='#fee2e2'" onmouseleave="this.style.background=''">
                  Slet
                </button>
              </div>
            </template>
          </div>
        </div>

        <div v-else class="flex items-center gap-2.5 text-sm italic mb-5 py-2" style="color: var(--text-3);">
          <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
          Ingen ydelser tilføjet endnu.
        </div>

        <!-- Add service form -->
        <div style="border-top: 1.5px solid var(--border); padding-top: 1.25rem;">
          <p class="text-sm font-semibold mb-3" style="color: var(--text-1);">Tilføj ny ydelse</p>
          <div class="space-y-3">
            <input v-model="newService.name" class="input-field" placeholder="Navn på ydelse" />
            <textarea v-model="newService.description" rows="2" class="input-field" placeholder="Beskrivelse af ydelsen" />
            <div class="grid grid-cols-2 gap-3">
              <div class="relative">
                <input v-model.number="newService.basePrice" type="number" min="1" class="input-field pr-14" placeholder="Pris" />
                <span class="absolute right-4 top-1/2 -translate-y-1/2 text-sm" style="color: var(--text-3);">DKK</span>
              </div>
              <select v-model="newService.categoryId" class="input-field">
                <option :value="null">Ingen kategori</option>
                <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
              </select>
            </div>
          </div>
          <button @click="addService" :disabled="addingService" class="btn-primary mt-4">
            <svg v-if="addingService" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>
            <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
            {{ addingService ? 'Tilføjer…' : 'Tilføj ydelse' }}
          </button>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'auth' })

import type { ClientDto, ServiceProviderDto, ProviderServiceListingDto, CategoryDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const auth = useAuthStore()

const cities = [
  'København', 'Aarhus', 'Odense', 'Aalborg', 'Esbjerg',
  'Randers', 'Kolding', 'Vejle', 'Horsens', 'Helsingør',
]

const { data: profile, pending } = await useAsyncData('my-profile', () =>
  $api<ClientDto | ServiceProviderDto>('/profile/me'))

const { data: categories } = await useAsyncData('categories', () =>
  $api<CategoryDto[]>('/categories'))

const clientForm = reactive({ fullName: '', email: '', phoneNumber: '' })
const providerForm = reactive({ companyName: '', contactName: '', email: '', phoneNumber: '', address: '', city: '' })
const services = ref<ProviderServiceListingDto[]>([])

watch(profile, (p) => {
  if (!p) return
  if (auth.isClient) {
    const c = p as ClientDto
    clientForm.fullName = c.fullName
    clientForm.email = c.email
    clientForm.phoneNumber = c.phoneNumber
  } else {
    const prov = p as ServiceProviderDto
    providerForm.companyName = prov.companyName
    providerForm.contactName = prov.contactName
    providerForm.email = prov.email
    providerForm.phoneNumber = prov.phoneNumber
    providerForm.address = prov.address
    providerForm.city = prov.city
    services.value = [...(prov.services ?? [])]
  }
}, { immediate: true })

const savingProfile = ref(false)
const profileSaved = ref(false)
const profileError = ref('')

async function saveClientProfile() {
  profileError.value = ''
  profileSaved.value = false
  savingProfile.value = true
  try {
    await $api('/profile/me', { method: 'PUT', body: clientForm })
    profileSaved.value = true
    setTimeout(() => { profileSaved.value = false }, 3000)
  } catch (e: unknown) {
    profileError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  } finally {
    savingProfile.value = false
  }
}

async function saveProviderProfile() {
  profileError.value = ''
  profileSaved.value = false
  savingProfile.value = true
  try {
    await $api('/profile/me', { method: 'PUT', body: providerForm })
    profileSaved.value = true
    setTimeout(() => { profileSaved.value = false }, 3000)
  } catch (e: unknown) {
    profileError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  } finally {
    savingProfile.value = false
  }
}

const newService = reactive({ name: '', description: '', basePrice: 0, categoryId: null as number | null })
const addingService = ref(false)
const serviceError = ref('')

async function addService() {
  serviceError.value = ''
  if (!newService.name || !newService.description || newService.basePrice <= 0) {
    serviceError.value = 'Udfyld navn, beskrivelse og pris.'
    return
  }
  addingService.value = true
  try {
    const added = await $api<ProviderServiceListingDto>('/profile/services', {
      method: 'POST',
      body: { name: newService.name, description: newService.description, basePrice: newService.basePrice, categoryId: newService.categoryId },
    })
    services.value.push(added)
    newService.name = ''
    newService.description = ''
    newService.basePrice = 0
    newService.categoryId = null
  } catch (e: unknown) {
    serviceError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  } finally {
    addingService.value = false
  }
}

const editingService = ref<{ id: string; name: string; description: string; basePrice: number } | null>(null)

function startEdit(svc: ProviderServiceListingDto) {
  editingService.value = { id: svc.id, name: svc.name, description: svc.description, basePrice: svc.basePrice }
}

async function saveEditService() {
  if (!editingService.value) return
  const { id, name, description, basePrice } = editingService.value
  try {
    const updated = await $api<ProviderServiceListingDto>(`/profile/services/${id}`, {
      method: 'PUT',
      body: { name, description, basePrice },
    })
    const idx = services.value.findIndex(s => s.id === id)
    if (idx !== -1) services.value[idx] = updated
    editingService.value = null
  } catch (e: unknown) {
    serviceError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  }
}

async function deleteService(id: string) {
  if (!confirm('Slet denne ydelse?')) return
  try {
    await $api(`/profile/services/${id}`, { method: 'DELETE' })
    services.value = services.value.filter(s => s.id !== id)
  } catch (e: unknown) {
    serviceError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  }
}
</script>
