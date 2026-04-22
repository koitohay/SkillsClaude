<template>
  <div>

    <!-- ── Hero ──────────────────────────────────────────────── -->
    <section class="relative overflow-hidden rounded-3xl mb-12"
             style="background: linear-gradient(135deg, #1e1040 0%, #3b0764 50%, #4c1d95 100%); min-height: 480px;">

      <!-- Geometric accent — top-right -->
      <div class="absolute top-0 right-0 w-96 h-96 opacity-20 pointer-events-none"
           style="background: radial-gradient(circle at 80% 20%, #a78bfa 0%, transparent 60%);"></div>
      <!-- Dot grid -->
      <div class="absolute inset-0 opacity-10 pointer-events-none"
           style="background-image: radial-gradient(circle, #c4b5fd 1px, transparent 1px); background-size: 32px 32px;"></div>
      <!-- Bottom fade -->
      <div class="absolute bottom-0 left-0 right-0 h-32 pointer-events-none"
           style="background: linear-gradient(to top, rgb(76 29 149 / 0.6), transparent);"></div>

      <div class="relative px-8 py-20 flex flex-col items-center text-center">

        <!-- Pill badge -->
        <div class="inline-flex items-center gap-2 rounded-full px-4 py-1.5 text-xs font-medium mb-8 border"
             style="background: rgb(255 255 255 / 0.08); border-color: rgb(255 255 255 / 0.15); color: #c4b5fd; backdrop-filter: blur(4px);">
          <span class="w-1.5 h-1.5 rounded-full bg-emerald-400 animate-pulse"></span>
          Tilgængelig i hele Danmark
        </div>

        <!-- Headline -->
        <h1 class="text-white mb-3 leading-tight" style="font-size: clamp(2rem, 5vw, 3.5rem); font-weight: 700; letter-spacing: -0.03em; max-width: 640px;">
          Find den rette service<br>
          <span class="display-serif" style="color: #c4b5fd;">i Danmark</span>
        </h1>
        <p class="mb-10 max-w-md" style="color: rgb(196 181 253 / 0.8); font-size: 1.0625rem; line-height: 1.65;">
          Sammenlign priser fra verificerede udbydere. Forhandl direkte. Book nemt.
        </p>

        <!-- Search bar -->
        <form @submit.prevent="doSearch" class="flex gap-2 w-full max-w-lg mb-8">
          <div class="flex-1 relative">
            <svg class="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 pointer-events-none" style="color: rgb(139 92 246 / 0.6);" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"/></svg>
            <input
              v-model="searchTerm"
              class="w-full pl-11 pr-4 py-3 rounded-xl text-sm font-medium focus:outline-none"
              style="background: rgb(255 255 255 / 0.95); color: #1a1714; box-shadow: 0 4px 24px rgb(0 0 0 / 0.15); border: none;"
              placeholder="Søg på ydelse eller udbyder…"
            />
          </div>
          <button type="submit"
                  class="px-6 py-3 rounded-xl text-sm font-semibold flex-shrink-0 transition-all duration-150"
                  style="background: rgb(255 255 255 / 0.15); color: white; border: 1.5px solid rgb(255 255 255 / 0.25); backdrop-filter: blur(4px);"
                  onmouseenter="this.style.background='rgb(255 255 255 / 0.25)'"
                  onmouseleave="this.style.background='rgb(255 255 255 / 0.15)'">
            Søg
          </button>
        </form>

        <!-- Secondary CTA -->
        <NuxtLink to="/auth/register-provider"
                  class="inline-flex items-center gap-2 text-sm font-medium transition-all duration-150"
                  style="color: rgb(196 181 253 / 0.7);"
                  onmouseenter="this.style.color='#c4b5fd'"
                  onmouseleave="this.style.color='rgb(196 181 253 / 0.7)'">
          Er du udbyder? Tilmeld gratis
          <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/></svg>
        </NuxtLink>
      </div>
    </section>

    <!-- ── Stats strip ───────────────────────────────────────── -->
    <section class="grid grid-cols-3 gap-3 mb-14">
      <div class="card text-center py-5">
        <p class="text-3xl font-bold leading-none mb-1" style="color: var(--violet-lt); letter-spacing: -0.04em;">5+</p>
        <p class="text-xs font-medium uppercase tracking-wider" style="color: var(--text-3);">Kategorier</p>
      </div>
      <div class="card text-center py-5">
        <p class="text-3xl font-bold leading-none mb-1" style="color: var(--violet-lt); letter-spacing: -0.04em;">50+</p>
        <p class="text-xs font-medium uppercase tracking-wider" style="color: var(--text-3);">Udbydere</p>
      </div>
      <div class="card text-center py-5">
        <p class="text-3xl font-bold leading-none mb-1" style="color: var(--violet-lt); letter-spacing: -0.04em;">100%</p>
        <p class="text-xs font-medium uppercase tracking-wider" style="color: var(--text-3);">Verificerede</p>
      </div>
    </section>

    <!-- ── Categories ───────────────────────────────────────── -->
    <section class="mb-14">
      <div class="flex items-end justify-between mb-6">
        <div>
          <h2 class="text-xl font-bold" style="color: var(--text-1); letter-spacing: -0.025em;">Vælg en kategori</h2>
          <p class="text-sm mt-0.5" style="color: var(--text-2);">Browse verificerede udbydere</p>
        </div>
        <NuxtLink to="/booking/new" class="text-sm font-semibold transition-colors duration-150" style="color: var(--violet-lt);"
                  onmouseenter="this.style.color='var(--violet-dk)'"
                  onmouseleave="this.style.color='var(--violet-lt)'">
          Se alle →
        </NuxtLink>
      </div>
      <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-3">
        <button
          v-for="cat in categories"
          :key="cat.id"
          @click="browseCategory(cat)"
          class="card-hover text-center py-7 group flex flex-col items-center"
        >
          <div class="text-3xl mb-3 transition-transform duration-200 group-hover:scale-110">{{ catEmoji(cat.slug) }}</div>
          <div class="text-sm font-semibold transition-colors duration-150 group-hover:text-violet-700" style="color: var(--text-1);">{{ cat.name }}</div>
          <div class="text-xs font-medium mt-1.5 opacity-0 group-hover:opacity-100 transition-opacity duration-150" style="color: var(--violet-lt);">Se udbydere →</div>
        </button>
      </div>
    </section>

    <!-- ── How it works ──────────────────────────────────────── -->
    <section class="rounded-3xl p-10 mb-12 relative overflow-hidden"
             style="background: var(--surface); border: 1.5px solid var(--border); box-shadow: var(--shadow-sm);">

      <!-- Subtle accent line top -->
      <div class="absolute top-0 left-16 right-16 h-px" style="background: linear-gradient(90deg, transparent, #c4b5fd, transparent);"></div>

      <div class="text-center mb-10">
        <p class="text-xs font-semibold uppercase tracking-widest mb-2" style="color: var(--violet-lt);">Sådan fungerer det</p>
        <h2 class="text-2xl font-bold" style="color: var(--text-1); letter-spacing: -0.025em;">
          Tre trin til din
          <span class="display-serif"> perfekte service</span>
        </h2>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-3 gap-8 relative">
        <!-- Step 1 -->
        <div class="text-center">
          <div class="w-14 h-14 rounded-2xl flex items-center justify-center mx-auto mb-5"
               style="background: linear-gradient(135deg, #7c3aed 0%, #5b21b6 100%); box-shadow: var(--shadow-v);">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"/></svg>
          </div>
          <p class="text-xs font-bold uppercase tracking-wide mb-2" style="color: var(--violet-lt);">Trin 1</p>
          <h3 class="font-semibold mb-2" style="color: var(--text-1);">Find service</h3>
          <p class="text-sm leading-relaxed" style="color: var(--text-2);">Vælg kategori og se udbydere med priser og beskrivelser</p>
        </div>
        <!-- Step 2 -->
        <div class="text-center">
          <div class="w-14 h-14 rounded-2xl flex items-center justify-center mx-auto mb-5"
               style="background: linear-gradient(135deg, #7c3aed 0%, #5b21b6 100%); box-shadow: var(--shadow-v);">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"/></svg>
          </div>
          <p class="text-xs font-bold uppercase tracking-wide mb-2" style="color: var(--violet-lt);">Trin 2</p>
          <h3 class="font-semibold mb-2" style="color: var(--text-1);">Send forespørgsel</h3>
          <p class="text-sm leading-relaxed" style="color: var(--text-2);">Beskriv hvad du har brug for og hvornår du ønsker det</p>
        </div>
        <!-- Step 3 -->
        <div class="text-center">
          <div class="w-14 h-14 rounded-2xl flex items-center justify-center mx-auto mb-5"
               style="background: linear-gradient(135deg, #7c3aed 0%, #5b21b6 100%); box-shadow: var(--shadow-v);">
            <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          </div>
          <p class="text-xs font-bold uppercase tracking-wide mb-2" style="color: var(--violet-lt);">Trin 3</p>
          <h3 class="font-semibold mb-2" style="color: var(--text-1);">Accepter og book</h3>
          <p class="text-sm leading-relaxed" style="color: var(--text-2);">Sammenlign tilbud, forhandl priser og bekræft din booking</p>
        </div>
      </div>
    </section>

  </div>
</template>

<script setup lang="ts">
import type { CategoryDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const router = useRouter()

const { data: categories } = await useAsyncData('categories', () =>
  $api<CategoryDto[]>('/categories'))

const searchTerm = ref('')

function doSearch() {
  if (searchTerm.value.trim())
    router.push(`/search?q=${encodeURIComponent(searchTerm.value.trim())}`)
}

function catEmoji(slug: string): string {
  const map: Record<string, string> = {
    salon: '✂️', nails: '💅', massage: '💆', dentist: '🦷', kiropraktor: '🦴'
  }
  return map[slug] ?? '🔧'
}

function browseCategory(cat: CategoryDto) {
  router.push(`/browse/${cat.id}`)
}
</script>
