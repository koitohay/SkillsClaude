<template>
  <div class="min-h-[70vh] flex items-center justify-center py-12">
    <div class="w-full max-w-sm">
      <!-- Brand mark -->
      <div class="text-center mb-8">
        <div class="w-12 h-12 rounded-2xl bg-gradient-to-br from-violet-600 to-purple-500 flex items-center justify-center mx-auto mb-4 shadow-lg">
          <span class="text-white text-xl font-bold">S</span>
        </div>
        <h1 class="text-2xl font-bold text-gray-900">Velkommen tilbage</h1>
        <p class="text-sm text-gray-500 mt-1">Log ind på din ServiceMatch konto</p>
      </div>

      <div class="card">
        <form @submit.prevent="submit" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">E-mail</label>
            <input v-model="form.email" type="email" required autocomplete="email" class="input-field" placeholder="din@email.dk" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Adgangskode</label>
            <input v-model="form.password" type="password" required autocomplete="current-password" class="input-field" placeholder="••••••••" />
          </div>

          <div v-if="error" class="alert-error">
            <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
            {{ error }}
          </div>

          <button type="submit" :disabled="loading" class="btn-primary w-full py-3 mt-2">
            <svg v-if="loading" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>
            {{ loading ? 'Logger ind…' : 'Log ind' }}
          </button>
        </form>

        <div class="divider"></div>

        <div class="text-center space-y-2">
          <p class="text-sm text-gray-500">Ingen konto?
            <NuxtLink to="/auth/register" class="text-violet-600 font-medium hover:text-violet-700">Tilmeld som kunde</NuxtLink>
          </p>
          <p class="text-sm text-gray-500">Er du udbyder?
            <NuxtLink to="/auth/register-provider" class="text-violet-600 font-medium hover:text-violet-700">Tilmeld som udbyder</NuxtLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const { $api } = useNuxtApp()
const authStore = useAuthStore()

const form = reactive({ email: '', password: '' })
const error = ref('')
const loading = ref(false)

async function submit() {
  error.value = ''
  loading.value = true
  try {
    const auth = await $api<{ token: string; role: string; userId: string }>('/auth/login', {
      method: 'POST',
      body: form,
    })
    authStore.setAuth({ token: auth.token, role: auth.role as 'Client' | 'Provider', userId: auth.userId })
    await navigateTo(auth.role === 'Provider' ? '/provider/dashboard' : '/dashboard')
  } catch {
    error.value = 'Forkert e-mail eller adgangskode.'
  } finally {
    loading.value = false
  }
}
</script>
