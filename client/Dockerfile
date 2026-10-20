FROM oven/bun:1

WORKDIR /app

COPY package.json bun.lock ./
RUN bun install --frozen-lockfile

COPY . .

ENV NODE_ENV=production
ENV PORT=3000

USER bun

EXPOSE 3000

CMD ["bun", "src/index.ts"]

