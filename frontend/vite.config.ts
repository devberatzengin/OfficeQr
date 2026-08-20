import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl'

// https://vite.dev/config/
export default defineConfig({
  // basicSsl: kamera erişimi (getUserMedia) tarayıcılarda yalnızca "secure
  // context" (https ya da localhost) üzerinde çalışır. Telefondan LAN IP'siyle
  // (https://<bilgisayarın-ip'si>:5173) açabilmek için self-signed sertifika
  // ile otomatik https sağlıyoruz — telefon ilk açılışta sertifikayı kabul
  // etmeni isteyecek, bu normal.
  plugins: [react(), basicSsl()],
  server: {
    host: true,
    proxy: {
      '/api': { target: 'http://localhost:5188', changeOrigin: true },
      '/identity': { target: 'http://localhost:5188', changeOrigin: true },
    },
  },
})
