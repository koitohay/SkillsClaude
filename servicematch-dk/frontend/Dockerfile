FROM node:22-alpine AS build
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm ci
COPY . .
RUN npx nuxt prepare && npm run build

FROM node:22-alpine AS final
WORKDIR /app
COPY --from=build /app/.output .
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser
EXPOSE 3000
ENV PORT=3000
ENV NODE_ENV=production
CMD ["node", "server/index.mjs"]
