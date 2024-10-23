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
Cypress.Commands.add('login', (email, password, lm, stayLoggedIn = true, bypass2FA = false) => {
    cy.visit('http://145.44.234.91/Access/Login');

    // Enter email and password
    cy.get('input#Email').type(email);
    cy.get('input#Password').type(password);
    
    // Handle "Keep me logged in" option
    if (stayLoggedIn) {
        cy.contains('Keep me logged in.').click();
    }
    // Click the login button
    cy.contains('button', 'Login').click();

    // Bypass 2FA if requested
    if (bypass2FA) {
        // Mock the 2FA validation
        cy.intercept('POST', '/Access/Validate', (req) => {
            const loginModel = lm; // Use the provided login model
            req.reply((res) => {
                // Simulate the setting of TempData
                const tempData = {
                    "LoginModel": JSON.stringify(loginModel),
                    "2FANumber": '123456' // Mock the 2FA number
                };
                
                // You can set the response to return a redirect after successful validation
                res.send({
                    status: 'success',
                    tempData: tempData
                });
            });
        }).as('validateUser');

        // Enter the mocked 2FA code
        cy.get('input[name="TwoFa"]').type('123456'); // Adjust the selector as needed
        cy.contains('button', 'Valideer').click();

        // Wait for the validation response and check for redirect
        cy.wait('@validateUser').its('response.statusCode').should('eq', 302);
        cy.url().should('include', '/Home'); // Validate redirection to the Home page
    }

    // Verify that the logout link is visible
    cy.contains('a', 'Uitloggen').should('be.visible');
});

