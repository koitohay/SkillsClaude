import { defineStore } from 'pinia'
import type { AuthTokenDto } from '~/types/api.types'

export const useAuthStore = defineStore('auth', () => {
  // Cookie refs for persistence — never exposed to Pinia state (CookieRef is a Proxy,
  // which causes devalue to crash when Pinia tries to serialize it for SSR hydration)
  const tokenCookie = useCookie<string | null>('auth_token', { default: () => null, maxAge: 60 * 60 * 24 * 7 })
  const roleCookie = useCookie<'Client' | 'Provider' | null>('auth_role', { default: () => null, maxAge: 60 * 60 * 24 * 7 })
  const userIdCookie = useCookie<string | null>('auth_userId', { default: () => null, maxAge: 60 * 60 * 24 * 7 })

  // Plain refs Pinia can serialize — initialized from cookies so SSR reads the right values
  const token = ref<string | null>(tokenCookie.value)
  const role = ref<'Client' | 'Provider' | null>(roleCookie.value)
  const userId = ref<string | null>(userIdCookie.value)

  // Keep cookies in sync when refs change
  watch(token, v => { tokenCookie.value = v })
  watch(role, v => { roleCookie.value = v })
  watch(userId, v => { userIdCookie.value = v })

  const isAuthenticated = computed(() => !!token.value)
  const isClient = computed(() => role.value === 'Client')
  const isProvider = computed(() => role.value === 'Provider')

  function setAuth(dto: AuthTokenDto) {
    token.value = dto.token
    role.value = dto.role
    userId.value = dto.userId
  }

  function clear() {
    token.value = null
    role.value = null
    userId.value = null
  }

  return { token, role, userId, isAuthenticated, isClient, isProvider, setAuth, clear }
})
