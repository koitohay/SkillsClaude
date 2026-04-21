export default defineNuxtRouteMiddleware(() => {
  const auth = useAuthStore()
  if (!auth.isAuthenticated) return navigateTo('/auth/login')
  if (!auth.isProvider) return navigateTo('/dashboard')
})
