let needsLogin = false;
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
let currentUser;
const setupUserSession = (user) => {
    cy.session(user.name, () => {
        cy.login(user.email, user.password, true);
        cy.visit('http://145.44.234.91');
        const today = new Date();
        const date = today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear();
        const time = today.getHours() + ':' + today.getMinutes() + ':' + today.getSeconds();

        cy.window().then((win) => {
            win.localStorage.setItem('gdpr-consent-choice', 'accept');
            win.localStorage.setItem('time-of-interaction', JSON.stringify({ datum: date, tijd: time }));
        });
        cy.contains('button', 'Ja, natuurlijk').click();
    });
};
Cypress.on('uncaught:exception', () => { return false })
describe('All Showcase tests for the frontend using Cypress', () => {
    it('Visits the Showcase', () => {
        cy.visit('http://145.44.234.91');
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
    beforeEach(() => {
        if (currentUser === testUser.name) {
            setupUserSession(testUser);
        } else if (currentUser === realUser.name) {
            setupUserSession(realUser);
        }

    });

    it('should log in as TestUser and show the Privacy page', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Privacy').click();
    });
    it('should log in as TestUser and show the Profile page', () => {
        currentUser = testUser.name;
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Profiel').click();
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
        currentUser = realUser.name;
    });
    it('should log in as RealUser and join TestUser dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('button')
            .contains('Meedoen')
            .click();
        currentUser = testUser.name;
    });

    it('should log in as TestUser and log out', () => {
        
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Uitloggen').click();
    });
    currentUser = realUser.name;
    it('should log in as RealUser and show the Privacy page', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Privacy').click();
    });

});
describe('Dammen tests', () => {
    beforeEach(() => {
        if (currentUser === testUser.name) {
            setupUserSession(testUser);
        } else if (currentUser === realUser.name) {
            setupUserSession(realUser);
        }

    });
    it('should log in as TestUser and view his dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('a')
            .contains('Bekijken')
            .click();
    });
    it('should log in as TestUser and make a move in his dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('a')
            .contains('Bekijken')
            .click();
        cy.wait(5000);
        cy.get('#shadowHost').then((shadowHost) => {
            // Access the shadow root
            const shadowRoot = shadowHost[0].shadowRoot;

            // Use querySelector to find the square and then the piece inside
            let square = shadowRoot.querySelector('#square21');
            let piece;
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('#piece1');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
            square = shadowRoot.querySelector('#square30');
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('.grey-circle');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
        });
        cy.wait(5000);
        currentUser = realUser.name;
    });
    it('should log in as RealUser and make a move in his dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('a')
            .contains('Bekijken')
            .click();
        cy.wait(5000);
        cy.get('#shadowHost').then((shadowHost) => {
            // Access the shadow root
            const shadowRoot = shadowHost[0].shadowRoot;

            // Use querySelector to find the square and then the piece inside
            let square = shadowRoot.querySelector('#square44');
            let piece;
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('#piece2');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
            square = shadowRoot.querySelector('#square37');
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('.grey-circle');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
        });
        cy.wait(5000);
        currentUser = testUser.name;
    });
    it('should log in as TestUser and make a capture move in his dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('a')
            .contains('Bekijken')
            .click();
        cy.wait(5000);
        cy.get('#shadowHost').then((shadowHost) => {
            // Access the shadow root
            const shadowRoot = shadowHost[0].shadowRoot;

            // Use querySelector to find the square and then the piece inside
            let square = shadowRoot.querySelector('#square30');
            let piece;
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('#piece1');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
            square = shadowRoot.querySelector('#square44');
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('.grey-circle');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
        });
        cy.wait(5000);
        currentUser = realUser.name;
    });
    it('should log in as RealUser and make a non-capture-forced move in his dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('a')
            .contains('Bekijken')
            .click();
        cy.wait(5000);
        cy.get('#shadowHost').then((shadowHost) => {
            // Access the shadow root
            const shadowRoot = shadowHost[0].shadowRoot;

            // Use querySelector to find the square and then the piece inside
            let square = shadowRoot.querySelector('#square42');
            let piece;
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('#piece2');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
            square = shadowRoot.querySelector('#square33');
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('.grey-circle');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
        });
        cy.wait(5000);
        currentUser = testUser.name;
    });
    it('should log in as TestUser and make a non-capture-forced move in his dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('a')
            .contains('Bekijken')
            .click();
        cy.wait(5000);
        cy.get('#shadowHost').then((shadowHost) => {
            // Access the shadow root
            const shadowRoot = shadowHost[0].shadowRoot;

            // Use querySelector to find the square and then the piece inside
            let square = shadowRoot.querySelector('#square23');
            let piece;
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('#piece1');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
            square = shadowRoot.querySelector('#square30');
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('.grey-circle');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
        });
        cy.wait(5000);
        currentUser = realUser.name;
    });
    it('should log in as RealUser and make the first move in a double move in his dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('a')
            .contains('Bekijken')
            .click();
        cy.wait(5000);
        cy.get('#shadowHost').then((shadowHost) => {
            // Access the shadow root
            const shadowRoot = shadowHost[0].shadowRoot;

            // Use querySelector to find the square and then the piece inside
            let square = shadowRoot.querySelector('#square51');
            let piece;
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('#piece2');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
            square = shadowRoot.querySelector('#square37');
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('.grey-circle');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }

            cy.wait(5000);
        });
    });
    it('should log in as RealUser and make the second move in a double move in his dammen game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('table')
            .contains('td', 'TestGame')
            .parents('tr')
            .find('a')
            .contains('Bekijken')
            .click();
        cy.wait(5000);
        cy.get('#shadowHost').then((shadowHost) => {
            // Access the shadow root
            const shadowRoot = shadowHost[0].shadowRoot;

            // Use querySelector to find the square and then the piece inside
            let square = shadowRoot.querySelector('#square37');
            let piece;
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('#piece2');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }
            square = shadowRoot.querySelector('#square23');
            if (square) {
                // Check if the square is visible
                expect(square).to.be.visible;

                // Find the inner piece
                piece = square.querySelector('.grey-circle');
                if (piece) {
                    // Check if the piece is visible
                    expect(piece).to.be.visible;

                    // Click the inner piece
                    piece.click();
                } else {
                    throw new Error('Piece not found');
                }
            } else {
                throw new Error('Square not found');
            }

            cy.wait(5000);
        });
        currentUser = testUser.name;
    });
    it('should remove TestUsers game', () => {
        cy.visit('http://145.44.234.91');
        cy.contains('a', 'Dammen!').click();
        cy.get('h2').contains('Jouw games')
            .next('table')
            .find('td')
            .contains('TestGame')
            .parents('tr')
            .find('button')
            .contains('verwijderen')
            .click();
    })
});

