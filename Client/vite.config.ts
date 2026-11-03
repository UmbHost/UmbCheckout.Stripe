import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: { "umbcheckout-stripe": "src/bundle.manifests.ts" },
      formats: ["es"],
      fileName: (_format, name) => `${name}.js`,
    },
    outDir: "../wwwroot/App_Plugins/UmbCheckout.Stripe",
    emptyOutDir: true,
    sourcemap: true,
    rollupOptions: {
      // @umbcheckout/core resolves at runtime through the core package's importmap.
      external: [/^@umbraco/, /^@umbcheckout\//],
    },
  },
});
