import path from 'path'
import tailwindcss from '@tailwindcss/vite'
import { defineConfig } from 'vite'
import react, { reactCompilerPreset } from '@vitejs/plugin-react'
import babel from '@rolldown/plugin-babel'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    babel({
      presets: [reactCompilerPreset()],
      include: /src\/.+\.[jt]sx?$/,
    }),
    tailwindcss(),
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  build: {
    rollupOptions: {
      output: {
        manualChunks(id) {
          if (!id.includes('node_modules')) return

          if (id.includes('@tiptap/') || id.includes('prosemirror')) return 'vendor-tiptap'
          if (id.includes('@tanstack/')) return 'vendor-tanstack'
          if (id.includes('react-dom') || id.includes('react-router') || id.includes('react/')) return 'vendor-react'
          if (id.includes('i18next') || id.includes('react-i18next')) return 'vendor-i18n'
          if (id.includes('@radix-ui/') || id.includes('lucide-react') || id.includes('cmdk') || id.includes('vaul')) return 'vendor-ui'

          return 'vendor'
        },
      },
    },
  },
})
