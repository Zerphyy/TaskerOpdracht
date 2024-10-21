let needsLogin = false; // Flag to track login status

describe('All Showcase tests for the frontend using Cypress', () => {
    it('Visits the Showcase', () => {
        cy.visit('http://145.44.234.91');
    });

    it('Accepts the GDPR', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('button', 'Ja, natuurlijk').click();
    });

    it('cant login without giving account creds', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('button', 'Login').click();
    });

    it('is able to navigate to the register page', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Registreer').click();
    });
});
describe('Logged-in User Tests', () => {
    let testUser = {
        name: 'TestUser',
        email: 'testmail@example.com',
        password: 'testUser123!',
    };

    let realUser = {
        name: 'RealUser',
        email: 'kevinspijker@kpnmail.nl',
        password: 'Plusklas01!',
    };

    // Use this variable to track which user you want to log in as
    let currentUser;

    beforeEach(() => {
        // Log in to the current user based on which test is being run
        if (currentUser === testUser.name) {
            cy.session(testUser.name, () => {
                cy.login(testUser.email, testUser.password, true);
            });
        } else if (currentUser === realUser.name) {
            cy.session(realUser.name, () => {
                cy.login(realUser.email, realUser.password, true);
            });
        }
    });

    it('should log in as TestUser and show the Privacy page', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Privacy').click();
    });

    it('should log in as TestUser and show the CV page', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Curriculum Vitae').click();
    });

    it('should log in as TestUser and show the Dammen page', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
    });

    it('should log in as TestUser and show the Contact page', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Contact').click();
    });

    it('should log in as TestUser and not show the Management page', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Gebruikers Beheren').should('not.exist');
    });

    it('should log in as TestUser and show the Dammen Create page', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.contains('a', 'Maak spel aan').click();
    });

    it('should log in as TestUser and create a Dammen game', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.contains('a', 'Maak spel aan').click();
        cy.get('input[placeholder="Checkers is amazing!"]').type('TestGame');
        cy.get('input[type="Submit"]').click();
    });

    it('should log in as TestUser and log out', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Uitloggen').click();
    });
    currentUser = realUser.name;
    it('should log in as RealUser and show the Privacy page', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Privacy').click();
    });

});

