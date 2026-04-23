import type { Config } from 'tailwindcss'

export default {
  content: [
    './components/**/*.{vue,js,ts}',
    './layouts/**/*.vue',
    './pages/**/*.vue',
    './plugins/**/*.{js,ts}',
    './app.vue',
    './assets/css/**/*.css',
  ],
  theme: {
    extend: {},
  },
  plugins: [],
} satisfies Config
