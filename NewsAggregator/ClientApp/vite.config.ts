import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const isDevelopment = process.env.NODE_ENV === "development"

export default defineConfig({
  plugins: [react()],
  base: "/news",
  server: {
    port: isDevelopment ? 40081 : undefined
  },
  build: {
    outDir: "build"
  }
})
