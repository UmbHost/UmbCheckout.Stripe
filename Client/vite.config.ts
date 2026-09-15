import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/bundle.manifests.ts", // Bundle registers one or more manifests
      formats: ["es"],
      fileName: "umbcheckout-stripe",
    },
    outDir: "../wwwroot/UmbCheckout.Stripe", // Served from /App_Plugins/UmbCheckout.Stripe
    emptyOutDir: true,
    sourcemap: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
