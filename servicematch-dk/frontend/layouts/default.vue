<template>
  <div class="min-h-screen" style="background: var(--bg)">

    <!-- ── Navbar ──────────────────────────────────────────── -->
    <nav class="sticky top-0 z-50"
         style="background: rgb(255 255 255 / 0.92); backdrop-filter: blur(12px); border-bottom: 1.5px solid var(--border);">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">

        <!-- Logo -->
        <NuxtLink to="/" class="flex items-center gap-2.5 flex-shrink-0 group">
          <div class="w-8 h-8 rounded-xl flex items-center justify-center flex-shrink-0 transition-transform duration-200 group-hover:scale-105"
               style="background: linear-gradient(135deg, #7c3aed 0%, #5b21b6 100%); box-shadow: 0 2px 8px rgb(109 40 217 / 0.25);">
            <span class="text-white text-sm font-bold tracking-tight">S</span>
          </div>
          <span class="font-semibold hidden sm:block" style="color: var(--text-1); letter-spacing: -0.02em;">
            ServiceMatch <span style="color: var(--violet-lt);">DK</span>
          </span>
        </NuxtLink>

        <!-- Desktop nav -->
        <div class="hidden sm:flex items-center gap-1">
          <template v-if="auth.isAuthenticated">
            <template v-if="auth.isClient">
              <NuxtLink to="/dashboard"     class="nav-link" :class="{ active: $route.path === '/dashboard' }">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/></svg>
                Forespørgsler
              </NuxtLink>
              <NuxtLink to="/bookings"      class="nav-link" :class="{ active: $route.path === '/bookings' }">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
                Bookinger
              </NuxtLink>
            </template>
            <template v-if="auth.isProvider">
              <NuxtLink to="/provider/dashboard" class="nav-link" :class="{ active: $route.path === '/provider/dashboard' }">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/></svg>
                Forespørgsler
              </NuxtLink>
              <NuxtLink to="/provider/bookings"  class="nav-link" :class="{ active: $route.path === '/provider/bookings' }">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
                Bookinger
              </NuxtLink>
            </template>

            <div class="w-px h-5 mx-2" style="background: var(--border);"></div>

            <NuxtLink to="/settings" class="nav-link" :class="{ active: $route.path === '/settings' }">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/></svg>
              Indstillinger
            </NuxtLink>

            <!-- New request CTA (client only) -->
            <NuxtLink v-if="auth.isClient" to="/booking/new" class="btn-primary ml-2">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
              Ny forespørgsel
            </NuxtLink>

            <button @click="logout" class="nav-link ml-1" style="color: var(--text-3);"
                    onmouseenter="this.style.color='#dc2626'; this.style.background='rgb(254 242 242)'"
                    onmouseleave="this.style.color=''; this.style.background=''">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/></svg>
            </button>
          </template>

          <template v-else>
            <NuxtLink to="/auth/login" class="btn-secondary">Log ind</NuxtLink>
            <NuxtLink to="/auth/register" class="btn-primary ml-2">Tilmeld dig</NuxtLink>
          </template>
        </div>

        <!-- Mobile hamburger -->
        <button
          @click="mobileOpen = !mobileOpen"
          class="sm:hidden w-9 h-9 flex items-center justify-center rounded-xl transition-all duration-150"
          :style="mobileOpen
            ? 'background: rgb(124 58 237 / 0.08); color: var(--violet-lt);'
            : 'color: var(--text-2);'"
          aria-label="Menu"
        >
          <svg v-if="!mobileOpen" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/>
          </svg>
          <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
          </svg>
        </button>
      </div>

      <!-- Mobile drawer -->
      <transition
        enter-active-class="transition-all duration-200 ease-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-150 ease-in"
        leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 -translate-y-2"
      >
        <div v-if="mobileOpen" class="sm:hidden px-4 pb-4 pt-2" style="border-top: 1.5px solid var(--border);">
          <template v-if="auth.isAuthenticated">
            <template v-if="auth.isClient">
              <NuxtLink to="/dashboard"    @click="mobileOpen = false" class="mobile-nav-link" :class="{ active: $route.path === '/dashboard' }">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/></svg>
                Forespørgsler
              </NuxtLink>
              <NuxtLink to="/bookings"     @click="mobileOpen = false" class="mobile-nav-link" :class="{ active: $route.path === '/bookings' }">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
                Bookinger
              </NuxtLink>
              <NuxtLink to="/booking/new"  @click="mobileOpen = false" class="mobile-nav-link" style="color: var(--violet-lt); font-weight: 600;">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
                Ny forespørgsel
              </NuxtLink>
            </template>
            <template v-if="auth.isProvider">
              <NuxtLink to="/provider/dashboard" @click="mobileOpen = false" class="mobile-nav-link" :class="{ active: $route.path === '/provider/dashboard' }">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/></svg>
                Forespørgsler
              </NuxtLink>
              <NuxtLink to="/provider/bookings"  @click="mobileOpen = false" class="mobile-nav-link" :class="{ active: $route.path === '/provider/bookings' }">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
                Bookinger
              </NuxtLink>
            </template>

            <div class="my-2" style="border-top: 1.5px solid var(--border);"></div>

            <NuxtLink to="/settings" @click="mobileOpen = false" class="mobile-nav-link" :class="{ active: $route.path === '/settings' }">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/></svg>
              Indstillinger
            </NuxtLink>
            <button @click="logout; mobileOpen = false" class="mobile-nav-link" style="color: #dc2626;">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/></svg>
              Log ud
            </button>
          </template>
          <template v-else>
            <NuxtLink to="/auth/login"    @click="mobileOpen = false" class="mobile-nav-link">Log ind</NuxtLink>
            <NuxtLink to="/auth/register" @click="mobileOpen = false" class="mobile-nav-link" style="color: var(--violet-lt); font-weight: 600;">Tilmeld dig</NuxtLink>
          </template>
        </div>
      </transition>
    </nav>

    <!-- ── Page content ─────────────────────────────────────── -->
    <main class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <slot />
    </main>

    <!-- ── Footer ───────────────────────────────────────────── -->
    <footer class="mt-16" style="border-top: 1.5px solid var(--border); background: var(--surface);">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 flex flex-col sm:flex-row items-center justify-between gap-3">
        <div class="flex items-center gap-2">
          <div class="w-6 h-6 rounded-lg flex items-center justify-center"
               style="background: linear-gradient(135deg, #7c3aed 0%, #5b21b6 100%);">
            <span class="text-white text-xs font-bold">S</span>
          </div>
          <span class="text-sm font-semibold" style="color: var(--text-1);">ServiceMatch DK</span>
        </div>
        <p class="text-xs" style="color: var(--text-3);">© 2026 ServiceMatch. Alle rettigheder forbeholdes.</p>
      </div>
    </footer>

    <ChatWidget />
  </div>
</template>

<script setup lang="ts">
const auth = useAuthStore()
const route = useRoute()
const mobileOpen = ref(false)

watch(() => route.path, () => { mobileOpen.value = false })

async function logout() {
  auth.clear()
  await navigateTo('/')
}
</script>
