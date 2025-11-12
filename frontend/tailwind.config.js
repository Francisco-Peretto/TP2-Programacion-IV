/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,jsx,ts,tsx}"],
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        bg: "#0f1115",
        surface: "#151924",
        muted: "#9aa4b2",
        text: "#e6e9ef",
        brand: "#6ea8fe",
        ok: "#3ddc97",
        danger: "#ff6b6b",
        border: "#22283a",
      },
      boxShadow: {
        "brand-glow": "0 0 14px #6ea8fe",
      },
      container: { center: true, padding: "1rem" },
    },
  },
  plugins: [],
};
