function placePieces(gameState) {
    // Iterate over the characters in the gameState string
    for (var i = 0; i < gameState.length; i++) {
        // Get the state of the square from the gameState string
        var squareState = parseInt(gameState.charAt(i));

        // Skip if the square is empty
        if (squareState === 0) continue;

        // Get the corresponding square div from the board
        var squareDiv = document.querySelector('#shadowHost').shadowRoot.querySelector('#square' + i);

        // Create a new div for the piece
        var pieceDiv = document.createElement('div');

        // Set the size and shape of the piece
        pieceDiv.style.width = '75%';
        pieceDiv.style.height = '75%';
        pieceDiv.style.borderRadius = '50%';  // Make the piece round
        pieceDiv.style.position = 'relative';
        pieceDiv.style.left = '12.5%';  // Center the piece horizontally
        pieceDiv.style.top = '12.5%';  // Center the piece vertically

        // Set the color of the piece based on the state
        switch (squareState) {
            case 1:
                pieceDiv.style.backgroundColor = 'red'; // Player 1's normal piece
                break;
            case 2:
                pieceDiv.style.backgroundColor = 'black'; // Player 2's normal piece
                break;
            case 3:
                pieceDiv.style.backgroundColor = 'darkred'; // Player 1's upgraded piece
                break;
            case 4:
                pieceDiv.style.backgroundColor = 'darkgray'; // Player 2's upgraded piece
                break;
        }

        // Add the piece div to the square div
        squareDiv.appendChild(pieceDiv);
    }
}
