// vite.config.js
import { defineConfig } from "vite"
import react from "@vitejs/plugin-react"

export default defineConfig({
  plugins: [react({})],
  build: {
    sourcemap: true,
    outDir: "../planner1/app/web/build/",
    target: "es2015",
    minify: false,
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

// import reactRefresh from "@vitejs/plugin-react-refresh";
// import react from "@vitejs/plugin-react";
// import { defineConfig } from "vite";

// import { name, version } from "./package.json";

// export default defineConfig({
//   define: {
//     pkgJson: { name, version },
//   },
//   esbuild: {
//     // jsxInject: `import React from 'react'`,
//   },
//   plugins: [react()],
//   server: {
//     port: 3003,
//     open: false,
//     hmr: {
//       clientPort: 5003,
//     },
//   },
// });
