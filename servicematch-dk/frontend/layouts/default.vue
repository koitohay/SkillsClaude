<template>
  <div class="min-h-screen bg-gray-50">
    <nav class="sticky top-0 z-50 bg-white/90 backdrop-blur-md border-b border-gray-100 shadow-sm">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between gap-4">
        <!-- Logo -->
        <NuxtLink to="/" class="flex items-center gap-2 flex-shrink-0">
          <div class="w-8 h-8 rounded-xl bg-gradient-to-br from-violet-600 to-purple-500 flex items-center justify-center shadow-sm">
            <span class="text-white text-sm font-bold">S</span>
          </div>
          <span class="font-bold text-gray-900 hidden sm:block">ServiceMatch <span class="text-violet-500">DK</span></span>
        </NuxtLink>

        <div class="flex items-center gap-0.5">
          <template v-if="auth.isAuthenticated">
            <!-- Client links -->
            <template v-if="auth.isClient">
              <NuxtLink to="/dashboard" class="btn-ghost text-xs sm:text-sm">
                <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/></svg>
                <span class="hidden sm:block">Forespørgsler</span>
              </NuxtLink>
              <NuxtLink to="/bookings" class="btn-ghost text-xs sm:text-sm">
                <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
                <span class="hidden sm:block">Bookinger</span>
              </NuxtLink>
              <NuxtLink to="/booking/new" class="btn-primary text-xs sm:text-sm">
                <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
                <span class="hidden sm:block">Ny</span>
              </NuxtLink>
            </template>

            <!-- Provider links -->
            <template v-if="auth.isProvider">
              <NuxtLink to="/provider/dashboard" class="btn-ghost text-xs sm:text-sm">
                <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/></svg>
                <span class="hidden sm:block">Forespørgsler</span>
              </NuxtLink>
              <NuxtLink to="/provider/bookings" class="btn-ghost text-xs sm:text-sm">
                <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
                <span class="hidden sm:block">Bookinger</span>
              </NuxtLink>
            </template>

            <!-- Shared: settings + logout -->
            <NuxtLink to="/settings" class="btn-ghost text-xs sm:text-sm">
              <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/></svg>
              <span class="hidden sm:block">Indstillinger</span>
            </NuxtLink>

            <div class="w-px h-5 bg-gray-200 mx-1"></div>

            <button @click="logout" class="btn-ghost text-xs sm:text-sm text-gray-400 hover:text-red-500">
              <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/></svg>
              <span class="hidden sm:block">Log ud</span>
            </button>
          </template>

          <template v-else>
            <NuxtLink to="/auth/login" class="btn-ghost">Log ind</NuxtLink>
            <NuxtLink to="/auth/register" class="btn-primary">Tilmeld dig</NuxtLink>
          </template>
        </div>
      </div>
    </nav>

    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <slot />
    </main>

    <footer class="mt-16 border-t border-gray-100 bg-white">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 flex flex-col sm:flex-row items-center justify-between gap-3">
        <div class="flex items-center gap-2">
          <div class="w-6 h-6 rounded-lg bg-gradient-to-br from-violet-600 to-purple-500 flex items-center justify-center">
            <span class="text-white text-xs font-bold">S</span>
          </div>
          <span class="text-sm font-semibold text-gray-700">ServiceMatch DK</span>
        </div>
        <p class="text-xs text-gray-400">© 2026 ServiceMatch. Alle rettigheder forbeholdes.</p>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
const auth = useAuthStore()

async function logout() {
  auth.clear()
  await navigateTo('/')
}
</script>
