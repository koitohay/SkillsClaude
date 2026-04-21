<template>
  <div>
    <!-- Hero -->
    <section class="relative overflow-hidden rounded-3xl bg-gradient-to-br from-violet-600 via-purple-600 to-indigo-700 text-white px-8 py-20 mb-12 text-center">
      <div class="absolute inset-0 bg-[url('data:image/svg+xml,%3Csvg width=\'60\' height=\'60\' viewBox=\'0 0 60 60\' xmlns=\'http://www.w3.org/2000/svg\'%3E%3Cg fill=\'none\' fill-rule=\'evenodd\'%3E%3Cg fill=\'%23ffffff\' fill-opacity=\'0.05\'%3E%3Cpath d=\'M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z\'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E')] opacity-40"></div>
      <div class="relative">
        <div class="inline-flex items-center gap-2 bg-white/10 backdrop-blur-sm border border-white/20 rounded-full px-4 py-1.5 text-sm font-medium mb-6">
          <span class="w-2 h-2 rounded-full bg-green-400 animate-pulse"></span>
          Tilgængelig i hele Danmark
        </div>
        <h1 class="text-4xl sm:text-5xl font-bold mb-4 leading-tight">
          Find den rette service<br class="hidden sm:block" /> i Danmark
        </h1>
        <p class="text-lg text-violet-100 mb-8 max-w-xl mx-auto">
          Sammenlign priser fra verificerede udbydere. Forhandl direkte. Book nemt.
        </p>
        <!-- Search bar -->
        <form @submit.prevent="doSearch" class="flex gap-2 max-w-lg mx-auto mb-6">
          <input
            v-model="searchTerm"
            class="flex-1 bg-white/90 backdrop-blur-sm text-gray-900 placeholder-gray-400 rounded-xl px-4 py-3 text-sm font-medium focus:outline-none focus:ring-2 focus:ring-white/50"
            placeholder="Søg på ydelse eller udbyder…"
          />
          <button type="submit" class="bg-white text-violet-700 font-semibold px-5 py-3 rounded-xl hover:bg-violet-50 transition-all shadow-lg text-sm flex-shrink-0">
            Søg
          </button>
        </form>
        <div class="flex justify-center">
          <NuxtLink to="/auth/register-provider" class="inline-flex items-center justify-center gap-2 bg-white/10 backdrop-blur-sm border border-white/30 text-white font-semibold px-8 py-3 rounded-xl hover:bg-white/20 transition-all text-sm">
            Bliv udbyder
          </NuxtLink>
        </div>
      </div>
    </section>

    <!-- Stats -->
    <section class="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-12">
      <div class="card text-center">
        <p class="text-2xl font-bold text-violet-600">5+</p>
        <p class="text-sm text-gray-500 mt-1">Kategorier</p>
      </div>
      <div class="card text-center">
        <p class="text-2xl font-bold text-violet-600">50+</p>
        <p class="text-sm text-gray-500 mt-1">Udbydere</p>
      </div>
      <div class="card text-center">
        <p class="text-2xl font-bold text-violet-600">100%</p>
        <p class="text-sm text-gray-500 mt-1">Verificerede</p>
      </div>
    </section>

    <!-- Categories -->
    <section class="mb-12">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-xl font-bold text-gray-900">Vælg en kategori</h2>
        <NuxtLink to="/booking/new" class="text-sm text-violet-600 font-medium hover:text-violet-700">Se alle →</NuxtLink>
      </div>
      <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-4">
        <div
          v-for="cat in categories"
          :key="cat.id"
          class="card-hover text-center group"
          @click="browseCategory(cat)"
        >
          <div class="text-3xl mb-3 group-hover:scale-110 transition-transform">{{ catEmoji(cat.slug) }}</div>
          <div class="font-semibold text-gray-800 text-sm">{{ cat.name }}</div>
          <div class="mt-2 text-xs text-violet-500 font-medium opacity-0 group-hover:opacity-100 transition-opacity">Se udbydere →</div>
        </div>
      </div>
    </section>

    <!-- How it works -->
    <section class="bg-white rounded-3xl border border-gray-100 p-8 mb-12">
      <h2 class="text-xl font-bold text-gray-900 text-center mb-8">Sådan fungerer det</h2>
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-8">
        <div class="text-center">
          <div class="w-12 h-12 rounded-2xl bg-violet-100 flex items-center justify-center mx-auto mb-4">
            <svg class="w-6 h-6 text-violet-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"/></svg>
          </div>
          <h3 class="font-semibold text-gray-800 mb-2">1. Find service</h3>
          <p class="text-sm text-gray-500">Vælg kategori og se udbydere med priser og beskrivelser</p>
        </div>
        <div class="text-center">
          <div class="w-12 h-12 rounded-2xl bg-violet-100 flex items-center justify-center mx-auto mb-4">
            <svg class="w-6 h-6 text-violet-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"/></svg>
          </div>
          <h3 class="font-semibold text-gray-800 mb-2">2. Send forespørgsel</h3>
          <p class="text-sm text-gray-500">Beskriv hvad du har brug for og hvornår</p>
        </div>
        <div class="text-center">
          <div class="w-12 h-12 rounded-2xl bg-violet-100 flex items-center justify-center mx-auto mb-4">
            <svg class="w-6 h-6 text-violet-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          </div>
          <h3 class="font-semibold text-gray-800 mb-2">3. Accepter og book</h3>
          <p class="text-sm text-gray-500">Sammenlign tilbud, forhandl priser og book</p>
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
