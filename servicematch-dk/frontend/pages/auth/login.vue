<template>
  <div class="min-h-[80vh] flex items-center justify-center py-12">
    <div class="w-full max-w-md">

      <!-- Brand -->
      <div class="text-center mb-10">
        <div class="inline-flex items-center justify-center w-14 h-14 rounded-2xl mb-5 mx-auto"
             style="background: linear-gradient(135deg, #7c3aed 0%, #5b21b6 100%); box-shadow: var(--shadow-v);">
          <span class="text-white text-xl font-bold">S</span>
        </div>
        <h1 class="text-3xl font-bold mb-1" style="color: var(--text-1); letter-spacing: -0.03em;">
          Velkommen tilbage
        </h1>
        <p class="text-sm" style="color: var(--text-2);">Log ind på din ServiceMatch konto</p>
      </div>

      <!-- Card -->
      <div class="card" style="padding: 2rem;">
        <form @submit.prevent="submit" class="space-y-5">
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">E-mail</label>
            <input v-model="form.email" type="email" required autocomplete="email"
                   class="input-field" placeholder="din@email.dk" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">Adgangskode</label>
            <input v-model="form.password" type="password" required autocomplete="current-password"
                   class="input-field" placeholder="••••••••" />
          </div>

          <div v-if="error" class="alert-error">
            <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
            {{ error }}
          </div>

          <button type="submit" :disabled="loading" class="btn-primary w-full py-3 mt-1">
            <svg v-if="loading" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/>
            </svg>
            {{ loading ? 'Logger ind…' : 'Log ind' }}
          </button>
        </form>

        <div class="divider"></div>

        <div class="text-center space-y-2.5">
          <p class="text-sm" style="color: var(--text-2);">
            Ingen konto?
            <NuxtLink to="/auth/register" class="font-semibold transition-colors duration-150" style="color: var(--violet-lt);"
                      onmouseenter="this.style.color='var(--violet-dk)'" onmouseleave="this.style.color='var(--violet-lt)'">
              Tilmeld som kunde
            </NuxtLink>
          </p>
          <p class="text-sm" style="color: var(--text-2);">
            Er du udbyder?
            <NuxtLink to="/auth/register-provider" class="font-semibold transition-colors duration-150" style="color: var(--violet-lt);"
                      onmouseenter="this.style.color='var(--violet-dk)'" onmouseleave="this.style.color='var(--violet-lt)'">
              Tilmeld som udbyder
            </NuxtLink>
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
