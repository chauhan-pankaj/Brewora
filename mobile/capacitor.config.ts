import type { CapacitorConfig } from "@capacitor/cli";

const config: CapacitorConfig = {
  appId: "com.brewora.cafe",
  appName: "Brewora Café & Kitchen",
  webDir: "dist",
  server: {
    androidScheme: "https",
  },
  android: {
    allowMixedContent: true,
  },
  plugins: {
    StatusBar: {
      style: "DARK",
      backgroundColor: "#173620",
    },
    Keyboard: {
      resize: "native",
    },
  },
};

export default config;
