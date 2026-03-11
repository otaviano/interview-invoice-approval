import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'
import { createVuetify } from 'vuetify'

export default createVuetify({
  theme: {
    defaultTheme: 'light',
    themes: {
      light: {
        colors: {
          primary: '#1565C0',
          secondary: '#42A5F5',
          accent: '#FF6F00',
          success: '#2E7D32',
          warning: '#F57F17',
          error: '#C62828',
        },
      },
    },
  },
})
