function placePieces(gameState, spelers, gebruiker) {
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

        // Use an IIFE to create a new scope for each iteration
        (function (i) {
            pieceDiv.addEventListener('click', function () {
                getPossibleMoves(i, gameState, gebruiker, spelers);
            });
        })(i);
    }
}

function getPossibleMoves(i, gameState, gebruiker, spelers) {
    // Convert the gameState string into a 2D array
    var gameStateArray = [];
    for (let j = 0; j < 8; j++) {
        gameStateArray[j] = [];
        for (let k = 0; k < 8; k++) {
            gameStateArray[j][k] = parseInt(gameState.charAt(j * 8 + k));
        }
    }

    // Get the clicked piece and its parent square
    var square = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);
    var piece = square.firstChild;

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
    var shadowRoot = document.querySelector('#shadowHost').shadowRoot;
    var existingCircles = shadowRoot.querySelectorAll('.grey-circle');
    existingCircles.forEach(function (circle) {
        circle.remove();
    });

    // Add a grey circle to the squares diagonally ahead
    [leftSquareIndex, rightSquareIndex].forEach(function (index) {
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
            squareDiv.appendChild(circleDiv);
        }
    });
}
