//function placePieces(gameState, spelers, gebruiker) {
//    for (let i = 0; i < gameState.length; i++) {
//        var squareState = parseInt(gameState.charAt(i));
//        if (squareState === 0) continue;

//        var squareDiv = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);

//        var pieceDiv = document.createElement('div');
//        pieceDiv.style.width = '75%';
//        pieceDiv.style.height = '75%';
//        pieceDiv.style.borderRadius = '50%';
//        pieceDiv.style.position = 'relative';
//        pieceDiv.style.left = '12.5%';
//        pieceDiv.style.top = '12.5%';

//        switch (squareState) {
//            case 1:
//                pieceDiv.style.backgroundColor = 'red';
//                break;
//            case 2:
//                pieceDiv.style.backgroundColor = 'black';
//                break;
//            case 3:
//                pieceDiv.style.backgroundColor = 'darkred';
//                break;
//            case 4:
//                pieceDiv.style.backgroundColor = 'darkgray';
//                break;
//        }

//        squareDiv.appendChild(pieceDiv);

//        // Use an IIFE to create a new scope for each iteration
//        (function (i) {
//            pieceDiv.addEventListener('click', function () {
//                getPossibleMoves(i, gameState, gebruiker, spelers);
//            });
//        })(i);
//    }
//}
var CheckersModule = (function () {
    var gameStateArray = [];
    var selectedPiece = null;
    var gameState = null;
    var gebruiker = null;
    var spelers = null;

    return {
        getGameStateArray: function () {
            return gameStateArray;
        },
        setGameStateArray: function (gameState) {
            for (let j = 0; j < 8; j++) {
                gameStateArray[j] = [];
                for (let k = 0; k < 8; k++) {
                    gameStateArray[j][k] = parseInt(gameState.charAt(j * 8 + k));
                }
            }
        },
        getSelectedPiece: function () {
            return selectedPiece;
        },
        setSelectedPiece: function (piece) {
            selectedPiece = piece;
        },
        getShadowRoot: function () {
            return document.querySelector('#shadowHost').shadowRoot;
        },
        getAvailableMovePositions: function () {
            return document.querySelector('#shadowHost').shadowRoot.querySelectorAll('.grey-circle');
        },
        removeAvailableMovePositions: function () {
            this.getAvailableMovePositions().forEach(function (circle) {
                circle.remove();
            });
        },
        getGameState: function () {
            return gameState;
        },
        setGameState: function (state) {
            gameState = state;
        },
        getGebruiker: function () {
            return gebruiker;
        },
        setGebruiker: function (user) {
            gebruiker = user;
        },
        getSpelers: function () {
            return spelers;
        },
        setSpelers: function (players) {
            spelers = players;
        }
    };
})();

function placePieces(gameState, spelers, gebruiker) {
    CheckersModule.setGameState(gameState);
    CheckersModule.setGameStateArray(gameState);
    CheckersModule.setSpelers(spelers);
    CheckersModule.setGebruiker(gebruiker);
    for (let i = 0; i < gameState.length; i++) {
        var squareState = parseInt(gameState.charAt(i));
        if (squareState === 0) continue;

        var squareDiv = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);

        var pieceDiv = document.createElement('div');
        pieceDiv.style.width = '75%';
        pieceDiv.style.height = '75%';
        pieceDiv.style.borderRadius = '50%';
        pieceDiv.style.position = 'relative';
        pieceDiv.style.left = '12.5%';
        pieceDiv.style.top = '12.5%';

        switch (squareState) {
            case 1:
                pieceDiv.style.backgroundColor = 'red';
                break;
            case 2:
                pieceDiv.style.backgroundColor = 'black';
                break;
            case 3:
                pieceDiv.style.backgroundColor = 'darkred';
                break;
            case 4:
                pieceDiv.style.backgroundColor = 'darkgray';
                break;
        }

        squareDiv.appendChild(pieceDiv);

        // Check if the piece belongs to the current player
        if ((gebruiker === spelers[0] && (squareState === 1 || squareState === 3)) ||
            (gebruiker === spelers[1] && (squareState === 2 || squareState === 4))) {
            // Use an IIFE to create a new scope for each iteration
            (function (i, squareDiv) {
                var pieceDiv = squareDiv.firstChild;
                var clickHandler = function () {
                    CheckersModule.setSelectedPiece(squareDiv);
                    getPossibleMoves(i, gameState, gebruiker, spelers);
                };
                pieceDiv.addEventListener('click', clickHandler);
                pieceDiv.clickHandler = clickHandler;  // Store the handler for later removal
            })(i, squareDiv);
        }
    }
}


function getPossibleMoves(i, gameState, gebruiker, spelers) {
    console.log("Press detected ", CheckersModule.getSelectedPiece());
    // Convert the gameState string into a 2D array
    var gameStateArray = CheckersModule.getGameStateArray();

    // Get the clicked piece and its parent square
    var square = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);
    var piece = square.firstChild;
    selectedPiece = piece;
    // Calculate the positions of the squares diagonally ahead
    var row = Math.floor(i / 8);
    var col = i % 8;
    var leftSquareIndex, rightSquareIndex;
    if (gebruiker === spelers[0]) {
        leftSquareIndex = col === 0 ? null : { row: row + 1, col: col - 1 };
        rightSquareIndex = col === 7 ? null : { row: row + 1, col: col + 1 };
    } else {
        leftSquareIndex = col === 0 ? null : { row: row - 1, col: col + 1 };
        rightSquareIndex = col === 7 ? null : { row: row - 1, col: col - 1 };
    }

    // Remove existing grey circles
    CheckersModule.removeAvailableMovePositions();
    var shadowRoot = CheckersModule.getShadowRoot();
    // Add a grey circle to the squares diagonally ahead
    [leftSquareIndex, rightSquareIndex].forEach(function (index) {
        console.log(gameStateArray, index);
        if (index !== null && index.row >= 0 && index.row < 8 && index.col >= 0 && index.col < 8 && gameStateArray[index.row][index.col] === 0) {
            var squareDiv = shadowRoot.querySelector('#square' + (index.row * 8 + index.col));
            var circleDiv = document.createElement('div');
            circleDiv.style.width = '50%';
            circleDiv.style.height = '50%';
            circleDiv.style.borderRadius = '50%';
            circleDiv.style.position = 'relative';
            circleDiv.style.left = '25%';
            circleDiv.style.top = '25%';
            circleDiv.style.backgroundColor = 'grey';
            circleDiv.className = 'grey-circle';
            (function (squareDiv) {
                circleDiv.addEventListener('click', function () {
                    movePiece(squareDiv);
                });
            })(squareDiv);
            squareDiv.appendChild(circleDiv);
        }
    });
}

function movePiece(squareDiv) {
    let square = CheckersModule.getSelectedPiece();
    let piece = square.firstChild;
    square.removeChild(piece);
    squareDiv.appendChild(piece);
    piece.removeEventListener('click', piece.clickHandler);  // Remove the old event listener

    // Add a new event listener with the updated squareDiv
    var clickHandler = function () {
        CheckersModule.setSelectedPiece(squareDiv);
        getPossibleMoves(i, CheckersModule.getGameState(), CheckersModule.getGebruiker(), CheckersModule.getSpelers());
    };
    piece.addEventListener('click', clickHandler);
    piece.clickHandler = clickHandler;  // Store the handler for later removal

    CheckersModule.setSelectedPiece(piece);  // Update the selected piece to the moved piece
    CheckersModule.removeAvailableMovePositions();  // Remove the grey circles
}
