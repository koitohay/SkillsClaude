export default defineNuxtConfig({
  compatibilityDate: '2024-11-01',
  devtools: { enabled: true },
  ssr: true,

  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
  ],

  runtimeConfig: {
    // Server-only
    jwtSecret: '',
    // Public (exposed to client)
    public: {
      apiBaseUrl: 'http://localhost:5000/api/v1',
    },
  },

  typescript: {
    strict: true,
    typeCheck: true,
  },
})
