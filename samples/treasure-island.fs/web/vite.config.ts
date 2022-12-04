// vite.config.js
import { defineConfig } from "vite"
import react from "@vitejs/plugin-react"

export default defineConfig({
  plugins: [
    react({
      jsxImportSource: "@emotion/react",
      babel: {
        plugins: ["@emotion/babel-plugin"],
      },
    }),
  ],
  build: {
    sourcemap: true,
    target: "es2015",
    minify: false,
  },
  esbuild: {
    jsxFactory: `jsx`,
    // jsxInject: `import { jsx, css } from '@emotion/react'`,
  },
  optimizeDeps: {
    include: ["react/jsx-runtime"],
  },

  server: {
    port: 3004,
    open: false,
    hmr: {
      clientPort: 5004,
    },
  },
})
// vite.config.js
