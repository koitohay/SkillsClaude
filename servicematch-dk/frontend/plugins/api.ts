import { $fetch } from 'ofetch'

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  const authStore = useAuthStore()

  const api = $fetch.create({
    baseURL: config.public.apiBaseUrl,
    onRequest({ options }) {
      const token = authStore.token
      if (token) {
        options.headers = new Headers(options.headers as HeadersInit)
        options.headers.set('Authorization', `Bearer ${token}`)
      }
    },
    onResponseError({ response }) {
      if (response.status === 401) {
        authStore.clear()
        navigateTo('/auth/login')
      }
    },
  })

  return {
    provide: { api },
  }
})
