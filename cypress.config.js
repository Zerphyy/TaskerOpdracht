const { defineConfig } = require('cypress')

module.exports = defineConfig({
  e2e: {
    specPattern: 'TestShowcase/CypressFrontEndTests/e2e/**/*.cy.{js,jsx,ts,tsx}', // Your new spec location
  },
  // You can similarly adjust other paths here if needed
})
