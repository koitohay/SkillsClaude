<template>
  <!-- FAB -->
  <button
    @click="open = !open"
    class="chat-fab"
    aria-label="Åbn AI Assistent"
  >
    <svg v-if="!open" class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
        d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"/>
    </svg>
    <svg v-else class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
    </svg>
    <span v-if="hasNewMessage && !open" class="chat-fab-dot"></span>
  </button>

  <!-- Panel -->
  <Transition name="chat-slide">
    <div v-if="open" class="chat-panel">
      <!-- Header -->
      <div class="chat-header">
        <div class="flex items-center gap-2.5">
          <div class="w-8 h-8 rounded-lg flex items-center justify-center" style="background: rgba(255,255,255,0.2);">
            <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"/>
            </svg>
          </div>
          <div>
            <p class="text-sm font-semibold text-white leading-none">AI Assistent</p>
            <p class="text-xs mt-0.5" style="color: rgba(255,255,255,0.7);">ServiceMatch DK</p>
          </div>
        </div>
        <button @click="open = false" class="text-white/70 hover:text-white transition-colors">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
          </svg>
        </button>
      </div>

      <!-- Messages -->
      <div ref="messageListEl" class="chat-messages">
        <div v-for="(msg, i) in messages" :key="i"
          :class="['chat-bubble-wrap', msg.role === 'user' ? 'user' : 'assistant']">
          <div :class="['chat-bubble', msg.role === 'user' ? 'chat-bubble-user' : 'chat-bubble-assistant']">
            <span style="white-space: pre-wrap;">{{ msg.content }}</span>
          </div>
        </div>

        <!-- Typing indicator -->
        <div v-if="loading" class="chat-bubble-wrap assistant">
          <div class="chat-bubble chat-bubble-assistant">
            <span class="typing-dots">
              <span></span><span></span><span></span>
            </span>
          </div>
        </div>
      </div>

      <!-- Input -->
      <div class="chat-input-row">
        <input
          v-model="input"
          @keydown.enter.prevent="send"
          :disabled="loading"
          class="input-field flex-1 text-sm"
          placeholder="Stil et spørgsmål…"
          autocomplete="off"
        />
        <button
          @click="send"
          :disabled="loading || !input.trim()"
          class="btn-primary px-4 py-2.5 text-sm flex-shrink-0"
          style="min-width: 72px;"
        >
          <svg v-if="loading" class="w-4 h-4 animate-spin mx-auto" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/>
          </svg>
          <span v-else>Send</span>
        </button>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
const { $api } = useNuxtApp()

const open = ref(false)
const loading = ref(false)
const input = ref('')
const hasNewMessage = ref(false)
const messageListEl = ref<HTMLElement | null>(null)

const messages = ref<{ role: 'user' | 'assistant'; content: string }[]>([
  {
    role: 'assistant',
    content: 'Hej! Jeg kan hjælpe dig med at finde den rigtige service i Danmark. Hvad leder du efter?'
  }
])

watch(open, (val) => {
  if (val) {
    hasNewMessage.value = false
    nextTick(scrollToBottom)
  }
})

async function send() {
  const text = input.value.trim()
  if (!text || loading.value) return

  messages.value.push({ role: 'user', content: text })
  input.value = ''
  loading.value = true
  nextTick(scrollToBottom)

  try {
    const { reply } = await $api<{ reply: string }>('/chat', {
      method: 'POST',
      body: { messages: messages.value }
    })
    messages.value.push({ role: 'assistant', content: reply })
    if (!open.value) hasNewMessage.value = true
  } catch {
    messages.value.push({ role: 'assistant', content: 'Beklager, der opstod en fejl. Prøv igen.' })
  } finally {
    loading.value = false
    nextTick(scrollToBottom)
  }
}

function scrollToBottom() {
  if (messageListEl.value)
    messageListEl.value.scrollTop = messageListEl.value.scrollHeight
}
</script>

<style scoped>
.chat-fab {
  position: fixed;
  bottom: 1.5rem;
  right: 1.5rem;
  z-index: 60;
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: linear-gradient(135deg, #7c3aed, #5b21b6);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 20px rgba(124, 58, 237, 0.45);
  transition: transform 0.15s ease, box-shadow 0.15s ease;
  border: none;
  cursor: pointer;
}
.chat-fab:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 28px rgba(124, 58, 237, 0.55);
}
.chat-fab-dot {
  position: absolute;
  top: 6px;
  right: 6px;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #f59e0b;
  border: 2px solid white;
  animation: pulse-dot 1.5s infinite;
}
@keyframes pulse-dot {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.3); }
}

.chat-panel {
  position: fixed;
  bottom: 5.5rem;
  right: 1.5rem;
  z-index: 60;
  width: 380px;
  max-height: 520px;
  display: flex;
  flex-direction: column;
  border-radius: 16px;
  overflow: hidden;
  background: rgba(255, 255, 255, 0.97);
  backdrop-filter: blur(12px);
  border: 1.5px solid var(--border);
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.12), 0 4px 16px rgba(124, 58, 237, 0.08);
}

@media (max-width: 480px) {
  .chat-panel {
    width: calc(100vw - 2rem);
    right: 1rem;
    bottom: 5rem;
  }
}

.chat-header {
  padding: 1rem 1.25rem;
  background: linear-gradient(135deg, #7c3aed, #5b21b6);
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-shrink: 0;
}

.chat-messages {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.625rem;
  background: var(--bg);
  scroll-behavior: smooth;
}
.chat-messages::-webkit-scrollbar { width: 4px; }
.chat-messages::-webkit-scrollbar-track { background: transparent; }
.chat-messages::-webkit-scrollbar-thumb { background: var(--border-md); border-radius: 4px; }

.chat-bubble-wrap {
  display: flex;
}
.chat-bubble-wrap.user { justify-content: flex-end; }
.chat-bubble-wrap.assistant { justify-content: flex-start; }

.chat-bubble {
  max-width: 82%;
  padding: 0.625rem 0.875rem;
  border-radius: 12px;
  font-size: 0.8125rem;
  line-height: 1.5;
}
.chat-bubble-user {
  background: linear-gradient(135deg, #7c3aed, #5b21b6);
  color: white;
  border-bottom-right-radius: 4px;
}
.chat-bubble-assistant {
  background: var(--surface);
  color: var(--text-1);
  border: 1.5px solid var(--border);
  border-bottom-left-radius: 4px;
}

.typing-dots {
  display: inline-flex;
  gap: 4px;
  align-items: center;
  height: 16px;
}
.typing-dots span {
  width: 6px;
  height: 6px;
  background: var(--text-3);
  border-radius: 50%;
  animation: typing-bounce 1.2s infinite;
}
.typing-dots span:nth-child(2) { animation-delay: 0.2s; }
.typing-dots span:nth-child(3) { animation-delay: 0.4s; }
@keyframes typing-bounce {
  0%, 60%, 100% { transform: translateY(0); }
  30% { transform: translateY(-5px); }
}

.chat-input-row {
  display: flex;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background: var(--surface);
  border-top: 1.5px solid var(--border);
  flex-shrink: 0;
}

/* Slide transition */
.chat-slide-enter-active,
.chat-slide-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}
.chat-slide-enter-from,
.chat-slide-leave-to {
  opacity: 0;
  transform: translateY(16px) scale(0.98);
}
</style>
