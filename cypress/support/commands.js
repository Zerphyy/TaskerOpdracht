// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
Cypress.Commands.add('login', (email, password, stayLoggedIn = true) => {
    cy.visit('http://145.44.234.91/Access/Login')

    cy.get('input#Email').type(email)
    cy.get('input#Password').type(password)
    
    if (stayLoggedIn) {
        cy.contains('Keep me logged in.').click()
    }
    cy.contains('button', 'Login').click()
    cy.contains('a', 'Uitloggen').should('be.visible')
});